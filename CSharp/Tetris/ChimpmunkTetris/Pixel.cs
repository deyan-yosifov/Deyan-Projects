using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    class Pixel : IDraw, IErase
    {
        public Point2D Coordinate { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }

        public Pixel(int x, int y, char symbol, ConsoleColor color)
        {
            Coordinate = new Point2D(x, y);
            Symbol = symbol;
            Color = color;
        }

        public void Draw()
        {
            Pixel.Draw(this.Coordinate.X, this.Coordinate.Y, this.Symbol, this.Color);
        }

        public static void Draw(int x, int y)
        {
            Pixel.Draw(x, y, '@', ConsoleColor.DarkMagenta);
        }

        public static void Draw(int x, int y, char symbol, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }

        public static void Draw(int x, int y, string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }
        
        public void Erase()
        {
            Pixel.Erase(this.Coordinate.X, this.Coordinate.Y);
        }

        public static void Erase(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(' ');
        }

        public static void Erase(int x, int y, string text)
        {
            Pixel.Draw(x, y, new String(' ', text.Length), ConsoleColor.Gray);
        }

        public override bool Equals(object obj)
        {
            if (obj is Pixel)
            {
                return this.Coordinate.Equals((obj as Pixel).Coordinate);
            }
            else if (obj is Point2D)
            {
                return this.Coordinate.Equals(obj as Point2D);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Coordinate.GetHashCode();
        }
    }
}
