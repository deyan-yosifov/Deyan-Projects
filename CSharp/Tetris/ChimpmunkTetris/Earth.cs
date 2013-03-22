using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChimpmunkTetris
{
    /// <summary>
    /// This is the Earth object of the tetris game (the object collecting the falling objects);
    /// </summary>
    class Earth : GameObject
    {
        private HashSet<Pixel> body;
        public Point2D topLeftPlayGround;
        public Point2D bottomRightPlayGround;
        private int[] levelElementsCount;

        public Earth(Point2D topLeftPlayGround, Point2D bottomRightPlayGround)
        {
            this.body = new HashSet<Pixel>();
            this.topLeftPlayGround = topLeftPlayGround;
            this.bottomRightPlayGround = bottomRightPlayGround;
            this.levelElementsCount = new int[this.bottomRightPlayGround.Y];
        }

        public bool Contains(Pixel pixel)
        {
            return this.body.Contains(pixel);
        }

        public void Add(FallingObject fallingObject)
        {
            foreach (Pixel pixel in fallingObject.Body)
            {
                this.body.Add(pixel);
                levelElementsCount[pixel.Coordinate.Y]++;
            }
        }

        public int UpdateBody()
        {            
            int rowsCollapsed = 0;
            int[] levelsToFallDown = new int[levelElementsCount.Length];
            for (int i = this.bottomRightPlayGround.Y - 1; i > this.topLeftPlayGround.Y; i--)
            {
                levelsToFallDown[i] = rowsCollapsed;
                if (levelElementsCount[i] == bottomRightPlayGround.X - topLeftPlayGround.X - 1)
                {
                    rowsCollapsed++;
                    EraseRow(i);                  
                }
            }
            if (rowsCollapsed > 0)
            {
                HashSet<Pixel> newBody = new HashSet<Pixel>();
                this.levelElementsCount = new int[levelElementsCount.Length];
                foreach (Pixel pixel in this.body)
                {
                    newBody.Add(new Pixel(pixel.Coordinate.X, pixel.Coordinate.Y + levelsToFallDown[pixel.Coordinate.Y],
                        pixel.Symbol, pixel.Color));
                    levelElementsCount[pixel.Coordinate.Y + levelsToFallDown[pixel.Coordinate.Y]]++;
                }
                this.body = newBody;
            }
            return rowsCollapsed;
        }

        private void EraseRow(int i)
        {
            Pixel.Draw(topLeftPlayGround.X + 1, i, new String('*', levelElementsCount[i]), ConsoleColor.Red);
            Thread.Sleep(Game.Speed);
            Pixel.Draw(topLeftPlayGround.X + 1, i, new String('*', levelElementsCount[i]), ConsoleColor.Green);
            Thread.Sleep(Game.Speed);
            Pixel.Draw(topLeftPlayGround.X + 1, i, new String('*', levelElementsCount[i]), ConsoleColor.Cyan);
            Thread.Sleep(Game.Speed);
            Pixel.Draw(topLeftPlayGround.X + 1, i, new String('*', levelElementsCount[i]), ConsoleColor.Red);
            Thread.Sleep(Game.Speed);

            Pixel.Erase(topLeftPlayGround.X + 1, i, new String(' ', levelElementsCount[i]));
            for (int x = topLeftPlayGround.X + 1; x < bottomRightPlayGround.X; x++)
            {
                this.body.Remove(new Pixel(x, i, ' ', ConsoleColor.Gray));
            }  
        }

        public override void Draw()
        {
            foreach (Pixel pixel in this.body) pixel.Draw();
        }

        public override void Erase()
        {
            foreach (Pixel pixel in this.body) pixel.Erase();
        }
    }
}
