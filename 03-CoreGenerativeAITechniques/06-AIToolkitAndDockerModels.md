# Running AI Models Locally with AI Toolkit and Docker

In this lesson, we'll explore how to run AI models locally using two popular approaches:
- **[AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview)** - A suite of tools for Visual Studio Code that enables running AI models locally
- **[Docker Model Runner](https://docs.docker.com/model-runner/)** - A containerized approach for running AI models with Docker

Running models locally provides several benefits:
- Data privacy - Your data never leaves your machine
- Cost efficiency - No usage charges for API calls
- Offline availability - Use AI even without internet connectivity
- Customization - Fine-tune models for specific use cases

## AI Toolkit for Visual Studio Code

The AI Toolkit for Visual Studio Code is a collection of tools and technologies that help you build and run AI applications locally on your PC. It leverages platform capabilities to optimize AI workloads.

### Key Features

- **DirectML** - Hardware-accelerated machine learning primitives
- **Windows AI Runtime (WinRT)** - Runtime environment for AI models
- **ONNX Runtime** - Cross-platform inference accelerator
- **Local model downloads** - Access to optimized models for Windows

### Getting Started

To get started with AI Toolkit for Visual Studio Code:

1. [Install the AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview)
2. Download a supported model
3. Use the APIs through .NET or other supported languages

> 📝 **Note**: AI Toolkit for Visual Studio Code requires Visual Studio Code and compatible hardware for optimal performance.

## Docker Model Runner

Docker Model Runner is a tool for running AI models in containers, making it easy to deploy and run inference workloads consistently across different environments.

### Key Features

- **Containerized models** - Package models with their dependencies
- **Cross-platform** - Run on Windows, macOS, and Linux
- **Built-in API** - RESTful API for model interaction
- **Resource management** - Control CPU and memory usage

### Getting Started

To get started with Docker Model Runner:

1. [Install Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. Pull a model image
3. Run the model container
4. Interact with the model through the API

```bash
# Pull and run a Llama model
docker run -d -p 12434:8080 \
  --name deepseek-model \
  --runtime=nvidia \
  ghcr.io/huggingface/dockerfiles/model-runner:latest \
  deepseek-ai/deepseek-llm-7b-chat
```

## Sample Code: Using AI Toolkit for Visual Studio Code with .NET

The AI Toolkit for Visual Studio Code provides a way to run AI models locally on your machine. We have two examples that demonstrate how to interact with AI Toolkit models using .NET:

### 1. Semantic Kernel with AI Toolkit

The [AIToolkit-01-SK-Chat](./src/AIToolkit-01-SK-Chat/) project shows how to use Semantic Kernel to chat with a model running via AI Toolkit for Visual Studio Code.

```csharp
// Example code demonstrating AI Toolkit for Visual Studio Code with Semantic Kernel integration
var builder = Kernel.CreateBuilder();
// Configure to use a locally installed model through AI Toolkit for Visual Studio Code
builder.AddAzureOpenAIChatCompletion(
    modelId: modelId,
    endpoint: new Uri(endpoint),
    apiKey: apiKey);
var kernel = builder.Build();
// Create a chat history and use it with the kernel
```

### 2. Microsoft Extensions for AI with AI Toolkit

The [AIToolkit-02-MEAI-Chat](./src/AIToolkit-02-MEAI-Chat/) project demonstrates how to use Microsoft Extensions for AI to interact with AI Toolkit for Visual Studio Code models.

```csharp
// Example code demonstrating AI Toolkit for Visual Studio Code with MEAI
OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(endpoint);
ApiKeyCredential credential = new ApiKeyCredential(apiKey);
// Create a chat client using local model through AI Toolkit for Visual Studio Code
ChatClient client = new OpenAIClient(credential, options).GetChatClient(modelId);
```

## Sample Code: Using Docker Models with .NET

In this repository, we have two examples that demonstrate how to interact with Docker-based models using .NET:

### 1. Semantic Kernel with Docker Models

The [DockerModels-01-SK-Chat](./src/DockerModels-01-SK-Chat/) project shows how to use Semantic Kernel to chat with a model running in Docker.

```csharp
var model = "ai/deepseek-r1-distill-llama";
var base_url = "http://localhost:12434/engines/llama.cpp/v1";
var api_key = "unused";

// Create a chat completion service
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(modelId: model, apiKey: api_key, endpoint: new Uri(base_url));
var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. Always reply in a funny way with short answers.");

// ... continue with chat functionality
```

### 2. Microsoft Extensions for AI with Docker Models

The [DockerModels-02-MEAI-Chat](./src/DockerModels-02-MEAI-Chat/) project demonstrates how to use Microsoft Extensions for AI to interact with Docker-based models.

```csharp
var model = "ai/deepseek-r1-distill-llama";
var base_url = "http://localhost:12434/engines/llama.cpp/v1";
var api_key = "unused";

OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(base_url);
ApiKeyCredential credential = new ApiKeyCredential(api_key);

ChatClient client = new OpenAIClient(credential, options).GetChatClient(model);

// Build and send a prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews...");
// ... add more text to the prompt

var response = await client.CompleteChatAsync(prompt.ToString());
Console.WriteLine(response.Value.Content[0].Text);
```

## Running the Samples

To run the samples in this repository:

1. Install Docker Desktop and start it
2. Pull the required model:
   ```bash
   docker run -d -p 12434:8080 \
     --name deepseek-model \
     ghcr.io/huggingface/dockerfiles/model-runner:latest \
     deepseek-ai/deepseek-llm-7b-chat
   ```
3. Navigate to one of the sample project directories
4. Run the project with `dotnet run`

## Comparing AI Toolkit and Docker Approach

| Feature | AI Toolkit for Visual Studio Code | Docker Model Runner |
|---------|-------------------------------|---------------------|
| Platform | Windows, macOS, Linux | Cross-platform |
| Integration | Visual Studio Code APIs | REST API |
| Deployment | VS Code Extension | Container-based |
| Hardware Acceleration | DirectML, DirectX | CPU, GPU |
| Models | Optimized for VS Code | Any containerized model |

## Additional Resources

- [AI Toolkit for Visual Studio Code Documentation](https://code.visualstudio.com/docs/intelligentapps/overview)
- [Docker Model Runner Documentation](https://docs.docker.com/model-runner/)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/overview/)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/dotnet/ai/)

## Summary

Running AI models locally with AI Toolkit for Visual Studio Code or Docker Model Runner offers flexibility, privacy, and cost benefits. The samples in this repository demonstrate how to integrate these local models with your .NET applications using Semantic Kernel and Microsoft Extensions for AI.

## Next Steps

You've learned how to run AI models locally using AI Toolkit for Visual Studio Code and Docker Model Runner. Next, you'll explore how to create AI agents that can perform tasks autonomously.

👉 [Check out AI Agents](./04-agents.md)
