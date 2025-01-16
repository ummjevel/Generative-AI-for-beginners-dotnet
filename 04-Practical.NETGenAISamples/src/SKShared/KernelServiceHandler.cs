#pragma warning disable SKEXP0001, SKEXP0110, OPENAI001

// Local function to invoke agent and display the conversation messages.
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using OpenAI.Assistants;
using OpenAI.Chat;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace AgentLabs;

public class KernelServiceHandler
{
    public static Kernel CreateKernelWithChatCompletion()
    {
        var builder = Kernel.CreateBuilder();
        AddChatCompletionToKernel(builder);
        return builder.Build();
    }

    public static void AddChatCompletionToKernel(IKernelBuilder builder)
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var apiKey = config["AZURE_OPENAI_APIKEY"];
        var endpoint = config["AZURE_OPENAI_ENDPOINT"];
        var deployment = config["AZURE_OPENAI_DEPLOYMENT"];

        builder.AddAzureOpenAIChatCompletion(deployment, endpoint, apiKey);
        
    }

}