using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Core.Common
{
    public class PreservableState<T>
        where T : ICloneable<T>, new()
    {
        private readonly Stack<T> states;

        public PreservableState()
        {
            this.states = new Stack<T>();
            this.states.Push(new T());
        }

        public T Value
        {
            get
            {
                return this.states.Peek();
            }
        }

        public IDisposable Preserve()
        {
            this.states.Push(this.Value.Clone());

            return new DisposableAction(this.Restore);
        }

        public void Restore()
        {
            if (this.states.Count > 1)
            {
                this.states.Pop();
            }
            else
            {
                throw new InvalidOperationException("Cannot call Restore() method more times than Preserve() method!");
            }
        }
    }
}
