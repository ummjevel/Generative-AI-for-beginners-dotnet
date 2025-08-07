using System;

namespace SpaceAINet.mjjeon
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int MaxBullets { get; set; } = 3;
        public int CurrentBullets { get; set; } = 0;
        public char Symbol { get; set; } = 'A';
        public ConsoleColor Color { get; set; } = ConsoleColor.Cyan;
        
        private int gameWidth;
        private int gameHeight;

        public Player(int gameWidth, int gameHeight)
        {
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;
            X = gameWidth / 2;
            Y = gameHeight - 3; // Lower area of screen
        }

        public void MoveLeft()
        {
            if (X > 1)
                X--;
        }

        public void MoveRight()
        {
            if (X < gameWidth - 2)
                X++;
        }

        public bool CanShoot()
        {
            return CurrentBullets < MaxBullets;
        }

        public void ShootBullet()
        {
            if (CanShoot())
            {
                CurrentBullets++;
            }
        }

        public void BulletDestroyed()
        {
            if (CurrentBullets > 0)
                CurrentBullets--;
        }

        public void Render(RenderState renderState)
        {
            renderState.SetChar(X, Y, Symbol, Color);
        }
    }
}
