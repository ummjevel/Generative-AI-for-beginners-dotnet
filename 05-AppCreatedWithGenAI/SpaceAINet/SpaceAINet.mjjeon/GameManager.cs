using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceAINet.mjjeon
{
    public class GameManager
    {
        private Player player = null!;
        private List<Enemy> enemies = null!;
        private List<Bullet> bullets = null!;
        private RenderState currentState;
        private RenderState previousState;
        private int gameWidth;
        private int gameHeight;
        private int score = 0;
        private DateTime startTime;
        private int gameSpeed = 100; // milliseconds between frames
        private Random random = new Random();
        private int enemyShootCooldown = 0;
        private bool gameRunning = true;
        
        // AI-related fields
        private GameMode currentMode = GameMode.Manual;
        private OllamaAIService aiService = null!;
        private ConsoleKey? lastAIAction = null;
        private DateTime lastAIThinkTime = DateTime.Now;
        private string lastAIActionString = "stop";
        private int aiThinkCooldown = 0;
        private string statusMessage = "";
        private DateTime statusMessageTime = DateTime.Now;

        public GameManager(int speed = 1)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            
            gameWidth = Console.WindowWidth;
            gameHeight = Console.WindowHeight;
            
            // Set game speed
            gameSpeed = speed switch
            {
                1 => 150, // Slow
                2 => 100, // Medium
                3 => 50,  // Fast
                _ => 150  // Default slow
            };
            
            currentState = new RenderState(gameWidth, gameHeight);
            previousState = new RenderState(gameWidth, gameHeight);
            
            InitializeGame();
            InitializeAI();
            ScreenshotService.Initialize();
            startTime = DateTime.Now;
        }

        private async void InitializeAI()
        {
            // Console.WriteLine("[DEBUG] Initializing AI service...");
            aiService = new OllamaAIService();
            
            // Check if Ollama is available
            bool isAvailable = await aiService.IsAvailableAsync();
            // Console.WriteLine($"[DEBUG] AI availability: {isAvailable}");
            
            if (!isAvailable)
            {
                // Console.WriteLine("[DEBUG] AI not available, staying in manual mode");
                currentMode = GameMode.Manual;
            }
            else
            {
                // Console.WriteLine("[DEBUG] AI service ready!");
            }
        }

        private void InitializeGame()
        {
            // Initialize player
            player = new Player(gameWidth, gameHeight);
            
            // Initialize enemies
            enemies = new List<Enemy>();
            
            // Top row (5 enemies): ><, oo, ><, oo, ><
            string[] topPatterns = { "><", "oo", "><", "oo", "><" };
            for (int i = 0; i < 5; i++)
            {
                enemies.Add(new Enemy(5 + i * 6, 3, topPatterns[i], ConsoleColor.Red, gameWidth, gameHeight));
            }
            
            // Bottom row (3 enemies): /O\
            for (int i = 0; i < 3; i++)
            {
                enemies.Add(new Enemy(8 + i * 8, 5, "/O\\", ConsoleColor.DarkYellow, gameWidth, gameHeight));
            }
            
            bullets = new List<Bullet>();
        }

        public async Task RunGameLoop()
        {
            Console.Clear();
            
            while (gameRunning)
            {
                if (currentMode == GameMode.Manual)
                {
                    HandleInput();
                }
                else if (currentMode == GameMode.AI_Ollama)
                {
                    await HandleAIInput();
                }
                
                UpdateGame();
                Render();
                Thread.Sleep(gameSpeed);
            }
            
            // Cleanup
            aiService?.Dispose();
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        player.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        player.MoveRight();
                        break;
                    case ConsoleKey.Spacebar:
                        if (player.CanShoot())
                        {
                            bullets.Add(new Bullet(player.X, player.Y - 1, true, gameWidth, gameHeight));
                            player.ShootBullet();
                        }
                        break;
                    case ConsoleKey.S:
                        ScreenshotService.CaptureScreenshot(currentState);
                        break;
                    case ConsoleKey.O:
                        // Console.WriteLine("[DEBUG] O key pressed in manual mode");
                        ToggleAIMode();
                        break;
                    case ConsoleKey.Q:
                        gameRunning = false;
                        break;
                }
            }
        }

        private async Task HandleAIInput()
        {
            // Handle manual overrides
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.S:
                        ScreenshotService.CaptureScreenshot(currentState);
                        break;
                    case ConsoleKey.O:
                        // Console.WriteLine("[DEBUG] O key pressed in AI mode");
                        ToggleAIMode();
                        return;
                    case ConsoleKey.Q:
                        gameRunning = false;
                        return;
                }
            }
            
            // AI decision making
            aiThinkCooldown--;
            if (aiThinkCooldown <= 0)
            {
                try
                {
                    var gameState = GameStateAnalyzer.GenerateGameState(currentState, player, score, DateTime.Now - startTime);
                    var nextAction = await aiService.GetNextActionAsync(gameState, lastAIActionString);
                    
                    ExecuteAIAction(nextAction);
                    lastAIActionString = nextAction;
                    lastAIThinkTime = DateTime.Now;
                    aiThinkCooldown = 5; // AI thinks every 5 frames
                }
                catch
                {
                    // If AI fails, do nothing
                    lastAIActionString = "stop";
                }
            }
        }

        private void ExecuteAIAction(string action)
        {
            switch (action.ToLower())
            {
                case "move_left":
                    player.MoveLeft();
                    lastAIAction = ConsoleKey.LeftArrow;
                    break;
                case "move_right":
                    player.MoveRight();
                    lastAIAction = ConsoleKey.RightArrow;
                    break;
                case "fire":
                    if (player.CanShoot())
                    {
                        bullets.Add(new Bullet(player.X, player.Y - 1, true, gameWidth, gameHeight));
                        player.ShootBullet();
                    }
                    lastAIAction = ConsoleKey.Spacebar;
                    break;
                default:
                    lastAIAction = null;
                    break;
            }
        }

        private void ShowStatusMessage(string message)
        {
            statusMessage = message;
            statusMessageTime = DateTime.Now;
        }

        private void ToggleAIMode()
        {
            if (currentMode == GameMode.Manual)
            {
                // Console.WriteLine("[DEBUG] Switching to AI mode...");
                // Note: Making this synchronous to avoid issues
                Task.Run(async () =>
                {
                    bool isAvailable = await aiService.IsAvailableAsync();
                    if (isAvailable)
                    {
                        currentMode = GameMode.AI_Ollama;
                        lastAIActionString = "stop";
                        aiThinkCooldown = 0;
                        ShowStatusMessage("AI Mode ON");
                        // Console.WriteLine("[DEBUG] AI mode activated!");
                    }
                    else
                    {
                        ShowStatusMessage("AI Not Available");
                        // Console.WriteLine("[DEBUG] AI not available, staying in manual mode");
                    }
                });
            }
            else
            {
                // Console.WriteLine("[DEBUG] Switching to manual mode...");
                currentMode = GameMode.Manual;
                ShowStatusMessage("Manual Mode ON");
                // Console.WriteLine("[DEBUG] Manual mode activated!");
            }
        }

        private void UpdateGame()
        {
            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                
                if (!bullets[i].IsActive)
                {
                    if (bullets[i].IsPlayerBullet)
                        player.BulletDestroyed();
                    bullets.RemoveAt(i);
                    continue;
                }
                
                // Check bullet-enemy collisions
                if (bullets[i].IsPlayerBullet)
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.CheckCollision(bullets[i].X, bullets[i].Y))
                        {
                            enemy.IsAlive = false;
                            bullets[i].Destroy();
                            player.BulletDestroyed();
                            score += 10;
                            break;
                        }
                    }
                }
                else
                {
                    // Check bullet-player collision
                    if (bullets[i].CheckCollisionWithPlayer(player))
                    {
                        // Game over or lose life
                        gameRunning = false;
                        break;
                    }
                }
            }
            
            // Update enemies
            bool shouldMoveDown = false;
            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                int oldX = enemy.X;
                enemy.Update();
                
                // Check if any enemy hit the edge
                if ((enemy.MovingRight && enemy.X + enemy.Pattern.Length >= gameWidth - 1) ||
                    (!enemy.MovingRight && enemy.X <= 1))
                {
                    shouldMoveDown = true;
                }
            }
            
            if (shouldMoveDown)
            {
                foreach (var enemy in enemies.Where(e => e.IsAlive))
                {
                    enemy.MoveDown();
                }
            }
            
            // Enemy shooting
            enemyShootCooldown--;
            if (enemyShootCooldown <= 0 && enemies.Any(e => e.IsAlive))
            {
                var aliveEnemies = enemies.Where(e => e.IsAlive).ToList();
                if (aliveEnemies.Count > 0)
                {
                    var shootingEnemy = aliveEnemies[random.Next(aliveEnemies.Count)];
                    bullets.Add(new Bullet(shootingEnemy.X + shootingEnemy.Pattern.Length / 2, 
                                         shootingEnemy.Y + 1, false, gameWidth, gameHeight));
                    enemyShootCooldown = 60; // Cooldown frames
                }
            }
            
            // Check win condition
            if (!enemies.Any(e => e.IsAlive))
            {
                gameRunning = false;
            }
        }

        private void Render()
        {
            // Clear current state
            for (int y = 0; y < gameHeight; y++)
            {
                for (int x = 0; x < gameWidth; x++)
                {
                    currentState.SetChar(x, y, ' ', ConsoleColor.Black);
                }
            }
            
            // Draw bounding box
            DrawBoundingBox();
            
            // Draw UI
            DrawUI();
            
            // Render game objects
            player.Render(currentState);
            
            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                enemy.Render(currentState);
            }
            
            foreach (var bullet in bullets.Where(b => b.IsActive))
            {
                bullet.Render(currentState);
            }
            
            // Double-buffered rendering: only update changed characters
            for (int y = 0; y < gameHeight; y++)
            {
                for (int x = 0; x < gameWidth; x++)
                {
                    char currentChar = currentState.GetChar(x, y);
                    char previousChar = previousState.GetChar(x, y);
                    ConsoleColor currentColor = currentState.GetColor(x, y);
                    ConsoleColor previousColor = previousState.GetColor(x, y);
                    
                    if (currentChar != previousChar || currentColor != previousColor)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = currentColor;
                        Console.Write(currentChar);
                        
                        previousState.SetChar(x, y, currentChar, currentColor);
                    }
                }
            }
        }

        private void DrawBoundingBox()
        {
            // Top border
            currentState.SetChar(0, 0, '┌', ConsoleColor.White);
            for (int x = 1; x < gameWidth - 1; x++)
            {
                currentState.SetChar(x, 0, '─', ConsoleColor.White);
            }
            currentState.SetChar(gameWidth - 1, 0, '┐', ConsoleColor.White);
            
            // Bottom border
            currentState.SetChar(0, gameHeight - 1, '└', ConsoleColor.White);
            for (int x = 1; x < gameWidth - 1; x++)
            {
                currentState.SetChar(x, gameHeight - 1, '─', ConsoleColor.White);
            }
            currentState.SetChar(gameWidth - 1, gameHeight - 1, '┘', ConsoleColor.White);
            
            // Side borders
            for (int y = 1; y < gameHeight - 1; y++)
            {
                currentState.SetChar(0, y, '│', ConsoleColor.White);
                currentState.SetChar(gameWidth - 1, y, '│', ConsoleColor.White);
            }
        }

        private void DrawUI()
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            int aliveEnemies = enemies.Count(e => e.IsAlive);
            
            string modeText = currentMode == GameMode.AI_Ollama ? "[🤖 AI-Ollama]" : "[👤 Manual]";
            string aiActionText = currentMode == GameMode.AI_Ollama ? $" Action: {lastAIActionString}" : " Press 'O' to enable AI";
            
            // Show status message for 3 seconds
            if (!string.IsNullOrEmpty(statusMessage) && (DateTime.Now - statusMessageTime).TotalSeconds < 3)
            {
                aiActionText = $" {statusMessage}";
            }
            
            string ui = $"Score: {score:D4}   Time: {elapsed.TotalSeconds:F0}s   Bullets: {player.CurrentBullets}/{player.MaxBullets}   Enemies: {aliveEnemies} {modeText}{aiActionText}";
            
            // Ensure UI fits within the box
            if (ui.Length > gameWidth - 4)
            {
                ui = ui.Substring(0, gameWidth - 4);
            }
            
            for (int i = 0; i < ui.Length; i++)
            {
                ConsoleColor uiColor = ConsoleColor.White;
                if (i >= ui.IndexOf(modeText) && i < ui.IndexOf(modeText) + modeText.Length)
                {
                    uiColor = currentMode == GameMode.AI_Ollama ? ConsoleColor.Green : ConsoleColor.Cyan;
                }
                currentState.SetChar(2 + i, 1, ui[i], uiColor);
            }
        }

        public RenderState GetRenderState()
        {
            return currentState;
        }
    }
}
