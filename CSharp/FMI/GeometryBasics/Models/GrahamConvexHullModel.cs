using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class GrahamConvexHullModel : ExampleModelBase
    {
        private const string name = "Алгоритъм на Graham за изчисляване на изпъкнала обвивка.";
        private const string description = @"Алгоритъмът следва следните стъпки:
    1. Избира екстремална точка от множеството - най-лявата най-долна точка от множеството.
    2. Сортира другите точки по ъгъл на предшествие спрямо хоризонталата през избраната екстремална точка.
    3. Добавя сортираните точки последователно в стек.
    4. На всяка добавена точка се проверява дали получената тройка от последните точки в стека е положително ориентирана.
    5. Ако последната тройка не е положително ориентирана се премахва средната точка от трите и се продължава проверката назад, докато не се стигне до положително ориентирана тройка или докато останат по-малко от 3 точки в стека.
    6. При приключване на алгоритъма получаваме положително ориентиран контур на изпъкналата обвивка на множеството.";

        public GrahamConvexHullModel()
            : base(name, description, Activator.CreateInstance<GrahamConvexHull>)
        {
        }
    }
}
