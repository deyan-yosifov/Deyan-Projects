using System;
using System.Windows;

namespace GeometryBasics.Models
{
    public class LineSegment
    {
        private readonly Point start;
        private readonly Point end;

        public LineSegment(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        public Point Start
        {
            get
            {
                return this.start;
            }
        }

        public Point End
        {
            get
            {
                return this.end;
            }
        }
    }
}
