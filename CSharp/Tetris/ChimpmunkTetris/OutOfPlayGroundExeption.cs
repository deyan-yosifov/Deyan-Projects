using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChimpmunkTetris
{
    class OutOfPlayGroundExeption : ApplicationException
    {
        public Point2D TopLeft { get; private set; }
        public Point2D BottomRight { get; private set; }

        public OutOfPlayGroundExeption(Field field, string message)
            : this(field, message, null) { }

        public OutOfPlayGroundExeption(Field field)
            : this(field, String.Format("An object was drawn outside field boundary!"), null) { }

        public OutOfPlayGroundExeption(Field field, string message, Exception innerException)
            : base(message, innerException) 
        {
            TopLeft = field.TopLeftPlayGround;
            BottomRight = field.BottomRightPlayGround;
        } 
    }
}
