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

        public Position3D(Matrix3D matrix)
        {
            this.matrix = matrix;
        }

        public Matrix3D Matrix
        {
            get
            {
                return this.matrix;
            }
        }

        public void Translate(Vector3D offset)
        {
            this.matrix.Translate(offset);
        }

        public void Rotate(Quaternion quaternion)
        {
            this.matrix.Rotate(quaternion);
        }

        public void Scale(Vector3D scale)
        {
            this.matrix.Scale(scale);
        }

        public void Append(Matrix3D matrix)
        {
            matrix.Append(matrix);
        }
    }
}
