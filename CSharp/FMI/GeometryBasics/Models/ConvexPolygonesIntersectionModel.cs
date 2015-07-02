using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class ConvexPolygonesIntersectionModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за пресичане на множество многоъгълници.";
        private const string description = @"Алгоритъмът ....";

        public ConvexPolygonesIntersectionModel()
            : base(name, description, Activator.CreateInstance<ConvexPolygonesIntersection>)
        {
        }
    }
}
