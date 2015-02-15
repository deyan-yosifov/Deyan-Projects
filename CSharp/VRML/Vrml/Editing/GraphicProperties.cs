using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Vrml.Model;

namespace Vrml.Editing
{
    public class GraphicProperties : ICopyable<GraphicProperties>
    {
        public GraphicProperties()
        {
            this.StrokeColor = new VrmlColor(Colors.Black);
            this.StrokeThickness = 1;
            this.SmoothnessResolution = 8;
            this.IsSmooth = true;
        }

        public VrmlColor StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public int SmoothnessResolution { get; set; }
        public bool IsSmooth { get; set; }

        public void CopyFrom(GraphicProperties other)
        {
            this.StrokeColor = other.StrokeColor;
            this.StrokeThickness = other.StrokeThickness;
            this.SmoothnessResolution = other.SmoothnessResolution;
            this.IsSmooth = other.IsSmooth;
        }
    }
}
