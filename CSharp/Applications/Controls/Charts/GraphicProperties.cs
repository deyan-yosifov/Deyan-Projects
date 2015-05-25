using Deyo.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Deyo.Controls.Charts
{
    public class GraphicProperties : ICloneable<GraphicProperties>
    {
        public GraphicProperties()
        {
            this.Stroke = new SolidColorBrush(Colors.Black);
            this.Fill = new SolidColorBrush(Colors.Black);
            this.Thickness = 1;
            this.IsFilled = true;
            this.IsStroked = true;
        }

        private GraphicProperties(GraphicProperties other)
        {
            this.Stroke = other.Stroke;
            this.Thickness = other.Thickness;
        }

        public bool IsStroked { get; set; }
        public bool IsFilled { get; set; }
        public Brush Stroke { get; set; }
        public Brush Fill { get; set; }
        public double Thickness { get; set; }

        public GraphicProperties Clone()
        {
            return new GraphicProperties(this);
        }
    }
}
