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
using AgentLabs;

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
    await AgentServiceHandler.InvokeAgentAsync(agent, threadId, "I want to go camping, but it seems that it maybe raining, what products do you suggest to go camping on rainy days?");
    await AgentServiceHandler.InvokeAgentAsync(agent, threadId, "what healthcare benefits are part of the Northwind Health plans?");
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

