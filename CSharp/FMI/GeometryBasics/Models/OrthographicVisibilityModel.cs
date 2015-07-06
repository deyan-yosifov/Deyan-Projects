using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class OrthographicVisibilityModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за видимост при аксонометрична проекция.";
        private const string description = @"Алгоритъмът ....";

        public OrthographicVisibilityModel()
            : base(name, description, Activator.CreateInstance<OrthographicVisibility>)
        {
        }
    }
}
