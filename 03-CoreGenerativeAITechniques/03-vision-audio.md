# Vision and Audio AI apps

In this lesson learn how vision AI allows your apps to generate and interpret images. Audio AI provides your apps to generate audio and even transcribe it in real-time.

---

## Vision

**INSERT: LESSON 3 VISION VIDEO HERE**

Vision-based AI approaches are used to generate and interpret images. This can useful for a wide range of applications, such as image recognition, image generation, and image manipulation. Current models are multimodal, meaning they can accept a variety of inputs, such as text, images, and audio, and generate a variety of outputs. In this case, we are going to focus on image recognition.

### Image recognition with MEAI

Image recognition is more than having the AI model tell you what it thinks is present in an image. You can also ask questions about the image, for example: _How many people are present and is it raining?_

Ok - so we're going to put the model through it's paces and ask it if it can tell us how many red shoes are in the first photo and then have it analyze a receipt that's in German so we know how much to tip.

![A composite showing both images the example will use. The first is several runners but only showing their legs. The second is a German restaurant receipt](./images/example-visual-image.png)

1. We're using MEAI and GitHub Models, so instantiate the `IChatClient` as we have been. Also start to create a chat history.

    ```csharp
    IChatClient chatClient = new ChatCompletionsClient(
        endpoint: new Uri("https://models.inference.ai.azure.com"),
        new AzureKeyCredential(githubToken)) // make sure to grab githubToken from the secrets or environment
    .AsChatClient("gpt-4o-mini");

    List<ChatMessage> messages = 
    [
        new ChatMessage(ChatRole.System, "You are a useful assistant that describes images using a direct style."),
        new ChatMessage(ChatRole.User, "How many red shoes are in the photo?") // we'll start with the running photo
    ];
    ```

1. The next part is to load the image into an `AIContent` object and set that as part of our conversation and then send that off to the model to describe for us.

    ```csharp
    var imagePath = "FULL PATH TO THE IMAGE ON DISK";

    AIContent imageContent = new ImageContent(File.ReadAllBytes(imagePath), "image/jpeg");
    var imageMessage = new ChatMessage(ChatRole.User, [imageContent]);

    messages.Add(imageMessage);

    var response = await chatClient.CompleteAsync(messages);
    ```