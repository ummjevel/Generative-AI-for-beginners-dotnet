using System;

namespace SpaceAINet.mjjeon
{
    public class RenderState
    {
        public char[,] CharBuffer { get; set; }
        public ConsoleColor[,] ColorBuffer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public RenderState(int width, int height)
        {
            Width = width;
            Height = height;
            CharBuffer = new char[height, width];
            ColorBuffer = new ConsoleColor[height, width];
            
            // Initialize buffers with empty spaces
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CharBuffer[y, x] = ' ';
                    ColorBuffer[y, x] = ConsoleColor.Black;
                }
            }
        }

        public void SetChar(int x, int y, char ch, ConsoleColor color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                CharBuffer[y, x] = ch;
                ColorBuffer[y, x] = color;
            }
        }

        public char GetChar(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return CharBuffer[y, x];
            return ' ';
        }

        public ConsoleColor GetColor(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return ColorBuffer[y, x];
            return ConsoleColor.Black;
        }
    }
}
