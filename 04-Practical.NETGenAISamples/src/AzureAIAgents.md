# Azure AI Agents

## Program.cs Functionality

The `Program.cs` file demonstrates how to use the Azure AI Projects SDK to create an AI agent that can assist with sales inquiries. Below is a detailed explanation of the functionality implemented in the file.

### Prerequisites
- .NET 9
- Azure AI Foundry Project connection string
- Azure tenant ID

This project requires the following user secrets
- Azure AI Foundry Project connection string
  At the moment, it should be in the format "<HostName>;<AzureSubscriptionId>;<ResourceGroup>;<ProjectName>"
 - Azure tenant id

Set them using the following commands:

```bash
dotnet user-secrets init
dotnet user-secrets set "connectionstring" "<HostName>;<AzureSubscriptionId>;<ResourceGroup>;<ProjectName>"
dotnet user-secrets set "tenantid" "<AzureSubscriptionTenantId>"
``bash

### Steps

1. **Configuration and Secrets Setup**
   - Read user secrets
     
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

flowchart TD
    A[Start] --> B[Create Agent Client]
    B --> C[Upload Files and Get File IDs]
    C --> D[Create Vector Store with Files]
    D --> E[Create FileSearchToolResource]
    E --> F[Create Agent with Tool Resources]
    F --> G[Create Thread for Communication]
    G --> H[User Question]
    H --> I[Agent Task to Answer Question]
    I --> J[Run the Agent Thread]
    J --> K[Wait for Response]
    K --> L[Show the Response]
    L --> M[Sort Messages by Creation Date]
    M --> N[Display Messages and Annotations]
    N --> O[End]

### References
For more detailed information, refer to the [Azure AI Projects Documentation](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.projects-readme?context=%2Fazure%2Fai-services%2Fagents%2Fcontext%2Fcontext&view=azure-dotnet-preview).


