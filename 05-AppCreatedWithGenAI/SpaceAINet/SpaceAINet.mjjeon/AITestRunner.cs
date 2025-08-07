using System;
using System.Threading.Tasks;

namespace SpaceAINet.mjjeon
{
    class AITestRunner
    {
        public static async Task TestOllamaConnection()
        {
            Console.WriteLine("=== Ollama AI Connection Test ===\n");
            
            var aiService = new OllamaAIService();
            
            // Test 1: Check availability
            Console.WriteLine("1. Testing Ollama availability...");
            bool isAvailable = await aiService.IsAvailableAsync();
            Console.WriteLine($"   Result: {(isAvailable ? "✅ Available" : "❌ Not Available")}\n");
            
            if (!isAvailable)
            {
                Console.WriteLine("❌ Ollama is not available. Please:");
                Console.WriteLine("   1. Install Ollama from https://ollama.com/");
                Console.WriteLine("   2. Run 'ollama serve' in terminal");
                Console.WriteLine("   3. Run 'ollama pull llama3.2' to download model");
                return;
            }
            
            // Test 2: Test AI response
            Console.WriteLine("2. Testing AI response...");
            var testGameState = @"Score: 0, Time: 10s
Player Position: X=15, Y=20
Player Bullets: 0/3

Game Field:
......><....oo....><......................
....................../O\................
..........................................
........................A.................";

            try
            {
                string action = await aiService.GetNextActionAsync(testGameState, "stop");
                Console.WriteLine($"   AI Action: {action}");
                
                if (action != "stop")
                {
                    Console.WriteLine("✅ AI is working correctly!");
                }
                else
                {
                    Console.WriteLine("⚠️  AI returned 'stop' - check debug logs above");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            
            Console.WriteLine("\n=== Test Complete ===");
            aiService.Dispose();
        }
    }
}
