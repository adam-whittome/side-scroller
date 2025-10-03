using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    public class RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Left { get { return X; } set { X = value; } }
        public float Right { get { return X + Width; } set { X = value - Width; } }
        public float Top { get { return Y + Height; } set { Y = value - Height; } }
        public float Bottom { get { return Y; } set { Y = value; } }
        public float CenterX { get { return X + (Width / 2); } }
        public float CenterY { get { return Y + (Height / 2); } }
        public Vector2 Center { get { return new Vector2(CenterX, CenterY); } }
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public bool Intersects(RectangleF rectangle)
        {
            return !(Right <= rectangle.Left || rectangle.Right <= Left ||
                     Top <= rectangle.Bottom || rectangle.Top <= Bottom);
        }
        public RectangleF Transform(float x, float y)
        {
            return new RectangleF(X + x, Y + y, Width, Height);
        }
        public void Draw(Shapes _shapes)
        {
            _shapes.DrawRectangle(X, Y, Width, Height, Color.White);
        }
        public void Draw(Shapes _shapes, Camera camera)
        {
            Transform(camera.XOffset, camera.YOffset).Draw(_shapes);
        }
    }
}
