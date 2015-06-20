using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAGD
{
    public static class ExtensionMethods
    {
        public static T RemoveLast<T>(this List<T> list)
        {
            int index = list.Count - 1;
            T element = list[index];
            list.RemoveAt(index);

            return element;
        }
    }
}
