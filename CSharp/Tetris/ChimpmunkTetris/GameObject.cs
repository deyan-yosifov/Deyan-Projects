using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    abstract class GameObject : IDraw, IErase
    {
        public abstract void Draw();

        public abstract void Erase();
    }
}
