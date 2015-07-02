using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class PerspectiveVisibilityModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за видимост при перспективна проекция.";
        private const string description = @"Алгоритъмът ....";

        public PerspectiveVisibilityModel()
            : base(name, description, Activator.CreateInstance<PerspectiveVisibility>)
        {
        }
    }
}
