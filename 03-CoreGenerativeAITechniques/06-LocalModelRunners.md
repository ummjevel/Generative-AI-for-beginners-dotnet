# Running AI Models Locally: AI Toolkit, Docker, and Foundry Local

In this lesson, you'll learn how to run AI models locally using three popular approaches:
- **[AI Toolkit for Windows](https://learn.microsoft.com/windows/ai/toolkit/)** – A suite of tools for Windows that enables running AI models locally
- **[Docker Model Runner](https://docs.docker.com/model-runner/)** – A containerized approach for running AI models with Docker
- **[Foundry Local](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-local/)** – A cross-platform, open-source solution for running Microsoft AI models locally

Running models locally provides several benefits:
- Data privacy – Your data never leaves your machine
- Cost efficiency – No usage charges for API calls
- Offline availability – Use AI even without internet connectivity
- Customization – Fine-tune models for specific use cases

## AI Toolkit for Windows

The AI Toolkit for Windows is a collection of tools and technologies that help you build and run AI applications locally on Windows PCs. It leverages the Windows platform capabilities to optimize AI workloads.

### Key Features
- **DirectML** – Hardware-accelerated machine learning primitives
- **Windows AI Runtime (WinRT)** – Runtime environment for AI models
- **ONNX Runtime** – Cross-platform inference accelerator
- **Local model downloads** – Access to optimized models for Windows

### Getting Started
1. [Install the AI Toolkit](https://learn.microsoft.com/windows/ai/toolkit/install)
2. Download a supported model
3. Use the APIs through .NET or other supported languages

> 📝 **Note**: AI Toolkit for Windows requires Windows 10/11 and compatible hardware for optimal performance.

## Docker Model Runner

Docker Model Runner is a tool for running AI models in containers, making it easy to deploy and run inference workloads consistently across different environments.

### Key Features

- **Containerized models** – Package models with their dependencies
- **Cross-platform** – Run on Windows, macOS, and Linux
- **Built-in API** – RESTful API for model interaction
- **Resource management** – Control CPU and memory usage

### Getting Started

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

## Foundry Local

[Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/) is an open-source, cross-platform solution for running Microsoft AI models on your own hardware. It supports Windows, Linux, and macOS, and is designed for privacy, performance, and flexibility.

- **Official documentation:** https://learn.microsoft.com/azure/ai-foundry/foundry-local/
- **GitHub repository:** https://github.com/microsoft/Foundry-Local/tree/main

### Key Features

- **Cross-platform** – Windows, Linux, and macOS
- **Microsoft models** – Run models from Azure AI Foundry locally
- **REST API** – Interact with models using a local API endpoint
- **No cloud dependency** – All inference runs on your machine

### Getting Started

1. [Read the official Foundry Local documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
2. Download and install Foundry Local for your OS
3. Start the Foundry Local server and download a model
4. Use the REST API to interact with the model

## Sample Code: Using AI Toolkit for Windows with .NET

The AI Toolkit for Windows provides a way to run AI models locally on Windows machines. We have two examples that demonstrate how to interact with AI Toolkit models using .NET:

### 1. Semantic Kernel with AI Toolkit

The [AIToolkit-01-SK-Chat](./src/AIToolkit-01-SK-Chat/) project shows how to use Semantic Kernel to chat with a model running via AI Toolkit for Windows.

```csharp
// Example code demonstrating AI Toolkit with Semantic Kernel integration
var builder = Kernel.CreateBuilder();
// Configure to use a locally installed model through AI Toolkit
builder.AddAzureOpenAIChatCompletion(
    modelId: modelId,
    endpoint: new Uri(endpoint),
    apiKey: apiKey);
var kernel = builder.Build();
// Create a chat history and use it with the kernel
```

### 2. Microsoft Extensions for AI with AI Toolkit

The [AIToolkit-02-MEAI-Chat](./src/AIToolkit-02-MEAI-Chat/) project demonstrates how to use Microsoft Extensions for AI to interact with AI Toolkit models.

```csharp
// Example code demonstrating AI Toolkit with MEAI
OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(endpoint);
ApiKeyCredential credential = new ApiKeyCredential(apiKey);
// Create a chat client using local model through AI Toolkit
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

## Sample Code: Using Foundry Local with .NET

This repository includes two demos for Foundry Local:

### 1. Semantic Kernel with Foundry Local

The [AIFoundryLocal-01-SK-Chat](./src/AIFoundryLocal-01-SK-Chat/Program.cs) project shows how to use Semantic Kernel to chat with a model running via Foundry Local.

### 2. Microsoft Extensions for AI with Foundry Local

The [AIFoundryLocal-01-MEAI-Chat](./src/AIFoundryLocal-01-MEAI-Chat/Program.cs) project demonstrates how to use Microsoft Extensions for AI to interact with Foundry Local models.

## Running the Samples

To run the samples in this repository:

1. Install Docker Desktop, AI Toolkit for Windows, or Foundry Local as needed
2. Pull or download the required model
3. Start the local model server (Docker, AI Toolkit, or Foundry Local)
4. Navigate to one of the sample project directories
5. Run the project with `dotnet run`

## Comparing Local Model Runners

| Feature | AI Toolkit for Windows | Docker Model Runner | Foundry Local |
|---------|------------------------|---------------------|--------------|
| Platform | Windows only | Cross-platform | Cross-platform |
| Integration | Native Windows APIs | REST API | REST API |
| Deployment | Local installation | Container-based | Local installation |
| Hardware Acceleration | DirectML, DirectX | CPU, GPU | CPU, GPU |
| Models | Optimized for Windows | Any containerized model | Microsoft Foundry models |

## Additional Resources

- [AI Toolkit for Windows Documentation](https://learn.microsoft.com/windows/ai/toolkit/)
- [Docker Model Runner Documentation](https://docs.docker.com/model-runner/)
- [Foundry Local Documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
- [Foundry Local GitHub Repository](https://github.com/microsoft/Foundry-Local/tree/main)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/overview/)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/dotnet/ai/)

## Summary

Running AI models locally with AI Toolkit for Windows, Docker Model Runner, or Foundry Local offers flexibility, privacy, and cost benefits. The samples in this repository demonstrate how to integrate these local models with your .NET applications using Semantic Kernel and Microsoft Extensions for AI.

## Next Steps

You've learned how to run AI models locally using AI Toolkit for Windows, Docker Model Runner, and Foundry Local. Next, you'll explore how to create AI agents that can perform tasks autonomously.

👉 [Check out AI Agents](./04-agents.md)
