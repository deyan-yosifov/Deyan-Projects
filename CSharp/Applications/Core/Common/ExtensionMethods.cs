using System;
using System.Collections.Generic;

namespace Deyo.Core.Common
{
    public static class ExtensionMethods
    {
        public static T PopLast<T>(this List<T> list)
        {
            int index = list.Count - 1;
            T element = list[index];
            list.RemoveAt(index);

            return element;
        }

        public static T PeekLast<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static T PeekFromEnd<T>(this List<T> list, int indexFromTheEnd)
        {
            return list[list.Count - 1 - indexFromTheEnd];
        }

        public static T PopFromEnd<T>(this List<T> list, int indexFromEnd)
        {
            int index = list.Count - 1 - indexFromEnd;
            T element = list[index];
            list.RemoveAt(index);

            return element;
        }
    }
}
