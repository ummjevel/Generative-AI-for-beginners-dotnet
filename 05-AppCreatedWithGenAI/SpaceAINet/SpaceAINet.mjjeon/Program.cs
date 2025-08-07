using System;

namespace SpaceAINet.mjjeon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set UTF-8 output encoding for box-drawing characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            
            // Check if user wants to test AI
            if (args.Length > 0 && args[0].ToLower() == "test-ai")
            {
                await AITestRunner.TestOllamaConnection();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            
            // Show start screen
            StartScreen.Show();
            
            // Read user input for speed selection
            int gameSpeed = 1; // Default slow
            
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                if (keyInfo.Key == ConsoleKey.D1 || keyInfo.KeyChar == '1')
                {
                    gameSpeed = 1; // Slow
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.D2 || keyInfo.KeyChar == '2')
                {
                    gameSpeed = 2; // Medium
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.D3 || keyInfo.KeyChar == '3')
                {
                    gameSpeed = 3; // Fast
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    gameSpeed = 1; // Default slow
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.T)
                {
                    // Quick AI test
                    Console.Clear();
                    await AITestRunner.TestOllamaConnection();
                    Console.WriteLine("\nPress any key to continue to game...");
                    Console.ReadKey();
                    Console.Clear();
                    StartScreen.Show();
                    continue;
                }
                else if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Q)
                {
                    return; // Exit program
                }
            }
            
            // Clear the console and start the game
            Console.Clear();
            
            // Create and run game manager
            GameManager gameManager = new GameManager(gameSpeed);
            await gameManager.RunGameLoop();
            
            // Game ended
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Game Over! Thanks for playing!");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
