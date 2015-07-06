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
        private const string description = @"Алгоритъмът следва следните стъпки:
    1. Взима като входни данни несамопресичащ се многоъгълник и една произволна точка.
    2. Построява се хоризонтален лъч през точката и този лъч се пресича със всяка от страните на многоъгълника.
    3. При всяко пресичане се брои като пресичанията във върхове трябва да се вземат предвид по-внимателно в зависимост от излизащите страни.
    4. Ако пресичанията са нечетен брой => точката е вътрешна.
    5. Ако пресичанията са четен брой => точката е външна.";

        public PointLocalizationModel()
            : base(name, description, Activator.CreateInstance<PointLocalization>)
        {
        }
    }
}
