using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    /// <summary>
    /// Main static game parameters (windowHeight, windowWidth, gameStatus, ...)
    /// </summary>
    public static class Game
    {
        private static int windowHeight;
        private static int windowWidth;

        public static int Height
        {
            get { return windowHeight; }
            set { Console.BufferHeight = Console.WindowHeight = windowHeight = value; }
        }

        public static int Width
        {
            get { return windowWidth; }
            set { Console.BufferWidth = Console.WindowWidth = windowWidth = value; }
        }

        public static Theme Colors { get; set; }

        public static int Speed { get; set; }

        public static GameStatus Status { get; set; }

        static Game()
        {
            Game.Height = 50;
            Game.Width = 35;
            Game.Speed = 100;
            Game.Status = GameStatus.Play;
            Game.Colors = new Theme(GameThemes.White);
        }

        public static void Start()
        {
            Console.CursorVisible = false;
            Console.Title = "TETRIS by Team Chipmunk";
            Console.BackgroundColor = Game.Colors.Background;
            Console.Clear();

            Engine engine = new Engine();
            engine.Field.Draw();
        }

        internal static void StartNewGame(Engine engine)
        {
            engine.ClockTimer.Pause();
            engine.PlayGameTimer.Pause();
            Game.Start();
            engine.KeyPressTimer.Pause();
        }
    }
}
