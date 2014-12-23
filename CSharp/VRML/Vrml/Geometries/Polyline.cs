﻿using System;
using System.Windows.Media.Media3D;
using Vrml.Core;

namespace Vrml.Geometries
{
    public class Polyline
    {
        private readonly Collection<Point3D> points;

        public Polyline()
        {
            this.points = new Collection<Point3D>();
        }

        public Collection<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }
    }
}
