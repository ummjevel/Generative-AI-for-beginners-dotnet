using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")))
        .AsChatClient("Phi-3.5-MoE-instruct");

var response = await client.CompleteAsync("What is AI?");

Console.WriteLine(response.Message);