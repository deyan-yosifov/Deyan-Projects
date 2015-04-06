using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deyo.Vrml.Editing
{
    public interface ICopyable<T>
    {
        void CopyFrom(T other);
    }
}
