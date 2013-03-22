using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    public class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int x, int y)
           // : this()
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2D)
            {
                Point2D other = obj as Point2D;
                if (this.X != other.X) return false;
                else if (this.Y != other.Y) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int prime = 83;
            int result = 1;
            unchecked
            {
                result = result * prime + this.X.GetHashCode();
                result = result * prime + this.Y.GetHashCode();
            }
            return result;
        }

        public override string ToString()
        {
            return String.Format("Point2D: ({0}, {1})", X, Y);
        }
    }
}
