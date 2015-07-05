using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryBasics.Models
{
    public class LineSegmentsIntersectionModel : ExampleModelBase
    {
        private const string name = "Алгоритъм за пресичане на множество отсечки.";
        private const string description = @"Алгоритъмът следва следните стъпки:
    1. Сортира крайщата на отсечките по височина.
    2. Започвайки да ""мете"" с хоризонтална права отдолу-нагоре се добавят нови точки от крайщата на отсечките.
    3. Добавените точки се пазят сортирани по хоризонтално съседство така, че на всяка стъпка да се търсят пресичанията само на съседни отсечки от новодобавените точки.
    4. Намерените пресечници също се добавят към сортирания вертикален списък от точки.
    5. При достигане на междинна точка от дадена отсечка се премахва от списъка на съседите точката от същата отсечка, която имат по-малка вертикална координата.
    6. Алгоритъмът приключва при достигане на най-високата точка от ""метенето"".";

        public LineSegmentsIntersectionModel()
            : base(name, description, Activator.CreateInstance<LineSegmentsIntersection>)
        {
        }
    }
}
