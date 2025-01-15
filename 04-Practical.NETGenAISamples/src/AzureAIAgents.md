# Azure AI Agents

## Program.cs Functionality

The `Program.cs` file demonstrates how to use the Azure AI Projects SDK to create an AI agent that can assist with sales inquiries. Below is a detailed explanation of the functionality implemented in the file.

### Prerequisites
- .NET 9
- Azure AI Foundry Project connection string
- Azure tenant ID

### Steps

1. **Configuration and Secrets Setup**
   - The project requires user secrets for the Azure AI Foundry Project connection string and Azure tenant ID.
   - Initialize user secrets and set the required secrets using the following commands:
     
2. **Create Agent Client**
   - Build the configuration and set up the `DefaultAzureCredentialOptions` with the tenant ID.
   - Create an `AgentsClient` using the connection string and credentials.

3. **Upload Files and Get File IDs**
   - Enumerate files in the "products" directory.
   - Upload each file asynchronously and store the file IDs in a `ConcurrentBag`.

4. **Create Vector Store**
   - Create a vector store with the uploaded file IDs and wait for it to be processed.

5. **Create Agent**
   - Define the agent model, name, instructions, tools, and tool resources.
   - Create the agent using the `AgentsClient`.

6. **Create Communication Thread**
   - Create a thread for communication with the agent.

7. **User Question and Agent Response**
   - Send a user question to the agent.
   - Define the agent's task to answer the question, including addressing the user by name and suggesting products from the catalog.

8. **Run the Agent Thread**
   - Run the agent thread and wait for the response.
   - Display the response messages, including any annotations and images.

### Main Flow Layout

### References
For more detailed information, refer to the [Azure AI Projects Documentation](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.projects-readme?context=%2Fazure%2Fai-services%2Fagents%2Fcontext%2Fcontext&view=azure-dotnet-preview).


