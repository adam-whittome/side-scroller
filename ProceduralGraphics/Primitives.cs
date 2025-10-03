using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor.ProceduralGraphics
{
    public partial class Shapes
    {
        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = x;
            float top = y;
            float right = x + width;
            float bottom = y + height;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, bottom);
            Vector2 d = new Vector2(left, bottom);

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            shapeCount++;
        }

        public void DrawRectangle(RectangleF rectangle, Color color)
        {
            DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, float thickness, Color color)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float halfThickness = thickness / 2;
            Vector2 pointA = new Vector2(x1, y1);
            Vector2 pointB = new Vector2(x2, y2);
            Vector2 toEdge = new Vector2(y2 - y1, x1 - x2);
            toEdge.Normalize();
            toEdge *= halfThickness;
            Vector2 a = pointA + toEdge;
            Vector2 b = pointB + toEdge;
            Vector2 c = pointB - toEdge;
            Vector2 d = pointA - toEdge;

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            shapeCount++;
        }

        public void DrawCircle(float x, float y, float radius, int resolution, Color color)
        {
            EnsureStarted();
            int shapeVertexCount = resolution + 1;
            int shapeIndexCount = resolution * 3;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float deltaAngle = MathHelper.TwoPi / resolution;
            float angle = 0;

            int centreIndex = vertexCount;
            Vector2 centre = new Vector2(x, y);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(centre, 0f), color);
            for (int angleCount = 0; angleCount < resolution; angleCount++)
            {
                Vector2 a = centre + new Vector2(MathF.Sin(angle) * radius, MathF.Cos(angle) * radius);
                angle += deltaAngle;
                Vector2 b = centre + new Vector2(MathF.Sin(angle) * radius, MathF.Cos(angle) * radius);

                indices[indexCount++] = centreIndex;
                indices[indexCount++] = vertexCount;
                indices[indexCount++] = vertexCount + 1;

                vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            }

            shapeCount++;
        }

        public void DrawConvexQuad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, Color color)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector2 a = new Vector2(x1, y1);
            Vector2 b = new Vector2(x2, y2);
            Vector2 c = new Vector2(x3, y3);
            Vector2 d = new Vector2(x4, y4);

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            shapeCount++;
        }

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            EnsureStarted();
            const int shapeVertexCount = 3;
            const int shapeIndexCount = 3;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector2 a = new Vector2(x1, y1);
            Vector2 b = new Vector2(x2, y2);
            Vector2 c = new Vector2(x3, y3);

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);

            shapeCount++;
        }
    }
}
