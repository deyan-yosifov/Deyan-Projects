using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    abstract class FallingObject : GameObject, IMovable, ICollide
    {
        protected List<Pixel> relativeBody;

        public List<Pixel> Body { get; protected set; }
        public Point2D Center { get; set; }
        public Rotation Rotation { get; set; }

        public void ReCalculateBody()
        {
            switch (this.Rotation)
            {
                case Rotation.R0:
                    for (int i = 0; i < this.Body.Count; i++)
                    {
                        Body[i].Coordinate.X = Center.X + relativeBody[i].Coordinate.X;
                        Body[i].Coordinate.Y = Center.Y + relativeBody[i].Coordinate.Y;
                        Body[i].Symbol = relativeBody[i].Symbol;
                        Body[i].Color = relativeBody[i].Color;
                    }
                    break;
                case Rotation.R90:
                    for (int i = 0; i < this.Body.Count; i++)
                    {
                        Body[i].Coordinate.X = Center.X + relativeBody[i].Coordinate.Y;
                        Body[i].Coordinate.Y = Center.Y - relativeBody[i].Coordinate.X;
                        Body[i].Symbol = relativeBody[i].Symbol;
                        Body[i].Color = relativeBody[i].Color;
                    }
                    break;
                case Rotation.R180:
                    for (int i = 0; i < this.Body.Count; i++)
                    {
                        Body[i].Coordinate.X = Center.X - relativeBody[i].Coordinate.X;
                        Body[i].Coordinate.Y = Center.Y - relativeBody[i].Coordinate.Y;
                        Body[i].Symbol = relativeBody[i].Symbol;
                        Body[i].Color = relativeBody[i].Color;
                    }
                    break;
                case Rotation.R270:
                    for (int i = 0; i < this.Body.Count; i++)
                    {
                        Body[i].Coordinate.X = Center.X - relativeBody[i].Coordinate.Y;
                        Body[i].Coordinate.Y = Center.Y + relativeBody[i].Coordinate.X;
                        Body[i].Symbol = relativeBody[i].Symbol;
                        Body[i].Color = relativeBody[i].Color;
                    }
                    break;
            }
        }

        public override void Draw()
        {
            foreach (Pixel pixel in this.Body) pixel.Draw();
        }

        public void Draw(int x, int y)
        {
            int oldX = this.Center.X;
            int oldY = this.Center.Y;
            this.Center.X = x;
            this.Center.Y = y;
            this.ReCalculateBody();
            this.Draw();
            this.Center.X = oldX;
            this.Center.Y = oldY;
            this.ReCalculateBody();
        }

        public override void Erase()
        {
            foreach (Pixel pixel in this.Body) pixel.Erase();
        }

        public void Move()
        {
            this.Move(ConsoleKey.DownArrow);
        }

        public void Move(ConsoleKey direction)
        {
            //this.Erase();
            switch (direction)
            {
                case ConsoleKey.LeftArrow:
                    this.Center.X--;
                    break;
                case ConsoleKey.RightArrow:
                    this.Center.X++;
                    break;
                case ConsoleKey.DownArrow:
                    this.Center.Y++;
                    break;
                case ConsoleKey.UpArrow:
                    this.Center.Y--;
                    break;
            }
            this.ReCalculateBody();
            //this.Draw();
        }

        public void Rotate()
        {
            //this.Erase();
            this.Rotation = Engine.NextRotation(this.Rotation);
            this.ReCalculateBody();
            //this.Draw();
        }

        public void RotateBack()
        {
            this.Rotation = Engine.PreviousRotation(this.Rotation);
            this.ReCalculateBody();
        }

        public IEnumerable<CollisionInfo> Collide(Field field)
        {
            List<CollisionInfo> result = new List<CollisionInfo>();
            foreach (Pixel pixel in this.Body)
            {
                if (pixel.Coordinate.X <= field.TopLeftPlayGround.X)
                {
                    result.Add(CollisionInfo.LeftBorder);
                }
                if (pixel.Coordinate.X >= field.BottomRightPlayGround.X)
                {
                    result.Add(CollisionInfo.RightBorder);
                }
                if (pixel.Coordinate.Y <= field.TopLeftPlayGround.Y)
                {
                    result.Add(CollisionInfo.TopBorder);
                }
                if (pixel.Coordinate.Y >= field.BottomRightPlayGround.Y)
                {
                    result.Add(CollisionInfo.BottomBorder);
                }
                if (field.Earth.Contains(pixel))
                {
                    result.Add(CollisionInfo.Earth);
                }
            }
            return result;
        }


    }
}
