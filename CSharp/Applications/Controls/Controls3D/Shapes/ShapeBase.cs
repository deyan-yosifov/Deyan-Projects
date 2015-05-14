using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Contols3D.Shapes
{
    public abstract class ShapeBase
    {
        private readonly GeometryModel3D geometryModel;
        private readonly MaterialGroup materialGroup;
        private readonly MaterialGroup backMaterialGroup;

        protected internal GeometryModel3D GeometryModel
        {
            get
            {
                return this.geometryModel;
            }
        }

        protected ShapeBase()
        {
            this.geometryModel = new GeometryModel3D();
            this.materialGroup = new MaterialGroup();
            this.backMaterialGroup = new MaterialGroup();
            this.geometryModel.Material = this.materialGroup;            
        }

        public void AddMaterial(Material material)
        {
            this.materialGroup.Children.Add(material);
        }

        public void AddBackMaterial(Material material)
        {
            if (this.geometryModel.BackMaterial == null)
            {
                this.geometryModel.BackMaterial = this.backMaterialGroup;
            }

            this.backMaterialGroup.Children.Add(material);
        }

        public void AddDiffuseMaterial(Color color)
        {
            this.AddMaterial(CreateDiffuseMaterial(color));
        }

        public void AddBackDiffuseMaterial(Color color)
        {
            this.AddBackMaterial(new DiffuseMaterial(new SolidColorBrush(color)));
        }

        public void AddTexture(ImageSource image)
        {
            this.AddMaterial(CreateDiffuseMaterial(image));
        }

        public void AddBackTexture(ImageSource image)
        {
            this.AddBackMaterial(CreateDiffuseMaterial(image));
        }

        private static DiffuseMaterial CreateDiffuseMaterial(Color color)
        {
            return new DiffuseMaterial(new SolidColorBrush(color));
        }

        private static DiffuseMaterial CreateDiffuseMaterial(ImageSource image)
        {
            return new DiffuseMaterial(new ImageBrush(image) { ViewportUnits = BrushMappingMode.Absolute });
        }
    }
}
