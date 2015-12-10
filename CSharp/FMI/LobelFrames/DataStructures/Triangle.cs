using System;

namespace LobelFrames.DataStructures
{
    public class Triangle
    {
        public Edge SideA { get; set; }
        public Edge SideB { get; set; }
        public Edge SideC { get; set; }

        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
    }
}
