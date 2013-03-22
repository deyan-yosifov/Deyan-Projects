using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    interface IMovable
    {
        void Move(ConsoleKey direction);
        void Rotate();
    }
}
