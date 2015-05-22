using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D
{
    public class GraphicProperties : ICloneable<GraphicProperties>, IMaterialsOwner
    {
        private readonly MaterialGroup frontMaterials;
        private readonly MaterialGroup backMaterials;
        private readonly MaterialsManager materialsManager;

        public GraphicProperties()
        {
            this.frontMaterials = new MaterialGroup();
            this.backMaterials = new MaterialGroup();
            this.materialsManager = new MaterialsManager(this);
            this.Thickness = 1;
            this.ArcResolution = 10;
            this.IsSmooth = true;
        }

        private GraphicProperties(GraphicProperties other)
            : this()
        {
            this.Thickness = other.Thickness;

            foreach(Material material in other.FrontMaterials.Children)
            {
                this.FrontMaterials.Children.Add(material);
            }

            foreach (Material material in other.BackMaterials.Children)
            {
                this.BackMaterials.Children.Add(material);
            }
        }

        public MaterialGroup FrontMaterials
        {
            get
            {
                return this.frontMaterials;
            }
        }

        public MaterialGroup BackMaterials
        {
            get
            {
                return this.backMaterials;
            }
        }

        public MaterialsManager MaterialsManager
        {
            get
            {
                return this.materialsManager;
            }
        }

        public double Thickness { get; set; }

        public int ArcResolution { get; set; }

        public bool IsSmooth { get; set; }

        public GraphicProperties Clone()
        {
            return new GraphicProperties(this);
        }
    }
}
