using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    class FallingSquare : FallingObject
    {
        public FallingSquare(Point2D center, Rotation rotation, char symbol, ConsoleColor color)
        {
            this.Center = center;
            this.Rotation = rotation;
            this.relativeBody = new List<Pixel>();

            //Here should come the body definitions in relative to center coordinates for creating new shape!
            relativeBody.Add(new Pixel(0, 0, symbol, color));
            relativeBody.Add(new Pixel(1, 0, symbol, color));
            relativeBody.Add(new Pixel(0, 1, symbol, color));
            relativeBody.Add(new Pixel(1, 1, symbol, color));

            this.Body = new List<Pixel>();
            for(int i = 0; i < relativeBody.Count; i++) Body.Add(new Pixel(0,0,'@', ConsoleColor.Gray));
            this.ReCalculateBody();
        }
    }
}
