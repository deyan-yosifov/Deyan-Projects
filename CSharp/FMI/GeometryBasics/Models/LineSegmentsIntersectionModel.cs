using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class LineSegmentsIntersectionModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за пресичане на множество отсечки.";
        private const string description = @"Алгоритъмът ....";

        public LineSegmentsIntersectionModel()
            : base(name, description, Activator.CreateInstance<LineSegmentsIntersection>)
        {
        }
    }
}
