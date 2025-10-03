using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    public class Particle
    {
        public float X;
        public float Y;
        public float XOffset;
        public float YOffset;
        public float SpeedX;
        public float SpeedY;
        public float Gravity;
        public float Size;
        public float SizeChange;
        public float Life;
        public Color Color;
        public Particle(float x, float y, float speedX, float speedY, float gravity, float size, float sizeChange,
                        float life, float angle, Color color)
        {
            X = x; Y = y;
            XOffset = MathF.Cos(angle);
            YOffset = MathF.Sin(angle);
            SpeedX = speedX; SpeedY = speedY;
            Gravity = gravity;
            Size = size; SizeChange = sizeChange;
            Life = life;
            Color = color;
        }
        public void Update(float deltaTime)
        {
            SpeedY += Gravity * deltaTime;
            X += SpeedX * deltaTime;
            Y += SpeedY * deltaTime;
            Size += SizeChange * deltaTime;
            Life -= deltaTime;
        }
        public bool Death()
        {
            return Life < 0 || Size < 0;
        }
        public void Draw(Shapes _shapes, Camera camera, float scale)
        {
            _shapes.DrawTriangle((X + XOffset * Size + camera.XOffset) / scale, (Y + YOffset * Size + camera.YOffset) / scale,
                                 (X + (XOffset * -0.5f - YOffset * 0.8660254f) * Size + camera.XOffset) / scale,
                                 (Y + (XOffset * 0.8660254f + YOffset * -0.5f) * Size + camera.YOffset) / scale,
                                 (X + (XOffset * -0.5f - YOffset * -0.8660254f) * Size + camera.XOffset) / scale,
                                 (Y + (XOffset * -0.8660254f + YOffset * -0.5f) * Size + camera.YOffset) / scale, Color);
        }
    }
}
