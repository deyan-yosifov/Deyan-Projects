using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class ClippingAlgorithmModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за отрязване на многоъгълник (clipping algorithm).";
        private const string description = @"Алгоритъмът ....";

        public ClippingAlgorithmModel()
            : base(name, description, Activator.CreateInstance<ClippingAlgorithm>)
        {
        }
    }
}
