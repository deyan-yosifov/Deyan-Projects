using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrml.Core
{
    public class Collection<T> : IEnumerable<T>
    {
        private readonly List<T> list;

        public Collection()
        {
            this.list = new List<T>();
        }

        public IEnumerable<T> Items
        {
            get
            {
                foreach (T item in this.list)
                {
                    yield return item;
                }
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }


        public void Add(T item)
        {
            this.list.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}
