using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpaceAINet.mjjeon
{
    public class OllamaAIService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;
        private string model;
        private readonly string[] fallbackModels = { "phi4-mini", "llama3", "phi3", "llama3.2", "codellama", "tinyllama", "qwen", "mistral" };

        public OllamaAIService(string baseUrl = "http://localhost:11434", string model = "phi4-mini")
        {
            this.baseUrl = baseUrl;
            this.model = model;
            this.httpClient = new HttpClient();
            this.httpClient.Timeout = TimeSpan.FromSeconds(15); // Increased timeout
            // Console.WriteLine($"[DEBUG] OllamaAIService initialized with model: {model}, URL: {baseUrl}");
        }

        public async Task<string> GetNextActionAsync(string gameState, string lastAction)
        {
            // Try current model first, then fallback models
            string[] modelsToTry = new[] { model }.Concat(fallbackModels.Where(m => m != model)).ToArray();
            
            foreach (string tryModel in modelsToTry)
            {
                try
                {
                    var result = await TryGetActionWithModel(tryModel, gameState, lastAction);
                    if (result != null)
                    {
                        // Update current model to working one
                        if (tryModel != model)
                        {
                            // Console.WriteLine($"[DEBUG] Switched to working model: {tryModel}");
                            model = tryModel;
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine($"[DEBUG] Model {tryModel} failed: {ex.Message}");
                    continue;
                }
            }
            
            // Console.WriteLine("[DEBUG] All models failed, returning stop");
            return "stop";
        }

        private async Task<string?> TryGetActionWithModel(string tryModel, string gameState, string lastAction)
        {
            try
            {
                var prompt = $@"You are playing a Space Invaders game. Analyze the current game state and decide the next action.

Game State:
{gameState}

Last Action: {lastAction}

Available Actions:
- move_left: Move the player ship left
- move_right: Move the player ship right  
- fire: Shoot a bullet
- stop: Do nothing

Rules:
1. Your ship is represented by 'A' at the bottom
2. Enemies are represented by '><', 'oo', and '/O\' patterns
3. Your bullets are '^' moving up
4. Enemy bullets are 'v' moving down
5. Try to shoot enemies while avoiding enemy bullets
6. Move strategically to avoid collisions

Respond with ONLY one of these actions: move_left, move_right, fire, stop

Action:";

                var requestBody = new
                {
                    model = tryModel,
                    prompt = prompt,
                    stream = false,
                    options = new
                    {
                        temperature = 0.3,
                        max_tokens = 20,
                        stop = new[] { "\n", "." }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Console.WriteLine($"[DEBUG] Trying model: {tryModel}");
                var response = await httpClient.PostAsync($"{baseUrl}/api/generate", content);
                
                // Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine($"[DEBUG] Raw response: {responseContent}");
                    
                    var responseJson = JsonDocument.Parse(responseContent);
                    var aiResponse = responseJson.RootElement.GetProperty("response").GetString();
                    
                    // Console.WriteLine($"[DEBUG] AI response: '{aiResponse}'");
                    
                    // Extract action from response
                    var action = ExtractAction(aiResponse ?? "stop");
                    // Console.WriteLine($"[DEBUG] Extracted action: '{action}'");
                    return action;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine($"[DEBUG] Error response: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"[DEBUG] Exception with model {tryModel}: {ex.Message}");
                return null;
            }
        }

        private string ExtractAction(string response)
        {
            if (string.IsNullOrEmpty(response))
                return "stop";

            var lowerResponse = response.ToLower().Trim();
            // Console.WriteLine($"[DEBUG] Parsing response: '{lowerResponse}'");
            
            // Check for exact matches first
            if (lowerResponse == "move_left" || lowerResponse == "left")
                return "move_left";
            if (lowerResponse == "move_right" || lowerResponse == "right")
                return "move_right";
            if (lowerResponse == "fire" || lowerResponse == "shoot")
                return "fire";
            if (lowerResponse == "stop")
                return "stop";
            
            // Check for contains
            if (lowerResponse.Contains("move_left") || lowerResponse.Contains("left"))
                return "move_left";
            else if (lowerResponse.Contains("move_right") || lowerResponse.Contains("right"))
                return "move_right";
            else if (lowerResponse.Contains("fire") || lowerResponse.Contains("shoot"))
                return "fire";
            
            // If no match found, default to fire (more aggressive)
            // Console.WriteLine($"[DEBUG] No action match found, defaulting to fire");
            return "fire";
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                // Console.WriteLine($"[DEBUG] Checking Ollama availability at: {baseUrl}");
                var response = await httpClient.GetAsync($"{baseUrl}/api/tags");
                // Console.WriteLine($"[DEBUG] Ollama availability check status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine($"[DEBUG] Available models: {content}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"[DEBUG] Ollama not available: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
