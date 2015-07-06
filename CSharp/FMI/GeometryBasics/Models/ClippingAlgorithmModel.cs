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
        private const string description = @"Алгоритъмът на Sutherland–Hodgman следва следните стъпки:
    1. На входа получаваме изпъкнал многоъгълник, който ще използваме за клипирането.
    2. На входа получаваме и друг произволен многоъгълник, който ще клипираме.
    3. За всяка страна от клипиращия многоъгълник изрязваме точките, които са от външната полуравнина спрямо клипирането.
    4. Накрая остава само частта, която е вътрешна за клипиращия многоъгълник.";

        public ClippingAlgorithmModel()
            : base(name, description, Activator.CreateInstance<ClippingAlgorithm>)
        {
        }
    }
}
