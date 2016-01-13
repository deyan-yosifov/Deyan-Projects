﻿using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Shapes
{
    public class ShapeFactory : GraphicStateFactory
    {
        internal ShapeFactory(GraphicState graphicState)
            : base(graphicState)
        {
        }

        public Cube CreateCube()
        {
            Cube cube = new Cube();
            this.ApplyMatherials(cube);

            return cube;
        }

        public Cylinder CreateCylinder(bool isClosed = true)
        {
            Cylinder cylinder = new Cylinder(this.GraphicProperties.ArcResolution, isClosed, this.GraphicProperties.IsSmooth);
            this.ApplyMatherials(cylinder);

            return cylinder;
        }

        public Line CreateLine()
        {
            Line line = new Line(this.GraphicProperties.ArcResolution);
            this.ApplyMatherials(line);

            return line;
        }

        public Sphere CreateSphere()
        {
            Sphere sphere = new Sphere(this.GraphicProperties.ArcResolution, this.GraphicProperties.ArcResolution, this.GraphicProperties.IsSmooth);
            this.ApplyMatherials(sphere);

            return sphere;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            this.ApplyMatherials(mesh);

            return mesh;
        }

        private void ApplyMatherials(ShapeBase shape)
        {
            foreach (Material material in this.GraphicProperties.MaterialsManager.FrontMaterials)
            {
                shape.MaterialsManager.AddFrontMaterial(material);
            }

            foreach (Material material in this.GraphicProperties.MaterialsManager.BackMaterials)
            {
                shape.MaterialsManager.AddBackMaterial(material);
            }
        }
    }
}
