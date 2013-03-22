using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    class Field : IDraw, IErase
    {
        public Point2D TopLeftPlayGround { get; set; }
        public Point2D BottomRightPlayGround { get; set; }

        public Border Border { get; set; }
        public Score Score { get; set; }
        public NextFallingObject NextFallingObject { get; set; }
        public Earth Earth { get; set; }
        public FallingObject FallingObject { get; set; }
        public ControlsInfo ControlsInfo { get; set; }
        public Clock Clock { get; set; }

        public Field():this(null, null)
        {
             
        }

        public Field(FallingObject fallingObject, FallingObject nextFallingObject)
        {
            this.FallingObject = fallingObject;
            this.TopLeftPlayGround = new Point2D(4, 5);
            this.BottomRightPlayGround = new Point2D(Game.Width - 20, Game.Height - 5);

            this.Earth = new Earth(this.TopLeftPlayGround, this.BottomRightPlayGround);
            this.Border = new Border(this);
            this.NextFallingObject = new NextFallingObject(this, this.FallingObject);
            this.ControlsInfo = new ControlsInfo(this);
            this.Score = new Score(this);
            this.Clock = new Clock(this);
        }

        public void Draw()
        {
            this.Border.Draw();
            this.Score.Draw();
            this.NextFallingObject.Draw();
            this.Earth.Draw();
            this.FallingObject.Draw();
            this.ControlsInfo.Draw();
            this.Clock.Draw();
        }
        public void Erase()
        {
            Console.Clear();
        }

        public void PauseDraw(ConsoleColor textColor, ConsoleColor borderColor)
        {
            
            int x = TopLeftPlayGround.X - 3;
            int y = TopLeftPlayGround.Y + 5;

            for(int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 20), borderColor);
           
            Pixel.Draw(x, ++y, "The Game Is Paused!".PadRight(10,' '), textColor);
            Pixel.Draw(x, ++y, "Press Space to Play", textColor);

            for (int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 20), borderColor);
        }

        public void WelcomeDraw(ConsoleColor textColor, ConsoleColor borderColor)
        {

            int x = TopLeftPlayGround.X - 3;
            int y = TopLeftPlayGround.Y + 5;

            for (int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 30), borderColor);

            Pixel.Draw(x, ++y, "  WELCOME to Chipmunk Tetris! ".PadRight(10, ' '), textColor);
            Pixel.Draw(x, ++y, "     Press any key to start   ", textColor);

            for (int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 30), borderColor);
        }

        public void GameOverDraw(ConsoleColor textColor, ConsoleColor borderColor)
        {

            int x = TopLeftPlayGround.X - 3;
            int y = TopLeftPlayGround.Y + 5;

            for (int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 20), borderColor);

            Pixel.Draw(x, ++y, "      GAME OVER     ".PadRight(10, ' '), textColor);
            Pixel.Draw(x, ++y, "Press Space to Play!", textColor);
            Pixel.Draw(x, ++y, "Press Escape to Exit", textColor);

            for (int i = 0; i < 2; i++)
                Pixel.Draw(x, ++y, new String('-', 20), borderColor);
        }
    }

    class Clock : IDraw, IErase
    {
        private int x;
        private int y;
        private TimeSpan time;

        public Clock (Field field)
        {
            this.time = new TimeSpan();
            this.x = field.BottomRightPlayGround.X;
            this.y = field.TopLeftPlayGround.Y - 3;
        }

        public void AddSeconds(double seconds)
        {
            time += TimeSpan.FromSeconds(seconds);
        }

        public void Update()
        {
            this.Erase();
            this.DrawTime();
        }

        public void Draw()
        {
            for (int row = x + 4; row <= x + 16; row++)
            {
                Pixel.Draw(row, y + 37, '-', Game.Colors.Foreground);
                Pixel.Draw(row, y + 39, '-', Game.Colors.Foreground);
            }
            DrawTime();
        }

        public void DrawTime()
        {
            Pixel.Draw(x + 7, y + 38, String.Format("{0}:{1}:{2}",
                time.Hours.ToString().PadLeft(2, '0'), time.Minutes.ToString().PadLeft(2, '0'),
                time.Seconds.ToString().PadLeft(2, '0')), Game.Colors.TextColor);
        }

        public void Erase()
        {
            Pixel.Erase(x + 7, y + 38, String.Format("{0}:{1}:{2}",
                time.Hours.ToString().PadLeft(2, '0'), time.Minutes.ToString().PadLeft(2, '0'),
                time.Seconds.ToString().PadLeft(2, '0')));
        }
    }

    class Border : IDraw, IErase
    {
        private int startX;
        private int endX;
        private int startY;
        private int endY;

        public Border(Field field)
        {
            this.startX = field.TopLeftPlayGround.X;//4
            this.endX = field.BottomRightPlayGround.X;//Game.Width - 20
            this.startY = field.TopLeftPlayGround.Y;//10
            this.endY = field.BottomRightPlayGround.Y;//Game.Height - 5
        }
        
        public void Erase()
        {
            throw new  NotImplementedException();
        }

        public void Draw()
        {

            TetrisDraw(endX + 5, startY - 1, Game.Colors.TextColor);
            //vertical
            for (int col = startY; col <= endY;  col++)
            {
                Pixel.Draw(startX, col, '#', Game.Colors.Foreground);
                Pixel.Draw(endX, col, '#', Game.Colors.Foreground);
            }
            //horizontal
            for (int row = startX; row <= endX; row++)
            {
                Pixel.Draw(row, startY, '#', Game.Colors.Foreground);
                Pixel.Draw(row, endY, '#', Game.Colors.Foreground); 
            }

            AutorsDraw(endX + 3, endY + 1, Game.Colors.TextColor);
        }

        public void TetrisDraw(int x, int y, ConsoleColor color)
        {
            Pixel.Draw(x, ++y, "TETRIS GAME", color);
        }

        public void AutorsDraw(int x, int y, ConsoleColor color)
        {
            Pixel.Draw(x, ++y, "by Team Chipmunk", color);
            Pixel.Draw(x, ++y, "Telerik Academy", color);
        }
    }

    class Score : IDraw, IErase
    {
        private int x;
        private int y;
        private int YourScore { get; set; }    

        public Score(Field field): this(field, 0)
        {
        }

        public Score(Field field, int score)
        {
            this.x = field.BottomRightPlayGround.X;
            this.y = field.TopLeftPlayGround.Y;
            this.YourScore = score;
        }

        public void Update(int newscore)
        {
            this.Erase();
            this.YourScore += newscore;
            this.Draw();
        }

        public void Draw()
        {
            Pixel.Draw(x + 4, y + 27, "-------------", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 28, "-Your Score:-", Game.Colors.TextColor);
            Pixel.Draw(x + 4, y + 29, "-------------", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 30, this.YourScore.ToString().PadLeft(13), Game.Colors.Foreground); 
        }

        public void Erase()
        {
            Pixel.Draw(x + 4, y + 30, YourScore.ToString(), ConsoleColor.White);         
        }
    }

    class NextFallingObject : IDraw, IErase
    {
        public FallingObject FallingObject { get; set; }
        private int x;
        private int y;

        public NextFallingObject(Field field, FallingObject fallingObject)
        {
            this.FallingObject = fallingObject;
            this.x = field.BottomRightPlayGround.X;
            this.y = field.TopLeftPlayGround.Y;
        }
        public void Draw()
        {
            Pixel.Draw(x + 4, y + 2, "-------------", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 3, "-Next Symbol-", Game.Colors.TextColor);
            Pixel.Draw(x + 4, y + 4, "-------------", Game.Colors.Foreground);
            this.FallingObject.Draw(x + 10, y + 8);
        }
        public void Erase()
        {
            throw new NotImplementedException();
        }
    }

    class ControlsInfo : IDraw, IErase
    {
        private int x;
        private int y;

        public ControlsInfo(Field field)
        {
            this.x = field.BottomRightPlayGround.X;
            this.y = field.TopLeftPlayGround.Y;
        }
        public void Draw()
        {
            Pixel.Draw(x + 4, y + 14, "-------------", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 15, "--Controls:--", Game.Colors.TextColor);
            Pixel.Draw(x + 4, y + 16, "-------------", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 17, "↑ Rotate", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 18, "↓ Move Down", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 19, "← Move Left ", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 20, "→ Move right", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 21, "Space ►/=", Game.Colors.Foreground);
            Pixel.Draw(x + 4, y + 22, "Esc to Quit", Game.Colors.Foreground);
    
        }

        public void Erase()
        {
            throw new NotImplementedException();
        }
    }
}
