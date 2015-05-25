using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Deyo.Controls.Charts
{
    public class ViewportInfo
    {
        public ViewportInfo(Point center, double visibleWidth)
        {
            this.Center = center;
            this.VisibleWidth = visibleWidth;
        }

        public Point Center
        {
            get;
            private set;
        }

        public double VisibleWidth
        {
            get;
            private set;
        }
    }
}
