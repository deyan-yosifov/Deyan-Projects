using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D
{
    public class MaterialsManager
    {
        private readonly IMaterialsOwner materialsOwner;

        public MaterialsManager(IMaterialsOwner materialsOwner)
        {
            this.materialsOwner = materialsOwner;
        }

        public void AddFrontMaterial(Material material)
        {
            this.materialsOwner.FrontMaterials.Children.Add(material);
        }

        public void AddBackMaterial(Material material)
        {
            this.materialsOwner.BackMaterials.Children.Add(material);
        }

        public void AddFrontDiffuseMaterial(Color color)
        {
            this.AddFrontMaterial(CreateDiffuseMaterial(color));
        }

        public void AddBackDiffuseMaterial(Color color)
        {
            this.AddBackMaterial(new DiffuseMaterial(new SolidColorBrush(color)));
        }

        public void AddFrontTexture(ImageSource image)
        {
            this.AddFrontMaterial(CreateDiffuseMaterial(image));
        }

        public void AddBackTexture(ImageSource image)
        {
            this.AddBackMaterial(CreateDiffuseMaterial(image));
        }

        public static DiffuseMaterial CreateDiffuseMaterial(Color color)
        {
            return new DiffuseMaterial(new SolidColorBrush(color));
        }

        public static DiffuseMaterial CreateDiffuseMaterial(ImageSource image)
        {
            return new DiffuseMaterial(new ImageBrush(image) { ViewportUnits = BrushMappingMode.Absolute });
        }
    }
}
