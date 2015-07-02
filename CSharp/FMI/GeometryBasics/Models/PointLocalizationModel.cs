using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class PointLocalizationModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за локализиране на точка в многоъгълник.";
        private const string description = @"Алгоритъмът ....";

        public PointLocalizationModel()
            : base(name, description, Activator.CreateInstance<PointLocalization>)
        {
        }
    }
}
