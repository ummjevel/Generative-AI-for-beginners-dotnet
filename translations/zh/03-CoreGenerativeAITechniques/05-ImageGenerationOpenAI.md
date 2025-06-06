# 使用 Azure OpenAI 生成图像

在本课程中，我们将探索如何在 .NET 应用程序中使用 Azure OpenAI 的 DALL-E 来生成图像。图像生成允许您根据文本描述创建原创图像，为各种应用开启创意可能性。

---

## 介绍

[![使用 Azure OpenAI 生成图像](https://img.youtube.com/vi/ru3U8MHbFFI/0.jpg)](https://youtu.be/ru3U8MHbFFI?feature=shared)

_⬆️ 点击图片观看视频 ⬆️_

图像生成 AI 可以从文本描述或提示中创建原创图像。通过 Azure OpenAI 使用 DALL-E 等服务，您可以精确指定想要在图像中看到的内容，包括风格、构图、物体等等。这对于创建插图、概念艺术、设计模型和其他视觉内容非常有用。

## 使用 Azure OpenAI 生成图像

让我们探索如何在 .NET 应用程序中使用 Azure OpenAI 生成图像：

```csharp
var client = new OpenAIClient(
    new Uri("您的 Azure OpenAI 端点"), 
    new AzureKeyCredential("您的 Azure OpenAI API 密钥"));

ImageGenerationOptions imageGenerationOptions = new()
{
    DeploymentName = "dalle3", // Azure OpenAI 上 DALL-E 模型部署的名称
    Prompt = "一只小猫在月光下坐着，数字艺术风格",
    Size = ImageSize.Size1024x1024,
    Quality = ImageGenerationQuality.Standard,
    Style = ImageGenerationStyle.Natural,
};

Response<ImageGenerations> imageGenerations = await client.GetImageGenerationsAsync(imageGenerationOptions);
Uri imageUri = imageGenerations.Value.Data[0].Url;
```

### 示例应用程序

在 [ImageGeneration-01](./src/ImageGeneration-01) 示例中，我们实现了一个控制台应用程序，使用 DALL-E 模型根据文本提示生成图像。

## 下一步

👉 [让我们使用 AI Toolkit、Docker 和 Foundry Local 在本地运行模型！](./06-LocalModelRunners.md)

**免责声明**：  
本文档是通过基于机器的人工智能翻译服务翻译的。尽管我们努力确保准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原始语言的文档作为权威来源。对于关键信息，建议使用专业的人类翻译。对于因使用本翻译而导致的任何误解或误读，我们概不负责。