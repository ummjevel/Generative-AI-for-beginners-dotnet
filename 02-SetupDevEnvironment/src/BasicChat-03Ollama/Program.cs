using Microsoft.Extensions.AI;

IChatClient client =
    new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2");

var response = client.CompleteStreamingAsync("What is AI?");
await foreach (var item in response)
{
    Console.Write(item);
}