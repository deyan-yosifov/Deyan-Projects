using Deyo.Controls.Controls3D.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Deyo.Controls.Controls3D.Visuals
{
    public class LineVisual
    {
        private Point3D start;
        private Point3D end;
        private readonly Line line;
        private readonly ModelVisual3D visual;

        public LineVisual(Line line)
        {
            this.line = line;
            this.visual = new ModelVisual3D() { Content = line.GeometryModel };
        }

        internal ModelVisual3D Visual
        {
            get
            {
                return this.visual;
            }
        }

        public Point3D Start
        {
            get
            {
                return this.start;
            }
        }

        public Point3D End
        {
            get
            {
                return this.end;
            }
        }

        public void Move(Point3D start, Point3D end)
        {
            // TODO: Calculate visual transform.
        }
    }
}
