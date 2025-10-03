using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor.ProceduralGraphics
{
    public class Grass
    {
        private float x;
        private float y;
        private float width;
        private float height;
        public float angle;
        Color color;
        public Grass(float x, float y, float width, float height, float angle, Color color)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.angle = angle;
            this.color = color;
        }
        public void Draw(Shapes shapes, float scale)
        {
            (float x, float y) = (this.x / scale, this.y / scale);
            shapes.DrawGrass(x, y, width, height, angle, color);
        }
        public void Update(float elapsed)
        {
            angle = MathF.Sin(x * 0.1f + elapsed * 0.004f) * 0.07f + MathF.PI / 2;
        }
        public void Push(Player player, RectangleF bounds)
        {
            float distanceX = player.Rectangle.CenterX - (x + bounds.X);
            if (-200 > distanceX || distanceX > 200) return;
            float distanceY = player.Rectangle.Bottom - (y + bounds.Y);
            angle -= 1500 * player.SpeedX / ((distanceX * distanceX + 3 * distanceY * distanceY) + player.SpeedY * player.SpeedY * 10000);
        }
    }

    public partial class Shapes : IDisposable
    {
        public void DrawGrass(float x, float y, float width, float height, float angle, Color color)
        {
            EnsureStarted();
            int shapeVertexCount = 4;
            int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            Vector2 outward = new Vector2(-direction.Y, direction.X);

            direction *= height;
            outward *= width * 0.5f;

            Vector2 bottom = new Vector2(x, y);
            Vector2 left = bottom + direction * 0.4f - outward;
            Vector2 right = bottom + direction * 0.4f + outward;
            Vector2 top = bottom + direction;

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(left, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(right, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(top, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(bottom, 0f), color);

            shapeCount++;
        }
    }
}
