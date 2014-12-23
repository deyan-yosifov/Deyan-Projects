﻿using System;
using System.Windows.Media.Media3D;

namespace Vrml.Model
{
    public class Viewpoint : IVrmlElement
    {
        public Point3D? Position { get; set; }
        public Orientation Orientation { get; set; }
    }
}
