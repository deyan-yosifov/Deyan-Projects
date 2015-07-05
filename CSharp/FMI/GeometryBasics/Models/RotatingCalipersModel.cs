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
        private const string description = @"Алгоритъмът следва следните стъпки:
    1. Като вход получава положително ориентиран контур на изпъкнал многоъгълник.
    2. Избира се произволно начална страна за база на шублера.
    3. Началния размер на шублера се намира чрез пресмятане на максимално лицево произведение на избраната основа и друга точка от контура.
    4. След това на всяка стъпка шублера се завърта на по-малкия от двата ъгъла (на основния и срещуположния край).
    5. На всяко положение на шублера се мерят срещуположните диаметри, за да се открие най-голям диаметър.
    6. Алгоритъмът приключва при достигане на завъртане от 180 градуса.";

        public RotatingCalipersModel()
            : base(name, description, Activator.CreateInstance<RotatingCalipers>)
        {
        }
    }
}
