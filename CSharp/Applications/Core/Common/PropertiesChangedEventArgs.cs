using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Core.Common
{
    public class PropertiesChangedEventArgs : EventArgs
    {
        public PropertiesChangedEventArgs(IEnumerable<string> propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public IEnumerable<string> PropertyNames
        {
            get;
            private set;
        }
    }
}
