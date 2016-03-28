using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobelFrames.ViewModels.Commands
{
    internal static class Labels
    {
        public const string Default = "Въведи параметър:";
        public const string InputMoveDistance = "Въведи разстояние и/или направление:";
        public const string WrongInputMoveDistance = "Грешен параметър - трябва да бъде число!";
        public const string PressEscapeToCancel = "Натисни Escape за излизане";
        public const string WrongMousePositioningMoveDirection = "Неясна посока на преместване - моля преместете мишката!";

        public static string GetDecimalNumberValue(double value, int digits = 2)
        {
            return string.Format("{0:0." + new string('0', digits) + "}", value);
        }

    }
}
