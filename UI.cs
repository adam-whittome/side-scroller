using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    enum TextAlignment
    {
        Left,
        Right,
        Center
    }

    class Button
    {
        RectangleF rectangle;
        private string Text;
        TextAlignment textAlignment;
        public Button()
        {

        }
    }

    class Label
    {
        RectangleF textRectangle;
        private string Text;
        TextAlignment textAlignment;
    }
}
