using Deyo.Controls.Controls3D;
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
        private readonly MaterialGroup frontMaterialGroup;
        private readonly MaterialGroup backMaterialGroup;
        private readonly MaterialsManager materialsManager;

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
            this.frontMaterialGroup = new MaterialGroup();
            this.backMaterialGroup = new MaterialGroup();
            this.geometryModel.Material = this.frontMaterialGroup;
            this.geometryModel.BackMaterial = this.backMaterialGroup;
            this.materialsManager = new MaterialsManager(this.frontMaterialGroup, this.backMaterialGroup);
        }

        public MaterialsManager MaterialsManager
        {
            get
            {
                return this.materialsManager;
            }
        }
    }
}
