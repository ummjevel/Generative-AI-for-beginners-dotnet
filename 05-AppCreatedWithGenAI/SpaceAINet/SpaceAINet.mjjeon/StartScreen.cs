using System;

namespace SpaceAINet.mjjeon
{
    public static class StartScreen
    {
        public static void Show()
        {
            Console.Clear();
            
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            
            // Title centered
            string title = "Space.AI.NET()";
            int titleX = (width - title.Length) / 2;
            Console.SetCursorPosition(titleX, height / 4);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(title);
            
            // Subtitle centered
            string subtitle = "Built with .NET + AI for galactic defense";
            int subtitleX = (width - subtitle.Length) / 2;
            Console.SetCursorPosition(subtitleX, height / 4 + 2);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(subtitle);
            
            // Instructions - left aligned
            Console.ForegroundColor = ConsoleColor.White;
            int startY = height / 2 - 5;
            int leftMargin = 10;
            
            Console.SetCursorPosition(leftMargin, startY);
            Console.WriteLine("How to Play:");
            Console.SetCursorPosition(leftMargin, startY + 1);
            Console.WriteLine("←   Move Left");
            Console.SetCursorPosition(leftMargin, startY + 2);
            Console.WriteLine("→   Move Right");
            Console.SetCursorPosition(leftMargin, startY + 3);
            Console.WriteLine("SPACE   Shoot");
            Console.SetCursorPosition(leftMargin, startY + 4);
            Console.WriteLine("S   Take Screenshot");
            Console.SetCursorPosition(leftMargin, startY + 5);
            Console.WriteLine("O   Toggle AI Mode (Ollama)");
            Console.SetCursorPosition(leftMargin, startY + 6);
            Console.WriteLine("Q   Quit");
            
            Console.SetCursorPosition(leftMargin, startY + 8);
            Console.WriteLine("Select Game Speed:");
            Console.SetCursorPosition(leftMargin, startY + 9);
            Console.WriteLine("[1] Slow (default)");
            Console.SetCursorPosition(leftMargin, startY + 10);
            Console.WriteLine("[2] Medium");
            Console.SetCursorPosition(leftMargin, startY + 11);
            Console.WriteLine("[3] Fast");
            Console.SetCursorPosition(leftMargin, startY + 12);
            Console.WriteLine("Press ENTER for default");
            Console.SetCursorPosition(leftMargin, startY + 13);
            Console.WriteLine("[T] Test AI Connection");
            
            Console.SetCursorPosition(leftMargin, startY + 15);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("AI Mode: Press 'O' during game to let Ollama AI play!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(leftMargin, startY + 16);
            Console.WriteLine("Make sure Ollama is running: ollama serve");
            Console.SetCursorPosition(leftMargin, startY + 17);
            Console.WriteLine("To test AI: dotnet run test-ai");
            
            Console.ResetColor();
        }
    }
}
