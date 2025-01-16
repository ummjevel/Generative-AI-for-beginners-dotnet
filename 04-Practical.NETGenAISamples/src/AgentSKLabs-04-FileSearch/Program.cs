#pragma warning disable SKEXP0001, SKEXP0110, OPENAI001

// Define the agent
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using OpenAI.Files;
using OpenAI.VectorStores;
using Microsoft.SemanticKernel.Agents.OpenAI;
using System.ClientModel;
using OpenAI.Assistants;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var apiKey = config["AZURE_OPENAI_APIKEY"];
var endpoint = config["AZURE_OPENAI_ENDPOINT"];
var modelId = config["AZURE_OPENAI_MODEL"];

var openAiAssistantDefinition = new OpenAIAssistantDefinition(modelId)
{
    EnableFileSearch = true,
    Metadata = new Dictionary<string, string>{
        { "file_search", "true" }
    },
    Instructions = "This assistant can help you with your questions. You can ask about camping, healthcare benefits, and more. The assistant will include and share information to existing content in the related information database. For each shared information, the assistant will declare that this is based on existing content and provide the necessary link to it.",
};

var provider = OpenAIClientProvider.ForAzureOpenAI(new ApiKeyCredential(apiKey), new Uri(endpoint));

OpenAIAssistantAgent agent = 
    await OpenAIAssistantAgent.CreateAsync( 
        clientProvider: provider,
        definition: openAiAssistantDefinition,
        kernel: new Kernel());

// Upload files - Using a table of fictional employees.
// iterate through the folder sampledocs
OpenAIFileClient fileClient = provider.Client.GetOpenAIFileClient();
List<string> fileIds = [];
foreach (var file in Directory.GetFiles("sampledocs"))
{
    await using Stream stream = File.OpenRead(file);
    OpenAIFile fileInfo = await fileClient.UploadFileAsync(stream, Path.GetFileName(file), FileUploadPurpose.Assistants);
    fileIds.Add(fileInfo.Id);
}

// Create a vector-store
var vectorStoreOptions = new VectorStoreCreationOptions()
{
    Name = "employees_vector_store"
};
foreach (var fileId in fileIds)
{
    vectorStoreOptions.FileIds.Add(fileId);
}
VectorStoreClient vectorStoreClient = provider.Client.GetVectorStoreClient();
CreateVectorStoreOperation result = await vectorStoreClient.CreateVectorStoreAsync(waitUntilCompleted: true, vectorStoreOptions);

// Create a thread associated with a vector-store for the agent conversation.
string threadId =
    await agent.CreateThreadAsync(
        new OpenAIThreadCreationOptions
        {
            VectorStoreId = result.VectorStoreId
        });

// Respond to user input
try
{
    await InvokeAgentAsync("I want to go camping, but it seems that it maybe raining, what products do you suggest to go camping on rainy days?");
    await InvokeAgentAsync("what healthcare benefits are part of the Northwind Health plans?");
}
finally
{
    await agent.DeleteThreadAsync(threadId);
    await agent.DeleteAsync();
    await vectorStoreClient.DeleteVectorStoreAsync(result.VectorStoreId);

    // Delete the uploaded files
    foreach (var fileInfo in fileIds)
        await fileClient.DeleteFileAsync(fileInfo);
}

// Local function to invoke agent and display the conversation messages.
async Task InvokeAgentAsync(string input)
{
    Microsoft.SemanticKernel.ChatMessageContent message = new(AuthorRole.User, input);
    await agent.AddChatMessageAsync(threadId, message);
    WriteAgentChatMessage(message);

    await foreach (Microsoft.SemanticKernel.ChatMessageContent response in agent.InvokeAsync(threadId))
    {
        WriteAgentChatMessage(response);
    }
}
void WriteAgentChatMessage(Microsoft.SemanticKernel.ChatMessageContent message)
{
    // Include ChatMessageContent.AuthorName in output, if present.
    string authorExpression = message.Role == AuthorRole.User ? string.Empty : $" - {message.AuthorName ?? "*"}";

    // Include TextContent (via ChatMessageContent.Content), if present.
    string contentExpression = string.IsNullOrWhiteSpace(message.Content) ? string.Empty : message.Content;
    bool isCode = message.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false;
    string codeMarker = isCode ? "\n  [CODE]\n" : " ";
    Console.WriteLine($"\n# {message.Role}{authorExpression}:{codeMarker}{contentExpression}");

    // Provide visibility for inner content (that isn't TextContent).
    foreach (KernelContent item in message.Items)
    {
        if (item is AnnotationContent annotation)
        {
            Console.WriteLine($"  [{item.GetType().Name}] {annotation.Quote}: File #{annotation.FileId}");
        }
        else if (item is FileReferenceContent fileReference)
        {
            Console.WriteLine($"  [{item.GetType().Name}] File #{fileReference.FileId}");
        }
        else if (item is ImageContent image)
        {
            Console.WriteLine($"  [{item.GetType().Name}] {image.Uri?.ToString() ?? image.DataUri ?? $"{image.Data?.Length} bytes"}");
        }
        else if (item is FunctionCallContent functionCall)
        {
            Console.WriteLine($"  [{item.GetType().Name}] {functionCall.Id}");
        }
        else if (item is FunctionResultContent functionResult)
        {
            Console.WriteLine($"  [{item.GetType().Name}] {functionResult.CallId} - {functionResult.Result?.AsJson() ?? "*"}");
        }
    }

    if (message.Metadata?.TryGetValue("Usage", out object? usage) ?? false)
    {
        if (usage is RunStepTokenUsage assistantUsage)
        {
            WriteUsage(assistantUsage.TotalTokenCount, assistantUsage.InputTokenCount, assistantUsage.OutputTokenCount);
        }
        else if (usage is ChatTokenUsage chatUsage)
        {
            WriteUsage(chatUsage.TotalTokenCount, chatUsage.InputTokenCount, chatUsage.OutputTokenCount);
        }
    }

    void WriteUsage(int totalTokens, int inputTokens, int outputTokens)
    {
        Console.WriteLine($"  [Usage] Tokens: {totalTokens}, Input: {inputTokens}, Output: {outputTokens}");
    }
}