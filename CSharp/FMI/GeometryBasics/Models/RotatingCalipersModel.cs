using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class RotatingCalipersModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за шублера за намиране на диаметъра на изпъкнал многоъгълник.";
        private const string description = @"Алгоритъмът ....";

        public RotatingCalipersModel()
            : base(name, description, Activator.CreateInstance<RotatingCalipers>)
        {
        }
    }
}
