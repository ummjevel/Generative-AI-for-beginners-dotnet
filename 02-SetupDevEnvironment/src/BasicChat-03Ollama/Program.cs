using Microsoft.Extensions.AI;

// you can test with the models "llama3.2" and "phi3.5"
// to test other models you can download them with the command "ollama pull <modelId>"
// in example: "ollama pull deepseek-r1" or "ollama pull phi4-mini" (for the phi4-mini model which is still being tested)
IChatClient client =
    new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2");

var response = client.GetStreamingResponseAsync("What is AI?");
await foreach (var item in response)
{
    Console.Write(item);
}