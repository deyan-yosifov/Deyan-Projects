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
        private const string description = @"Алгоритъмът следва следните стъпки:
    1. Получава на входа изпъкнал многостен със списък от върхове, ребра и стени.
    2. Започва да рисува всяка от стените като преди това проверява дали стената е видима или не.
    3. Стената е видима ако проекциите върху нормалата на стената на аксонометричния проекционнен вектор и на вектор сочещ към вътрешността на многостена са еднопосочни.
    4. Ако стената е видима всички нейни ръбове трябва да се нарисуват като видими.
    5. Ако стената е невидима всички нейни ръбове, които не са били нарисувани като видими в друга стена, трябва да бъдат нарисувани като невидими.
    6. Алгоритъмът приключва след обхождането на всички стени на многостена.";

        public OrthographicVisibilityModel()
            : base(name, description, Activator.CreateInstance<OrthographicVisibility>)
        {
        }
    }
}
