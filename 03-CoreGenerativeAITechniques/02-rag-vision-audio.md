<div align="center">
    <h1>Generative AI Fundamentals for .NET</h1>
    <h2>Lesson 3.2: RAG, Video and Audio Applications </h2>
    <p><em>Apply AI knowledge into data intense domains</em></p>
</div>

---

## RAG - Retrieved-Augmented Generation

(5 min video .NET explanation, links to main GenAI and content)

![Retrieved-Augmented Generation](../03-CoreGenerativeAITechniques/images/RAG.png)

**RAG** is an architecture that combines a retriever and a language model to create a more powerful applcation. The retriever is used to find relevant information from a large vector dataset, and the language model is used to generate the response, augmenting the previous prompt with the retrieved information.

Some situations where RAG can be used are where we need to generate a response based on a large amount of data, or where we need to generate a response based on a specific context. 

*Remember*, Language Models have a *knowledge cutoff date* and may not have the most recent information. Plus, they may need some context to generate a more accurate response.

Look at the following infographic from [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners/tree/main/15-rag-and-vector-databases) to understand how RAG works:

![RAG](../03-CoreGenerativeAITechniques/images/how-rag-works.png)

As we can see, the **RAG model** gets our question from the prompt and retrieves the most relevant information from the database. Then, augments the prompt with the retrieved information, sending it to the language model to generate the response.

For our .NET applications, we can use **Semantic Kernel** and **MEAI** to implement RAG.

First, we need the information which will be injected to be retrieved, for example, a movie database. 

In **Semantic Kernel**, we can use the following code snippet to get some data from Memory:

```csharp
// add facts to the collection
Dictionary<string, string> memoryInformation = new()
{
    {"1", "Gisela's favourite super hero is Batman" },
    {"2", "The last super hero movie watched by Gisela was Guardians of the Galaxy Vol 3" },
    {"3", "Bruno's favourite super hero is Invincible" },
    {"4", "The last super hero movie watched by Bruno was Deadpool and Wolverine" },
    {"5", "Bruno does not like the super hero movie: Eternals" }
};

const string MemoryCollectionName = "fanFacts";
foreach (var information in memoryInformation)
{
    await memory.SaveInformationAsync(MemoryCollectionName,
        id: information.Key,
        text: information.Value);
}

TextMemoryPlugin memoryPlugin = new(memory);
```

Above, we are using the `TextMemoryPlugin` to save the information in the memory, and then we can use it to retrieve the information in the conversation. To retrieve the information, we can use the following code snippet:

```csharp
// Restructuring the conversation to use the question, agumented with the memory content
var prompt = @"Question: {{$input}}
    Answer the question using the memory content: {{Recall}}";

// Add the user question to the conversation history
history.AddUserMessage(prompt);

// Pass the question to the kernel, along with the collection name and the conversation history
var arguments = new KernelArguments(settings)
{
    { "input", question },
    { "collection", MemoryCollectionName },
    { "messages", history }
};

// Get the response from the kernel
var newResponse = kernel.InvokePromptStreamingAsync<StreamingChatMessageContent>(prompt, arguments);
```

Semantic Kernel provides a way to create a plugin that can be used to retrieve information from a Dictionary, this plugin can be used to retrieve information from a database, a file, or any other source of information.

In `src/MEAIVectorsShared` we can see how to create a mock vector database using MEAI. 

```csharp
public static List<Movie<T>> GetMovieList()
    {
        var movieData = new List<Movie<T>>()
        {
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "Lion King",
                Year = 1994,
                Category = "Animation, Family, Adventure",
                Description = "The Lion King (1994) is a classic Disney animated film that tells the story of a young lion named Simba who embarks on a journey to reclaim his throne as the king of the Pride Lands after the tragic death of his father."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "Inception",
                Year = 2010,
                Category = "Science Fiction, Action, Thriller",
                Description = "Inception (2010) is a science fiction film directed by Christopher Nolan that follows a group of thieves who enter the dreams of their targets to steal information."
            },

...
```

 The following code snippet shows how to create a RAG model using **MEAI**:

```csharp
// Tries to the movie data from the database
var movies = vectorStore.GetCollection<int, MovieVector<int>>("movies");
// Create the collection if it doesn't exist
await movies.CreateCollectionIfNotExistsAsync();
// Get the movie data from the database
var movieData = MovieFactory<int>.GetMovieVectorList();

// Get embeddings generator in this case Ollama
IEmbeddingGenerator<string, Embedding<float>> generator =
    new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm");

// Generate embeddings for movies
foreach (var movie in movieData)
{
    movie.Vector = await generator.GenerateEmbeddingVectorAsync(movie.Description);
    await movies.UpsertAsync(movie);
}
```

To retrieve the information from the database, we can use the following code snippet:

