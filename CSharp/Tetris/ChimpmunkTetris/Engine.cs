using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChimpmunkTetris
{
    class Engine
    {
        private Random rand = new Random();
        private GameStatus gameStatus = GameStatus.Welcome;
        private bool changedStatus = true;

        private ConsoleKey key = ConsoleKey.Spacebar;
        public Timer ClockTimer { get; set; }
        public Timer KeyPressTimer { get; set; }
        public Timer PlayGameTimer { get; set; }
        public Field Field { get; set; }

        public Engine()
            : this(new Field()) { }

        public Engine(Field field)
        {
            this.Field = field;

            this.ClockTimer = new Timer(1000);
            this.KeyPressTimer = new Timer(10);
            this.PlayGameTimer = new Timer(Game.Speed);

            this.Field.FallingObject = GetRandomObject();
            this.Field.NextFallingObject = new NextFallingObject(this.Field, GetRandomObject());

            this.KeyPressTimer.TimerChanged += new EventHandler(OnKeyPress);
            this.KeyPressTimer.Play();            
            this.PlayGameTimer.TimerChanged += new EventHandler(OnGameMoveOn);
            this.PlayGameTimer.Play();

            this.ClockTimer.TimerChanged += new EventHandler(OnClockTimer);
        }

        private void OnClockTimer(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            this.Field.Clock.AddSeconds(1);
        }

        private void OnGameMoveOn(object sender, EventArgs e)
        {
            if (this.gameStatus == GameStatus.Play)
            {
                if (this.changedStatus)
                {
                    this.Field.Erase();
                    this.Field.Draw();
                    this.changedStatus = false;
                }
                this.Field.Clock.Update();
                this.CheckKey();
                this.Field.FallingObject.Erase();
                this.Field.FallingObject.Move();
                HandleCollisions(this.Field.FallingObject.Collide(this.Field), ConsoleKey.DownArrow);
                this.Field.FallingObject.Draw();
            }
            else if (this.gameStatus == GameStatus.Pause)
            {
                this.Field.PauseDraw(Game.Colors.TextColor, Game.Colors.Foreground);
            }
            else if (this.gameStatus == GameStatus.Welcome)
            {
                this.Field.WelcomeDraw(Game.Colors.TextColor, Game.Colors.Foreground);
            }
            else if (this.gameStatus == GameStatus.GameOver)
            {
                this.Field.GameOverDraw(Game.Colors.TextColor, Game.Colors.Foreground);
                
            }
            else if (this.gameStatus == GameStatus.TopScores)
            {

            }
        }

        internal void QuitGame()
        {            
            this.ClockTimer.Pause();
            this.PlayGameTimer.Pause();
            this.KeyPressTimer.Pause();
        }        

        private void HandleCollisions(IEnumerable<CollisionInfo> collisions, ConsoleKey key)
        {
            if (collisions.Count<CollisionInfo>() > 0)
            {
                if (key == ConsoleKey.DownArrow)//this collision happened after Move(down)
                {
                    this.Field.FallingObject.Move(ConsoleKey.UpArrow);
                    if (this.Field.FallingObject.Collide(this.Field).Count<CollisionInfo>() == 0)
                    {                        
                        this.Field.Earth.Add(this.Field.FallingObject);
                        this.Field.Earth.Draw();
                        this.Field.Score.Update(10 + 100 * this.Field.Earth.UpdateBody());
                        this.Field.FallingObject = this.Field.NextFallingObject.FallingObject;
                        this.Field.NextFallingObject.FallingObject = GetRandomObject();
                        this.Field.Erase();
                        this.Field.Draw();
                    }
                    else
                    {
                        this.gameStatus = GameStatus.GameOver;
                    }
                }
                else if (key == ConsoleKey.UpArrow)//this collision happened after Rotate()
                {
                    this.Field.FallingObject.RotateBack();
                }
                else if (key == ConsoleKey.LeftArrow)//this collision happened after Move(left)
                {
                    this.Field.FallingObject.Move(ConsoleKey.RightArrow);
                }
                else if (key == ConsoleKey.RightArrow)//this collision happened after Move(right)
                {
                    this.Field.FallingObject.Move(ConsoleKey.LeftArrow);
                }
            }
            this.Field.Clock.Update();
        }

        private void CheckKey()
        {
            if (this.key != ConsoleKey.Spacebar && this.key != ConsoleKey.Escape)
            {
                this.Field.FallingObject.Erase();
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        this.Field.FallingObject.Rotate();
                        break;                    
                    default:
                        this.Field.FallingObject.Move(key);
                        break;                        
                }
                HandleCollisions(this.Field.FallingObject.Collide(this.Field), key);
                this.Field.FallingObject.Draw();
                this.key = ConsoleKey.Spacebar;
            }
        }

        private void OnKeyPress(object sender, EventArgs e)
        {
            this.key = Console.ReadKey(true).Key;
            if (this.key == ConsoleKey.Escape && this.gameStatus != GameStatus.Welcome) QuitGame();
            switch(this.gameStatus)
            {
                case GameStatus.Play:
                case GameStatus.Pause:
                    if (this.key == ConsoleKey.Spacebar)
                    {
                        this.ClockTimer.PauseOrPlay();
                        //this.PlayGameTimer.PauseOrPlay();
                        if (this.gameStatus == GameStatus.Pause)
                        {                            
                            this.gameStatus = GameStatus.Play;
                            this.changedStatus = true;
                        }
                        else if (this.gameStatus == GameStatus.Play)
                        {
                            this.gameStatus = GameStatus.Pause;
                        }
                    }
                    break;

                case GameStatus.Welcome:
                    this.gameStatus = GameStatus.Play;
                    this.ClockTimer.PauseOrPlay();
                    this.changedStatus = true;
                    break;
                case GameStatus.GameOver:
                    if (this.key == ConsoleKey.Spacebar)
                    {
                        Game.StartNewGame(this);
                    }
                    break;
                case GameStatus.TopScores:

                    break;
            }
        }

        public static Rotation NextRotation(Rotation rotation)
        {
            return (Rotation)(((int)(++rotation)) % 4);
        }

        public static Rotation PreviousRotation(Rotation rotation)
        {
            return (Rotation)(((int)(--rotation)+4) % 4);
        }

        public FallingObject GetRandomObject()
        {
            int startX = (this.Field.TopLeftPlayGround.X + this.Field.BottomRightPlayGround.X)/2;
            int startY = this.Field.TopLeftPlayGround.Y + 1;
            FallingObject result;
            switch (rand.Next(4))
            {
                case 0:
                    result = new FallingL(new Point2D(startX, startY), Rotation.R270, '@',
                        Game.Colors.Colors[rand.Next(Game.Colors.Colors.Count)]);
                    break;
                case 1:
                    result = new FallingSquare(new Point2D(startX, startY), Rotation.R0, '@',
                        Game.Colors.Colors[rand.Next(Game.Colors.Colors.Count)]);
                    break;
                case 2:
                    result = new FallingLine(new Point2D(startX, startY), Rotation.R0, '@',
                        Game.Colors.Colors[rand.Next(Game.Colors.Colors.Count)], rand.Next(5) + 1);
                    break;
                default:
                    result = new FallingZ(new Point2D(startX, startY), Rotation.R0, '@',
                        Game.Colors.Colors[rand.Next(Game.Colors.Colors.Count)]);
                    break;
            }
            return result;
        }
    }
}
