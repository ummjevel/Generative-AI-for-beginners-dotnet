<div align="center">
    <h1>Generative AI Fundamentals for .NET</h1>
    <h2>Lesson 4: Generative AI Samples</h2>
    <p><em>Get inspired to create with MSFT's Generative AI Samples</em></p>
</div>

> 💡 **Quick Summary**: Understand how Generative AI works with Microsoft Samples, get inspirated, and understand the best scenarios to apply it into new applications.

---

**What you'll achieve:**
- Understand how Agents work in complex scenarios for Generative AI, both using Semantic Kernel and GitHub Models.
- See the best scenarios to apply Generative AI in new applications.


**Index**

1. [eShopLite Demos](#intro-video)
1. [Chat with your Data](#understanding-github-codespaces)
1. [Creative Writer](#pre-flight-check-setting-up-github-access-tokens)
1. [Conclusions and Resources](#conclusions-and-resources)

---

## Intro Video

[![Watch the video](../images/04-videocover.jpg)](https://microsoft-my.sharepoint.com/:v:/p/brunocapuano/ERHGiBsTtVJBtN9VevaxdnwB6dfV_GCdFXbZL9D-GnEkew?e=Y4oAjD&nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJTdHJlYW1XZWJBcHAiLCJyZWZlcnJhbFZpZXciOiJTaGFyZURpYWxvZy1MaW5rIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXcifX0%3D)

In our first video, Bruno Capuano introduces some of the practical scenarios for .NET Generative AI! 
On the first demo, we use Ollama to create an application to generate descriptions for images quickly using Llama 3.2 Vision model running locally, later, four demos that showcase the power of Semantic Kernel, Language Models, and the .NET AI Extension.


## eShopLite Demos

![A screenshot of eShopLite](./images/eShopLite-site.png)

For our first two demos, we'll explore the eShopLite project, a simple e-commerce application for outdoor gear and camping enthusiasts that is augmented with Generative AI capabilities, such as search features optimization, Customer Support, and Realtime Audio Analysis. 

The first demo, we show how to use the Semantic Kernel to enhance the search capabilities, which can understand the context of the user's queries and provide accurate results. 

### eShopLite with Semantic Search

![Video thumbnail for eShopLite with Semantic Search](./images/eshoplite-semantic-search.png)

In eShopLite with Semantic Search, we use the Semantic Kernel to enhance the search capabilities of the e-commerce application. Semantic Kernel auxiliate us to create a more robust search engine that can understand the context of the user's queries and provide more accurate results. For example, if a user searches for "do you have something for cooking", the search engine can understand that the user is looking for kitchenware and show the most relevant products, in context of our sample, it returns Camping Cookware. 

![Image demonstrating the search capabilities in eShopLite](./images/search-eshoplite.png)

Semantic Search can help users find the products they need more easily, leading to a better shopping experience and increased sales, to implement this feature, we need to have a vector store with the products, a search index, a language model, and [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) to coordinate all the processes in the backend.

![Image demonstrating the .NET Aspire Dashboard](./images/aspire-dashboard.png)

In the Dashboard, we can see the Products, sql, and store containers, which can interact with the language model. Looking deeper into the Aspire App Host, we have the following:

```csharp
if (builder.ExecutionContext.IsPublishMode)
{
    // Add the Azure Application Insights for monitoring
    var appInsights = builder.AddAzureApplicationInsights("appInsights");
    // Add the Azure OpenAI for the chat and embeddings deployments, the embedding is used for the vector entities
    var chatDeploymentName = "gpt-4o-mini";
    var embeddingsDeploymentName = "text-embedding-ada-002";
    var aoai = builder.AddAzureOpenAI("openai")
        .AddDeployment(new AzureOpenAIDeployment(chatDeploymentName,
        "gpt-4o-mini",
        "2024-07-18",
        "GlobalStandard",
        10))
        .AddDeployment(new AzureOpenAIDeployment(embeddingsDeploymentName,
        "text-embedding-ada-002",
        "2"));

    products.WithReference(appInsights)
        .WithReference(aoai)
        .WithEnvironment("AI_ChatDeploymentName", chatDeploymentName)
        .WithEnvironment("AI_embeddingsDeploymentName", embeddingsDeploymentName);

    store.WithReference(appInsights)
        .WithExternalHttpEndpoints();
}
```

The code above demonstrates how to add the Azure Application Insights for monitoring, the Azure OpenAI for the chat and embeddings deployments, and the embedding used for the vector entities. For embedding and AOAI creation, it can be found at the product container, as follows:

```csharp
var azureOpenAiClientName = "openai";
builder.AddAzureOpenAIClient(azureOpenAiClientName);

// get azure openai client and create Chat client from aspire hosting configuration
builder.Services.AddSingleton<ChatClient>(serviceProvider =>
{
    var chatDeploymentName = "gpt-4o-mini";
    var logger = serviceProvider.GetService<ILogger<Program>>()!;
    logger.LogInformation($"Chat client configuration, modelId: {chatDeploymentName}");
    ChatClient chatClient = null;
    try
    {
        OpenAIClient client = serviceProvider.GetRequiredService<OpenAIClient>();
        chatClient = client.GetChatClient(chatDeploymentName);
    }...
}
```
The code above demonstrates how to get the Azure OpenAI client and create the Chat client from the Aspire hosting configuration. The `chatDeploymentName` is the name of the deployment used in the application. The same process is used to create the Embedding client, as follows:

```csharp
// get azure openai client and create embedding client from aspire hosting configuration
builder.Services.AddSingleton<EmbeddingClient>(serviceProvider =>
{
    var embeddingsDeploymentName = "text-embedding-ada-002";
    var logger = serviceProvider.GetService<ILogger<Program>>()!;
    logger.LogInformation($"Embeddings client configuration, modelId: {embeddingsDeploymentName}");
    EmbeddingClient embeddingsClient = null;
    try
    {
        OpenAIClient client = serviceProvider.GetRequiredService<OpenAIClient>();
        embeddingsClient = client.GetEmbeddingClient(embeddingsDeploymentName);
    }...
});
```	

With it we can create the MemoryContext, as our vector store to compare to the user's query, and return the most relevant products, as follows:

```csharp
// Iterate over the products and add them to the memory
_logger.LogInformation("Adding product to memory: {Product}", product.Name);
var productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";

// Create a new product vector
var productVector = new ProductVector
{
    Id = product.Id,
    Name = product.Name,
    Description = product.Description,
    Price = product.Price,
    ImageUrl = product.ImageUrl
};

// Generate the embedding for the product information
var result = await _embeddingClient.GenerateEmbeddingAsync(productInfo);

// Convert the embedding result to a float array and assign it to the product vector
productVector.Vector = result.Value.ToFloats();
var recordId = await _productsCollection.UpsertAsync(productVector);
_logger.LogInformation("Product added to memory: {Product} with recordId: {RecordId}", product.Name, recordId);
``` 

The code above demonstrates how to iterate over the products and add them to the memory, create a new product vector, generate the embedding for the product information, convert the embedding result to a float array, and assign it to the product vector. The product is then added to the memory, repeating the process for each product in the collection. After that, when the user searches for a product, we can compare the user's query with the product vectors and return the most relevant products.

```csharp
try
{
    // Generate embedding for the search query
    var result = await _embeddingClient.GenerateEmbeddingAsync(search);
    var vectorSearchQuery = result.Value.ToFloats();

    var searchOptions = new VectorSearchOptions()
    {
        Top = 1, // Retrieve the top 1 result
        VectorPropertyName = "Vector"
    };

    // Search the vector database for the most similar product
    var searchResults = await _productsCollection.VectorizedSearchAsync(vectorSearchQuery, searchOptions);
    double searchScore = 0.0;
    await foreach (var searchItem in searchResults.Results)
    {
        if (searchItem.Score > 0.5)
        {
            // Product found, retrieve the product details
            firstProduct = new Product
            {
                Id = searchItem.Record.Id,
                Name = searchItem.Record.Name,
                Description = searchItem.Record.Description,
                Price = searchItem.Record.Price,
                ImageUrl = searchItem.Record.ImageUrl
            };

            searchScore = searchItem.Score.Value;
            responseText = $"The product [{firstProduct.Name}] fits with the search criteria [{search}][{searchItem.Score.Value.ToString("0.00")}]";
            _logger.LogInformation($"Search Response: {responseText}");
        }
    }

    // Generate a friendly response message using the found product information
    var prompt = @$"You are an intelligent assistant helping clients with their search about outdoor products. Generate a catchy and friendly message using the following information:
    - User Question: {search}
    - Found Product Name: {firstProduct.Name}
    - Found Product Description: {firstProduct.Description}
    - Found Product Price: {firstProduct.Price}
    Include the found product information in the response to the user question.";

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(_systemPrompt),
        new UserChatMessage(prompt)
    };

    _logger.LogInformation("{ChatHistory}", JsonConvert.SerializeObject(messages));

    var resultPrompt = await _chatClient.CompleteChatAsync(messages);
}
```

With the code above, we  generate the embedding for the search query, search the vector database for the most similar product, and get a response message using the found product information. Help the user find the products they need more easily, leading to a better shopping experience and increased sales. Moreover, as generative AI evolves, we need some telemetry and monitoring to understand the user's behavior and improve the search engine, this is where Azure Application Insights and .NET Aspire come in.

![Image demonstrating the .NET Aspire tracing capabilities](./images/aspire-tracing-eshoplite.png)

.NET Aspire provides a powerful set of tools to monitor and trace the application's behavior, including the user's interactions with the search engine, backend services, and the AI models. The tracing capabilities can help us understand possible bottlenecks, errors, and performance issues, allowing us to optimize the application and provide a better user experience.

![Image demonstrating the Azure Application Insights in eShopLite](./images/app-insights-eshoplite.png)

Application Insights provides a comprehensive set of telemetry data, helping us to understand how our services are performing, and how users are interacting with the application and cloud usage.

> 💡 **Pro Tip**: For more information on eShopLite with Semantic Search, look at the repository to learn more: https://github.com/Azure-Samples/eShopLite-SemanticSearch/

### eShopLite with Realtime Analysis

![Video thumbnail for eShopLite with Realtime Analysis](./images/eshoplite-realtime.png)

In eShopLite with Realtime Analysis, we use the Realtime audio capabilities of GPT-4o to analyze the conversations between the customer and the chatbot, providing a more personalized and engaging experience. For example, if a customer asks for a product recommendation, the chatbot can analyze the customer's request in real-time and provide a more accurate and relevant response.

![Image demonstrating the Realtime Analysis in eShopLite](./images/realtime-analysis-eshoplite.gif)

To implement this feature, we need to implement new features to create the endpoints for the Realtime Analysis, it can be found on the `StoreRealtime\ConversationManager.cs` implementation.


```csharp
public async Task RunAsync(
    Stream audioInput, 
    Speaker audioOutput, 
    Func<string, Task> addMessageAsync, 
    Func<string, bool, Task> addChatMessageAsync, 
    CancellationToken cancellationToken)
{
    // Define the initial prompt for the assistant
    var prompt = $"""
        You are a useful assistant.
        Respond as succinctly as possible, in just a few words.
        Check the product database and external sources for information.
        The current date is {DateTime.Now.ToLongDateString()}
        """;

    // Notify the user that the connection is being established
    await addMessageAsync("Connecting...");
    // Send an initial greeting message
    await addChatMessageAsync("Hello, how can I help?", false);

    // Create AI functions for semantic search and product name search
    var contosoSemanticSearchTool = AIFunctionFactory.Create(_contosoProductContext.SemanticSearchOutdoorProductsAsync);
    var contosoSearchByProductNameTool = AIFunctionFactory.Create(_contosoProductContext.SearchOutdoorProductsByNameAsync);

    // Add the AI functions to a list of tools
    List<AIFunction> tools = new List<AIFunction> { contosoSemanticSearchTool, contosoSearchByProductNameTool };

    // Configure the conversation session options
    var sessionOptions = new ConversationSessionOptions()
    {
        Instructions = prompt,
        Voice = ConversationVoice.Shimmer,
        InputTranscriptionOptions = new() { Model = "whisper-1" },
    };

    // Add each tool to the session options
    foreach (var tool in tools)
    {
        sessionOptions.Tools.Add(tool.ToConversationFunctionTool());
    }

    // Start the conversation session with the configured options
    session = await client.StartConversationSessionAsync(cancellationToken);
    await session.ConfigureSessionAsync(sessionOptions);

    // Initialize a StringBuilder to store the output transcription
    var outputTranscription = new StringBuilder();
}
```

>  💡 **Pro Tip**: For more information on eShopLite with Realtime Audio, look at the repository to learn more: https://github.com/Azure-Samples/eShopLite-RealtimeAudio
---

## Chat with your Data
### Coming Soon
## Creative Writer Agent
### Coming Soon

## Conclusions and resources

Those are just a few examples of how you can use Generative AI in your applications. The possibilities are endless, and the technology is evolving rapidly, look at some of our resources to learn more about Generative AI and how you can use it in your projects.

### Additional Resources

> ⚠️ **Note**: If you encounter any issues, please, open an issue in the repository.

- [Azure AI Agents - Instructions](./src/AzureAIAgentsmd)
- [Azure AI Agents - File Search](./src/AzureAIAgents-04-FileSearch.md)

### Next Steps

Learn about responsible AI practices and how to ensure that your AI models are ethical and have a positive impact!

<p align="center">
    <a href="../05-ResponsibleGenAI/readme.md">Go to Chapter 5</a>
</p>

