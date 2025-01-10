using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if(string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken))
        .AsChatClient("Phi-3.5-MoE-instruct");

var response = await client.CompleteAsync("What is AI?");

Console.WriteLine(response.Message);