```csharp
// Get the movie data, generate the embeddings for the query, and search for the most relevant movies
var query = "A family friendly movie that includes ogres and dragons";
var queryEmbedding = await generator.GenerateEmbeddingVectorAsync(query);
var searchOptions = new VectorSearchOptions()
{
    Top = 2,
    VectorPropertyName = "Vector"
};
```

**Vector stores** can be useful quickly to retrieve information, retrieving more information from the source or local information. With this objective, you can use Azure AI Search or locally Qdrant. 

To integrate **Qdrant** with **MEAI**, you can use the following code snippet, look into the sample `src/RAGSimple-04MEAIVectorsQdrant` for more info:

```csharp
// Create a vector store using Qdrant
var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"));

/// Using the vector store to get the collection of movies
var movies = vectorStore.GetCollection<ulong, MovieVector<ulong>>("movies");
```

> ⚠️ **Note**: Learn more about RAG go to [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners/tree/main/15-rag-and-vector-databases) and understand RAG in depth!


## Vision
(5 min video .NET explanation, links to main GenAI and content)

![Vision](../03-CoreGenerativeAITechniques/images/Vision.png)

**Vision-based AI** approaches are used to generate and interpret images. This can useful for a wide range of applications, such as image recognition, image generation, and image manipulation. Current models are multimodal, meaning they can accept a variety of inputs, such as text, images, and audio, and generate a variety of outputs. In this case, we are going to focus on image recognition.

> 💡 **Pro Tip**: Learn more about Image Generation approaches in [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners/tree/main/09-building-image-applications)

Meaning that we could use this technology to make questions about images and getting them formatted for any format or input needed. In example, questioning how many people are in the image, getting the answer in a JSON format, then sending for a function in your workflow.

For vision applications, we can use **Semantic Kernel**, **MEAI** and basic models to implement vision-based AI approaches.

Send images to the model with the following code snippet:

```csharp
// read the image bytes, create a new image content part and add it to the messages
AIContent aic = new ImageContent(File.ReadAllBytes(image), "image/jpeg");
List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.User, prompt),
    new ChatMessage(ChatRole.User, [aic])
 ];

var imageAnalysis = await chatClient.CompleteAsync(messages);
```

Videos can be analyzed in the same way, using the following code snippet:

```csharp
// Create the OpenAI files that represent the video frames, and add them to the messages
int step = (int)Math.Ceiling((double)frames.Count / PromptsHelper.NumberOfFrames);

// Show in the console the total number of frames and the step that neeeds to be taken to get the desired number of frames for the video analysis
Console.WriteLine($"Video total number of frames: {frames.Count}");
Console.WriteLine($"Get 1 frame every [{step}] to get the [{PromptsHelper.NumberOfFrames}] frames for analysis");

for (int i = 0; i < frames.Count; i += step)
{
    // Save the frame to the "data/frames" folder, using the frame number as the file name and the OpenCV library
    string framePath = Path.Combine(dataFolderPath, "frames", $"{i}.jpg");
    Cv2.ImWrite(framePath, frames[i]);

    // Read the image bytes, create a new image content part and add it to the messages
    AIContent aic = new ImageContent(File.ReadAllBytes(framePath), "image/jpeg");
    var message = new ChatMessage(ChatRole.User, [aic]);
    messages.Add(message);
}

// Send the messages to the chat client
var completionUpdates = chatClient.CompleteStreamingAsync(chatMessages: messages);
```

## Real-Time Audio
![Real-Time Audio](../03-CoreGenerativeAITechniques/images/Audio.png)

**Real-Time Audio** techniques allows you to generate and transcribe audio in real-time. This can be useful for a wide range of applications, speech synthesis, and audio manipulation. This is possible using [**Whisper**](https://github.com/openai/whisper), a real-time audio generation model that can  transcribe audio in real-time. 

Another option in audio, is to use the [**Cognitive Services Speech SDK**](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/quickstarts/setup-platform?tabs=windows%2Cubuntu%2Cdotnetcli%2Cdotnet%2Cjre%2Cmaven%2Cnodejs%2Cmac%2Cpypi&pivots=programming-language-csharp), which provides a way to interact with transcriptions and it can  generate audio in real-time.

A simple way to detect in real-time is to use the following code snippet in `src/Audio-01-SpeechMic`:

```csharp
// Create the speech translation configuration
using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

Console.WriteLine("Speak into your microphone.");
// Start the translation recognizer and wait for a result
var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
// Create an output speech recognition result
OutputSpeechRecognitionResult(translationRecognitionResult);
```

In the `src/Audio-02-RealTimeAudio` sample, we have a conversation with a Language Model, using the OpenAI Realtime Conversation library. Look into the sample for more information.

---

For last, explore how can you use the models in agentic systems, and how can you use them to create agents and assistants.

<p align="center">
    <a href="../03-CoreGenerativeAITechniques/03-agents-conclusions.md">Go to Part 3 - Agents</a>
</p>

---