using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

// Create Agent Client
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var options = new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeWorkloadIdentityCredential = true,
    TenantId = config["tenantid"]
};
var connectionString = config["connectionString"];
var client = new AgentsClient(connectionString, new DefaultAzureCredential(options));

// upload the files and get the file Ids
var fileIds = new ConcurrentBag<string>();
var fileToId = new ConcurrentDictionary<string, string>();
var files = Directory.EnumerateFiles("products");

await Task.WhenAll(files.Select(async file =>
{
    var uploadFileResponse = await client.UploadFileAsync(filePath: file, purpose: AgentFilePurpose.Agents);
    fileIds.Add(uploadFileResponse.Value.Id);
    fileToId.TryAdd(file, uploadFileResponse.Value.Id);
}));

// Create a vector store with the files and wait for it to be processed.
VectorStore vectorStore = await client.CreateVectorStoreAsync(
    fileIds: fileIds.ToList(),
    name: "products_vector_store");

FileSearchToolResource fileSearchToolResource = new FileSearchToolResource();
fileSearchToolResource.VectorStoreIds.Add(vectorStore.Id);

// Create an agent with toolResources and process assistant run
Response<Agent> agentResponse = await client.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "Outdoor Sales Assistants",
    instructions: "You are an Sales Assistant working in an Outdoor Sales Company.",
    tools: new List<ToolDefinition> { new FileSearchToolDefinition() },
    toolResources: new ToolResources() { FileSearch = fileSearchToolResource });
Agent agent = agentResponse.Value;

// Create thread for communication
Response<AgentThread> threadResponse = await client.CreateThreadAsync();
AgentThread thread = threadResponse.Value;

// user question
Response<ThreadMessage> userMessageResponse = await client.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    "I want to go camping, but it seems that it maybe raining, what do you suggest to go camping on rainy days?");
ThreadMessage userMessage = userMessageResponse.Value;

// agent task to answer the question
Response<ThreadMessage> agentMessageResponse = await client.CreateMessageAsync(
    thread.Id,
    MessageRole.Agent,
    "Please address the user as their name. The user has a basic account. Suggest products from the catalog when it's possible. Include any specific details of the suggested products like name, description and price.");
ThreadMessage agentMessage = agentMessageResponse.Value;

// run the agent thread
Response<ThreadRun> runResponse = await client.CreateRunAsync(
    thread.Id,
    assistantId: agent.Id
    );
ThreadRun run = runResponse.Value;

// wait for the response
do
{
    await Task.Delay(TimeSpan.FromMilliseconds(100));
    runResponse = await client.GetRunAsync(thread.Id, runResponse.Value.Id);
    Console.Clear();
    Console.WriteLine($"{DateTime.Now} Run status: {runResponse.Value.Status}");
}
while (runResponse.Value.Status == RunStatus.Queued
    || runResponse.Value.Status == RunStatus.InProgress);
Console.Clear();

// show the response
Response<PageableList<ThreadMessage>> afterRunMessagesResponse = await client.GetMessagesAsync(thread.Id);
IReadOnlyList<ThreadMessage> messages = afterRunMessagesResponse.Value.Data;

// sort the messages by creation date
messages = messages.OrderBy(m => m.CreatedAt).ToList();

// Note: messages iterate from newest to oldest, with the messages[0] being the most recent
foreach (ThreadMessage threadMessage in messages)
{
    Console.WriteLine($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
    foreach (MessageContent contentItem in threadMessage.ContentItems)
    {
        if (contentItem is MessageTextContent textItem)
        {
            Console.WriteLine(textItem.Text);

            // if there is any annotations, show them in the console
            if (textItem.Annotations != null && textItem.Annotations.Count > 0)
            {
                Console.WriteLine("");
                Console.WriteLine(">> Annotations:");
                foreach (MessageTextFileCitationAnnotation annotation in textItem.Annotations)
                {
                    string fileName = fileToId.FirstOrDefault(x => x.Value == annotation.FileId).Key;
                    Console.WriteLine($"\tFile Name [Id]: {fileName} [{annotation.FileId}]");
                    Console.WriteLine($"\tStart-End: {annotation.StartIndex}-{annotation.EndIndex}");
                    Console.WriteLine($"\tText: {annotation.Text}");
                }
            }
        }
        else if (contentItem is MessageImageFileContent imageFileItem)
        {
            Console.WriteLine($"<image from ID: {imageFileItem.FileId}");
        }
        Console.WriteLine();
    }
}
