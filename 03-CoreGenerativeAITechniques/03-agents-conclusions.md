<div align="center">
    <h1>Generative AI Fundamentals for .NET</h1>
    <h2>Lesson 3.3: Agents and Conclusions </h2>
    <p><em>Learn how to create Agentic AI in .NET!</em></p>
</div>

---
## Agents / Assistants
![Agents / Assistants](../03-CoreGenerativeAITechniques/images/Agents-Assistants.png)

Agents are AI applications which have some autonomy, and can perform tasks following workflows using certain LLM archtectures. Usually, agents need plugins to perform tasks, native functions, and optimized logic to invoke functions in the right order.

In our demos, we have the following structure:

- `Agent Client`: The client that hosts the agent, configuring the connection to the Cloud, AI models and be the base for the agent.
- `Agent`: The agent itself, with the logic to perform the tasks, using the plugins and functions to perform the tasks. Agents can be shaped as needed to solve and perform tasks.
- `Tools`: Tools are the plugins and functions that the agent uses to perform the tasks. They can be used to perform tasks, like getting the weather, sending emails, or even controlling IoT devices.

In the `src/Agents-01-Simple` we can see how to create a simple agent that gets helps in simple math, look into the Agent Client code:

```csharp
// Configure the connection to the Cloud, AI models and be the base for the agent
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var options = new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeWorkloadIdentityCredential = true,
    TenantId = config["tenantid"]
};
var connectionString = config["connectionString"];

// Create the agent client with the connection string and the Azure credentials
AgentsClient client = new AgentsClient(connectionString, new AgentsClient client = new AgentsClient(connectionString, new DefaultAzureCredential(options));
```

For the agent, we can see how to create a simple agent that gets helps in simple math, look into the Agent code:

```csharp
// Create Agent with the model, name, instructions and tools, to do that we use the Agent Client
Response<Agent> agentResponse = await client.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "Math Tutor",
    instructions: "You are a personal math tutor. Write and run code to answer math questions.",
    tools: [new CodeInterpreterToolDefinition()]);
Agent agentMathTutor = agentResponse.Value;
// Use the Agent to create a thread and start the conversation with the Client
Response<AgentThread> threadResponse = await client.CreateThreadAsync();
AgentThread thread = threadResponse.Value;

```	

Look how the Agent Client and the Agent are interconnected, the Agent Client is the base for the Agent, and the Agent is the one that performs the tasks.

For the tools, we can see how to create a tool for a travel agency, look into the `src/Agents-02-TravelAgency` Agent:

```csharp
// create Agent
Response<Agent> agentResponse = await client.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "SDK Test Agent - Vacation",
        instructions: @"You are a travel assistant. Use the provided functions to help answer questions. 
Customize your responses to the user's preferences as much as possible. Write and run code to answer user questions.",
    // Add the tools to the agent, tools are the plugins and functions that the agent uses to perform the tasks. 
    tools: new List<ToolDefinition> {        
        CityInfo.getUserFavoriteCityTool,
        CityInfo.getWeatherAtLocationTool,
        CityInfo.getParksAtLocationTool}
    );
Agent agentTravelAssistant = agentResponse.Value;
Response<AgentThread> threadResponse = await client.CreateThreadAsync();
AgentThread thread = threadResponse.Value;
```

The tools are the plugins and functions that the agent uses to perform the tasks. In this case, the agent uses the CityInfo tools to get information about cities. Know more about `CityInfo` tools in the `src/Agents-02-TravelAgency` sample.


## Conclusions and resources

In this chapter, we explored the core generative AI techniques, including Language Model Completions, RAG, Vision and Audio applications. 

In the next chapter, we will explore how to implement these techniques in practice, using real-world examples and complex samples.

### Additional Resources

> ⚠️ **Note**: If you encounter any issues, open an issue in the repository.

- [GitHub Codespaces Documentation](https://docs.github.com/en/codespaces)
- [GitHub Models Documentation](https://docs.github.com/en/github-models/prototyping-with-ai-models)
- [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/get-started/quick-start-guide?pivots=programming-language-csharp)
- [MEAI Documentation](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)

### Next Steps

Next, we'll explore some samples in how to implement these algoritms pratically. 

<p align="center">
    <a href="../04-Practical.NETGenAISamples/readme.md">Go to Chapter 4</a>
</p>