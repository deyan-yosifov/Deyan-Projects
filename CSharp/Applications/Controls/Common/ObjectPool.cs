using System;
using System.Collections.Generic;

namespace Deyo.Controls.Common
{
    public class ObjectPool<T>
    {
        private readonly Queue<T> pool;
        private readonly Action<T> hideElement;
        private readonly Action<T> showElement;

        public ObjectPool(Action<T> showElement, Action<T> hideElement)
        {
            this.pool = new Queue<T>();
            this.showElement = showElement;
            this.hideElement = hideElement;
        }

        public void AddElementToPool(T element)
        {
            this.hideElement(element);
            this.pool.Enqueue(element);
        }

        public bool TryGetElementFromPool(out T element)
        {
            if (this.pool.Count > 0)
            {
                element = this.pool.Dequeue();
                this.showElement(element);

                return true;
            }

            element = default(T);

            return false;
        }
    }
}
