using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vrml.Editing
{
    public class PropertiesState<T>
        where T : ICopyable<T>
    {
        private readonly Stack<T> propertiesStack;

        public PropertiesState()
        {
            this.propertiesStack = new Stack<T>();
            this.propertiesStack.Push(Activator.CreateInstance<T>());
        }

        public T Properties
        {
            get
            {
                return this.propertiesStack.Peek();
            }
        }

        public IDisposable Save()
        {
            T properties = Activator.CreateInstance<T>();
            properties.CopyFrom(this.propertiesStack.Peek());
            this.propertiesStack.Push(properties);

            return new DisposableAction(this.Restore);
        }

        private void Restore()
        {
            if (this.propertiesStack.Count <= 1)
            {
                throw new InvalidOperationException("Cannot call Restore() method more times compared to Save() method!");
            }

            this.propertiesStack.Pop();
        }
    }
}
