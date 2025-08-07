using System;

namespace SpaceAINet.mjjeon
{
    public class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.White;
        public bool IsPlayerBullet { get; set; }
        public bool IsActive { get; set; } = true;
        
        private int gameWidth;
        private int gameHeight;

        public Bullet(int x, int y, bool isPlayerBullet, int gameWidth, int gameHeight)
        {
            X = x;
            Y = y;
            IsPlayerBullet = isPlayerBullet;
            Symbol = isPlayerBullet ? '^' : 'v';
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;
        }

        public void Update()
        {
            if (!IsActive) return;

            if (IsPlayerBullet)
            {
                Y--; // Move up
                if (Y <= 1) // Hit top boundary
                {
                    IsActive = false;
                }
            }
            else
            {
                Y++; // Move down
                if (Y >= gameHeight - 2) // Hit bottom boundary
                {
                    IsActive = false;
                }
            }
        }

        public bool CheckCollisionWithPlayer(Player player)
        {
            if (!IsActive || IsPlayerBullet) return false;
            
            return X == player.X && Y == player.Y;
        }

        public void Destroy()
        {
            IsActive = false;
        }

        public void Render(RenderState renderState)
        {
            if (IsActive)
            {
                renderState.SetChar(X, Y, Symbol, Color);
            }
        }
    }
}
