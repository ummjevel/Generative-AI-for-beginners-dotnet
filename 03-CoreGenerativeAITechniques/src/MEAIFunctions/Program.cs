using Azure.AI.Inference;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

ChatOptions options = new ChatOptions
{
    Tools = [
        AIFunctionFactory.Create(GetTheWeather)
    ]
};


IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.inference.ai.azure.com"),
    new AzureKeyCredential(githubToken))
    .AsChatClient("gpt-4o-mini")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var response = await client.CompleteAsync("Do I need an umbrella today?", options);

Console.WriteLine(response);


[Description("Get the weather")]
static string GetTheWeather()
{
    var temperature = Random.Shared.Next(5, 20);

    var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

    return $"The weather is {temperature} degrees C and {conditions}.";
}