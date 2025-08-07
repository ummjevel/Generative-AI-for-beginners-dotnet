using System;

namespace SpaceAINet.mjjeon
{
    public enum GameMode
    {
        Manual,
        AI_Ollama
    }

    public class GameStateAnalyzer
    {
        public static string GenerateGameState(RenderState renderState, Player player, int score, TimeSpan elapsed)
        {
            var gameState = new System.Text.StringBuilder();
            
            gameState.AppendLine($"Score: {score}, Time: {elapsed.TotalSeconds:F0}s");
            gameState.AppendLine($"Player Position: X={player.X}, Y={player.Y}");
            gameState.AppendLine($"Player Bullets: {player.CurrentBullets}/{player.MaxBullets}");
            gameState.AppendLine();
            gameState.AppendLine("Game Field:");
            
            // Generate a simplified view of the game field
            for (int y = 1; y < renderState.Height - 1; y++)
            {
                var line = new System.Text.StringBuilder();
                for (int x = 1; x < renderState.Width - 1; x++)
                {
                    char ch = renderState.GetChar(x, y);
                    if (ch == ' ')
                        line.Append('.');
                    else
                        line.Append(ch);
                }
                gameState.AppendLine(line.ToString());
            }
            
            return gameState.ToString();
        }
        
        public static string GetLastActionString(ConsoleKey? lastKey)
        {
            return lastKey switch
            {
                ConsoleKey.LeftArrow => "move_left",
                ConsoleKey.RightArrow => "move_right",
                ConsoleKey.Spacebar => "fire",
                _ => "stop"
            };
        }
    }
}
