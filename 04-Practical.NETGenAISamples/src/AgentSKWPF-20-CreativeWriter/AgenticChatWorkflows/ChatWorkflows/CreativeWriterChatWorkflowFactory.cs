using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel.DataAnnotations;
using AgentsSK_20_CreativeWriter.Helpers;


#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0050, CS8600, CS8604

namespace AgentsSK_20_CreativeWriter;

public static class CreativeWriterChatWorkflowFactory
{
    private const string Researcher = "Researcher";
    private const string CreativeWriter = "CreativeWriter";
    private const string Critic = "Critic";
    private const string TerminationKeyword = "approved";

    // Lazy Kernel initialization
    private static Kernel? _kernel;
    public static Kernel Kernel => _kernel ??= CreateKernel();

    // Create the Kernel lazily using the environment variables
    private static Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton<IFunctionInvocationFilter, SearchFunctionFilter>();

        Kernel kernel = builder.AddAzureOpenAIChatCompletion(
                            deploymentName: EnvironmentWellKnown.DeploymentName,
                            endpoint: EnvironmentWellKnown.Endpoint,
                            apiKey: EnvironmentWellKnown.ApiKey)
                        .Build();

        BingConnector bing = new BingConnector(EnvironmentWellKnown.BingApiKey);
        kernel.ImportPluginFromObject(new WebSearchEnginePlugin(bing), "bing");

        return kernel;
    }

    public static IAgentGroupChat CreateChat(int characterLimit = 2000, int maxIterations = 3)
    {
        // Create agents using separate methods        
        ChatCompletionAgent researcher = CreateResearcherAgent();
        ChatCompletionAgent creativeWriter = CreateCreativeWriterAgent(maxIterations);
        ChatCompletionAgent critic = CreateCriticAgent();

        var twoAgentChat = new ResearcherWriterCriticChat(
            researcher,
            creativeWriter,
            critic,
            maxIterations, 
            TerminationKeyword);

        twoAgentChat.IsComplete = false;

        return twoAgentChat;
    }


    // Method to create the CreativeWriter Agent
    private static ChatCompletionAgent CreateCreativeWriterAgent(int maxIterations = 3)
    {
        string copyWriterInstructions = $"""
            You are a writer with ten years of experience and are known for brevity and a dry humor.
            The goal is to refine and decide on the single best copy as an expert in the field.
            The researcher will share with you information from online sources, that you will use in your creative process.
            Only provide a single proposal per response.
            You're laser focused on the goal at hand.

            The whole creative process should not take more than {maxIterations} iterations.
            """;

        return new ChatCompletionAgent
        {
            Instructions = copyWriterInstructions,
            Name = CreativeWriter,
            Kernel = Kernel,
        };
    }

    private static ChatCompletionAgent CreateResearcherAgent()
    {
        string researcherInstructions = $"""
                You are an expert searching the web. 
                Research using the Bing Search plugin search engine on the requested topic.
                Always return the search results and the source Urls from the found documentation.
                Only return the results of the search results including the following information:
                - Search Result: [the result from the search query]
                - Source Url: [source Url]
                - Result Content: [if available]
                - Context: [if available]
                Do not return any other text.
                """;

        var researcherAgent = new ChatCompletionAgent()
        {
            Instructions = researcherInstructions,
            Name = Researcher,
            Kernel = Kernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                }),
        };
        return researcherAgent;
    }

    private static ChatCompletionAgent CreateCriticAgent()
    {
        const string metaReviewerInstructions = @$"You are a critic reviewer. You aggregate and review the work of other writers and give a summarized final review from writer on the content. 
Your goal is to determine if the given copy is acceptable to print.
If so, state that it is approved. Say ""{TerminationKeyword}"" to approve the copy.";

        return new ChatCompletionAgent
        {
            Instructions = metaReviewerInstructions,
            Name = Critic,
            Kernel = Kernel,
        };
    }
}