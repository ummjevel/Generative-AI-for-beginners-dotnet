# 使用 Azure OpenAI 生成圖像

在本課程中，我們將探索如何在 .NET 應用程式中使用 Azure OpenAI 的 DALL-E 來生成圖像。圖像生成允許您根據文本描述創建原創圖像，為各種應用開啟創意可能性。

---

## 介紹

[![使用 Azure OpenAI 生成圖像](https://img.youtube.com/vi/ru3U8MHbFFI/0.jpg)](https://youtu.be/ru3U8MHbFFI?feature=shared)

_⬆️ 點擊圖片觀看視頻 ⬆️_

圖像生成 AI 可以從文本描述或提示中創建原創圖像。通過 Azure OpenAI 使用 DALL-E 等服務，您可以精確指定想要在圖像中看到的內容，包括風格、構圖、物體等等。這對於創建插圖、概念藝術、設計模型和其他視覺內容非常有用。

## 使用 Azure OpenAI 生成圖像

讓我們探索如何在 .NET 應用程式中使用 Azure OpenAI 生成圖像：

```csharp
var client = new OpenAIClient(
    new Uri("您的 Azure OpenAI 端點"), 
    new AzureKeyCredential("您的 Azure OpenAI API 密鑰"));

ImageGenerationOptions imageGenerationOptions = new()
{
    DeploymentName = "dalle3", // Azure OpenAI 上 DALL-E 模型部署的名稱
    Prompt = "一隻小貓在月光下坐著，數字藝術風格",
    Size = ImageSize.Size1024x1024,
    Quality = ImageGenerationQuality.Standard,
    Style = ImageGenerationStyle.Natural,
};

Response<ImageGenerations> imageGenerations = await client.GetImageGenerationsAsync(imageGenerationOptions);
Uri imageUri = imageGenerations.Value.Data[0].Url;
```

### 範例應用程式

在 [ImageGeneration-01](./src/ImageGeneration-01) 範例中，我們實現了一個控制台應用程式，使用 DALL-E 模型根據文本提示生成圖像。

## 下一步

👉 [讓我們使用 AI Toolkit、Docker 和 Foundry Local 在本地運行模型！](./06-LocalModelRunners.md)

**免責聲明**：  
本文檔使用機器翻譯服務進行翻譯。儘管我們努力保證準確性，但請注意，自動翻譯可能包含錯誤或不準確之處。應以原始語言的文件作為權威來源。對於關鍵資訊，建議尋求專業人工翻譯。我們對使用本翻譯而引起的任何誤解或錯誤解釋不承擔責任。