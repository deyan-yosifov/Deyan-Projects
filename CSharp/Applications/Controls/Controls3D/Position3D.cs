using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D
{
    public class Position3D
    {
        private Matrix3D matrix;

        public Matrix3D Matrix
        {
            get
            {
                return this.matrix;
            }
            set
            {
                if (this.matrix != value)
                {
                    this.matrix = value;
                    this.OnMatrixChanged();
                }
            }
        }

        public event EventHandler MatrixChanged;

        protected void OnMatrixChanged()
        {
            if (this.MatrixChanged != null)
            {
                this.MatrixChanged(this, new EventArgs());
            }
        }
    }
}
