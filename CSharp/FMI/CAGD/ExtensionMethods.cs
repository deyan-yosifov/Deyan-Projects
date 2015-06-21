using Deyo.Controls.Controls3D;
using Deyo.Controls.Controls3D.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
