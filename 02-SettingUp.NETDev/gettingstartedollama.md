# 🚀 Getting Started in 5 Minutes: Run Your First AI App with .NET with Ollama

Welcome to the **Generative AI for Beginners - .NET** ([GenAINET]) repo! 🎉 Let’s get your first .NET AI application up and running in no time. Follow along as we dive into a step-by-step guide to build, tweak, and enhance your AI app. Let’s go!

## 🏁 Run Your First .NET AI App in 4 Easy Steps 

### Step 1: Create a GitHub Codespace that includes ollama

1. Open the repo in GitHub.

1. Click the **Code** button, select **Codespaces**, select **...** and select **New with options**

![Create codespace with options](./images/firststepsollama-10-createcodespace.png)

1. From the New Codespace options, select the "**C# .NET - Ollama**" definition.

![Create codespace with options](./images/firststepsollama-15-ollamadefinition.png)

1. This process will take a couple of minutes, wait until the codespace is fully created.

[Learn more about GitHub Codespaces](https://docs.github.com/en/codespaces/getting-started).

### Step 2: Navigate to the Basic Chat Sample

Once the Codespace is ready:

```bash
cd 02-SettingUp.NETDev/src/BasicChat-03Ollama/
```

### Step 3: Run the Chat App

Run the following command in the terminal:

```bash
dotnet run
```

🎉 **Done!** You’ve just launched your first AI application. Here’s what it looks like:

![1st .NET AI chat application running in Codespaces](./images/firststepsollama-20-apprun.png)

---

## 🔄 Let’s Add Streaming Capabilities to Your Chat App

Your app might take a while to answer questions. Let’s make it cooler by enabling streaming, so you see responses as they’re generated!

### Step 1: Edit the Program.cs File

Go to the file `/workspaces/Generative-AI-for-beginners-dotnet/02-SettingUp.NETDev/src/BasicChat-01MEAI/Program.cs`

### Step 2: Replace the Code (Lines 10 to End)

Replace everything from line 10 onward with:

```csharp
var response = client.CompleteStreamingAsync("What is AI?");
await foreach (var item in response)
{
    Console.Write(item);
}
```

### Step 3: Run the Chat App

Run the following command:

```bash
dotnet run
```

🎉 **Done!** Now your app streams the response live as it’s being generated. Here’s what it looks like:

![.NET AI chat application using streaming mode](./images/firsttesps-15-apprunstreaming.gif)

---

## 🧠 Test Another Model in Your App

The current app uses the "**Phi-3.5-MoE-instruct**" model. Let’s switch it up and try the "**Mistral-Nemo**" model instead!

### Step 1: Update the Model in Code

Edit the main code to update the model name:

```csharp
IChatClient client = new ChatCompletionsClient(
    endpoint: new Uri("https://models.inference.ai.azure.com"),
    new AzureKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")))
    .AsChatClient("Mistral-Nemo");
```

### Step 2: Run the Chat App

Run the following command:
```bash
dotnet run
```
🎉 **Done!** You’ve just switched to a new model. Notice how the response is longer and more detailed. Here’s what it looks like:

![1st .NET AI chat application using Mistral model](./images/firsttesps-20-useMistralModel.png)

---

## 📖 References and Resources

- [Working with GitHub Codespaces](https://docs.github.com/en/codespaces/getting-started)
- [How to Generate a Personal Access Token on GitHub](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [Microsoft Extensions for AI Documentation](https://learn.microsoft.com/en-us/dotnet/)
