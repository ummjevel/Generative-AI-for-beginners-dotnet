using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpaceAINet.mjjeon
{
    public class ScreenshotService
    {
        private static string screenshotFolder = "screenshoots";
        private static int screenshotCounter = 0;

        public static void Initialize()
        {
            // Create and clear the screenshot folder
            if (Directory.Exists(screenshotFolder))
            {
                Directory.Delete(screenshotFolder, true);
            }
            Directory.CreateDirectory(screenshotFolder);
            screenshotCounter = 0;
        }

        public static void CaptureScreenshot(RenderState renderState)
        {
            try
            {
                int charWidth = 8;
                int charHeight = 16;
                int imageWidth = renderState.Width * charWidth;
                int imageHeight = renderState.Height * charHeight;

                using (Bitmap bitmap = new Bitmap(imageWidth, imageHeight))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Black);
                    
                    // Use a monospace font
                    Font font = new Font("Consolas", 12, FontStyle.Regular);
                    
                    for (int y = 0; y < renderState.Height; y++)
                    {
                        for (int x = 0; x < renderState.Width; x++)
                        {
                            char ch = renderState.GetChar(x, y);
                            ConsoleColor consoleColor = renderState.GetColor(x, y);
                            Color color = ConvertConsoleColor(consoleColor);
                            
                            if (ch != ' ')
                            {
                                using (Brush brush = new SolidBrush(color))
                                {
                                    graphics.DrawString(ch.ToString(), font, brush, 
                                        x * charWidth, y * charHeight);
                                }
                            }
                        }
                    }
                    
                    font.Dispose();
                    
                    string filename = Path.Combine(screenshotFolder, 
                        $"screenshot_{screenshotCounter:D4}.png");
                    bitmap.Save(filename, ImageFormat.Png);
                    screenshotCounter++;
                }
            }
            catch (Exception ex)
            {
                // Silently fail if screenshot cannot be taken
                Console.WriteLine($"Screenshot failed: {ex.Message}");
            }
        }

        private static Color ConvertConsoleColor(ConsoleColor consoleColor)
        {
            return consoleColor switch
            {
                ConsoleColor.Black => Color.Black,
                ConsoleColor.DarkBlue => Color.DarkBlue,
                ConsoleColor.DarkGreen => Color.DarkGreen,
                ConsoleColor.DarkCyan => Color.DarkCyan,
                ConsoleColor.DarkRed => Color.DarkRed,
                ConsoleColor.DarkMagenta => Color.DarkMagenta,
                ConsoleColor.DarkYellow => Color.FromArgb(128, 128, 0),
                ConsoleColor.Gray => Color.Gray,
                ConsoleColor.DarkGray => Color.DarkGray,
                ConsoleColor.Blue => Color.Blue,
                ConsoleColor.Green => Color.Green,
                ConsoleColor.Cyan => Color.Cyan,
                ConsoleColor.Red => Color.Red,
                ConsoleColor.Magenta => Color.Magenta,
                ConsoleColor.Yellow => Color.Yellow,
                ConsoleColor.White => Color.White,
                _ => Color.White
            };
        }
    }
}
