using System;

namespace SpaceAINet.mjjeon
{
    public class Enemy
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Pattern { get; set; }
        public ConsoleColor Color { get; set; }
        public bool IsAlive { get; set; } = true;
        public bool MovingRight { get; set; } = true;
        
        private int gameWidth;
        private int gameHeight;

        public Enemy(int x, int y, string pattern, ConsoleColor color, int gameWidth, int gameHeight)
        {
            X = x;
            Y = y;
            Pattern = pattern;
            Color = color;
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;
        }

        public void Update()
        {
            if (!IsAlive) return;

            // Move horizontally
            if (MovingRight)
            {
                X++;
                if (X + Pattern.Length >= gameWidth - 1)
                {
                    MovingRight = false;
                }
            }
            else
            {
                X--;
                if (X <= 1)
                {
                    MovingRight = true;
                }
            }
        }

        public void MoveDown()
        {
            if (IsAlive)
            {
                Y++;
            }
        }

        public bool CheckCollision(int bulletX, int bulletY)
        {
            if (!IsAlive) return false;
            
            return bulletX >= X && bulletX < X + Pattern.Length && bulletY == Y;
        }

        public void Render(RenderState renderState)
        {
            if (!IsAlive) return;

            for (int i = 0; i < Pattern.Length; i++)
            {
                renderState.SetChar(X + i, Y, Pattern[i], Color);
            }
        }
    }
}
