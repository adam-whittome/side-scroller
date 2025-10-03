using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CSCRefactor
{
    public class Camera
    {
        private float halfWidth;
        private float halfHeight;
        public int Height;
        public int Width;
        private float trueXOffset = 0;
        private float trueYOffset = 0;
        private float Lag = 0.006f;
        public float XOffset = 0;
        public float YOffset = 0;
        public Camera(int width, int height)
        {
            halfWidth = width / 2.0f;
            halfHeight = height / 2.0f;
            Height = height;
            Width = width;
            XOffset = 0;
            YOffset = 0;
        }
        public void Update(float deltaTime, float x, float y)
        {
            trueXOffset = -x + halfWidth;
            trueYOffset = -y + halfHeight;
            XOffset += Lag * (trueXOffset - XOffset) * deltaTime;
            YOffset += Lag * (trueYOffset - YOffset) * deltaTime;
        }
        public void Update(float deltaTime, float x, float y, RectangleF bounds)
        {
            trueXOffset = -x + halfWidth;
            trueYOffset = -y + halfHeight;
            if (x - halfWidth < bounds.Left) XOffset += Lag * (-bounds.Left - XOffset) * deltaTime;
            else if (x + halfWidth > bounds.Right) XOffset += Lag * (-bounds.Right + halfWidth * 2 - XOffset) * deltaTime;
            else XOffset += Lag * (trueXOffset - XOffset) * deltaTime;
            if (y - halfHeight < bounds.Bottom) YOffset += Lag * (-bounds.Bottom - YOffset) * deltaTime;
            else if (y + halfHeight > bounds.Top) YOffset += Lag * (-bounds.Top + halfHeight * 2 - YOffset) * deltaTime;
            else YOffset += Lag * (trueYOffset - YOffset) * deltaTime;
        }
        public Vector2 GetSpritePosition(RectangleF rectangle, Frame frame, float scale)
        {
            return new Vector2(rectangle.X + frame.OffsetX * scale + XOffset, Height - frame.Texture.Height * scale - rectangle.Y - YOffset);
        }
    }
}
