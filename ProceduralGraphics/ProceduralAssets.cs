using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CSCRefactor.ProceduralGraphics
{
    public partial class Shapes
    {
        public void DrawLightningling(Vector2 start, Vector2 end, float[] offsets, float variation, float thickness, Vector2 parentHalfThickness, Color color)
        {
            EnsureStarted();
            int shapeVertexCount = (offsets.Length + 2) * 2;
            int shapeIndexCount = (offsets.Length * 6) + 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector2 direction = (end - start);
            Vector2 segment = direction / (offsets.Length + 1);
            Vector2 outward = new Vector2(-direction.Y, direction.X);
            outward.Normalize();
            Vector2 halfThickness = outward * (thickness * 0.5f);
            outward *= variation;

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + parentHalfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(start - parentHalfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + segment + (offsets[0] * outward) - halfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + segment + (offsets[0] * outward) + halfThickness, 0f), color);

            for (int i = 1; i < offsets.Length; i++)
            {
                indices[indexCount++] = vertexCount;
                indices[indexCount++] = vertexCount + 1;
                indices[indexCount++] = vertexCount + 2;

                indices[indexCount++] = vertexCount;
                indices[indexCount++] = vertexCount + 2;
                indices[indexCount++] = vertexCount + 3;

                vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + (segment * i) + (offsets[i - 1] * outward) + halfThickness, 0f), color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + (segment * i) + (offsets[i - 1] * outward) - halfThickness, 0f), color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + (segment * (i + 1)) + (offsets[i] * outward) - halfThickness, 0f), color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector3(start + (segment * (i + 1)) + (offsets[i] * outward) + halfThickness, 0f), color);
            }

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;

            indices[indexCount++] = vertexCount;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(end - segment + (offsets[offsets.Length - 1] * outward) + halfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(end - segment + (offsets[offsets.Length - 1] * outward) - halfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(end - parentHalfThickness, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(end + parentHalfThickness, 0f), color);

            shapeCount++;
        }
        public class Lightning
        {
            public float X;
            public float Y;
            public Vector2 Start;
            public Vector2 End;
            public float Thickness;
            public float[] Offsets;
            public float Variation;
            Random random = new Random();
            List<Lightningling> lightninglings;
            public RenderTarget2D renderTarget;
            public Texture2D texture;
            public RenderTarget2D fullRenderTarget;

            public Lightning(GraphicsDevice graphicsDevice, Vector2 start, Vector2 end, int points,
                             int lingPoints, float thickness, float variation, float lingVariation, float scale)
            {
                Start = start;
                End = end;
                Thickness = thickness;
                Variation = variation;
                Offsets = new float[points];
                for (int i = 0; i < points; i++)
                {
                    Offsets[i] = (random.NextSingle() * 2) - 1;
                }
                Vector2 direction = (end - start) / scale;
                if (direction.X < 0 && direction.Y > 0) // up-Left
                {
                    renderTarget = new RenderTarget2D(graphicsDevice, (int)(-direction.X + (4 * variation)), (int)(direction.Y + (4 * Variation)));
                    fullRenderTarget = new RenderTarget2D(graphicsDevice, (int)((-direction.X + (4 * variation)) * scale), (int)((direction.Y + (4 * Variation)) * scale));
                    X = start.X + (direction.X - variation * 2) * scale; Y = start.Y - (variation * 2 * scale);
                    start = new Vector2(-direction.X + variation * 2, variation * 2);
                    end = new Vector2(variation * 2, direction.Y + variation * 2);
                }
                else if (direction.X >= 0 && direction.Y > 0) // up-right
                {
                    renderTarget = new RenderTarget2D(graphicsDevice, (int)(direction.X + (4 * variation)), (int)(direction.Y + (4 * Variation)));
                    fullRenderTarget = new RenderTarget2D(graphicsDevice, (int)((direction.X + (4 * variation)) * scale), (int)((direction.Y + (4 * Variation)) * scale));
                    X = start.X - variation * 2 * scale; Y = start.Y - variation * 2 * scale;
                    start = new Vector2(variation * 2, variation * 2);
                    end = new Vector2(direction.X + variation * 2, direction.Y + variation * 2);
                }
                else if (direction.X >= 0 && direction.Y < 0) // down-right
                {
                    renderTarget = new RenderTarget2D(graphicsDevice, (int)(direction.X + (4 * Variation)), (int)(-direction.Y + (4 * Variation)));
                    fullRenderTarget = new RenderTarget2D(graphicsDevice, (int)((direction.X + (4 * Variation)) * scale), (int)((-direction.Y + (4 * Variation)) * scale));
                    X = start.X - variation * 2 * scale; Y = start.Y + (direction.Y - variation * 2) * scale;
                    start = new Vector2(variation * 2, -direction.Y + variation * 2);
                    end = new Vector2(direction.X + variation * 2, variation * 2);
                }
                else // down-left
                {
                    renderTarget = new RenderTarget2D(graphicsDevice, (int)(-direction.X + (4 * Variation)), (int)(-direction.Y + (4 * Variation)));
                    fullRenderTarget = new RenderTarget2D(graphicsDevice, (int)((-direction.X + (4 * Variation)) * scale), (int)((-direction.Y + (4 * Variation)) * scale));
                    X = start.X + (direction.X - variation * 2) * scale; Y = start.Y + (direction.Y - variation * 2) * scale;
                    start = new Vector2(-direction.X + variation * 2, -direction.Y + variation * 2);
                    end = new Vector2(variation * 2, variation * 2);
                }
                Vector2 segment = direction / (Offsets.Length + 1);
                Vector2 outward = new Vector2(-direction.Y, direction.X);
                outward.Normalize();
                Vector2 halfThickness = outward * (thickness * 0.5f);
                outward *= variation;
                lightninglings = new List<Lightningling>
                {
                    new Lightningling(start, start + segment + (outward * Offsets[0]), thickness, lingPoints, lingVariation, random, halfThickness),
                    new Lightningling(start, start + segment + (outward * Offsets[0]), thickness / 3, lingPoints, lingVariation * 2, random, halfThickness),
                    new Lightningling(start, start + segment + (outward * Offsets[0]), thickness / 3, lingPoints, lingVariation * 2, random, halfThickness)
                };
                for (int i = 1; i < points; i++)
                {
                    lightninglings.Add(new Lightningling(start + (segment * i) + (Offsets[i - 1] * outward), start + (segment * (i + 1)) + (Offsets[i] * outward),
                                                         thickness, lingPoints, lingVariation, random, halfThickness));
                    lightninglings.Add(new Lightningling(start + (segment * i) + (Offsets[i - 1] * outward), start + (segment * (i + 1)) + (Offsets[i] * outward),
                                                         thickness / 3, lingPoints, lingVariation * 3, random, halfThickness));
                    if (random.Next(0, 3) == 0)
                        lightninglings.Add(new Lightningling(start + (segment * i) + (Offsets[i - 1] * outward), start + (segment * (i + 1)) + (Offsets[i] * outward),
                                                             thickness / 3, lingPoints, lingVariation * 2, random, halfThickness));
                }
                lightninglings.Add(new Lightningling(end - segment + (outward * Offsets[Offsets.Length - 1]), end,
                                   thickness, lingPoints, lingVariation, random, halfThickness));
            }
            public void Update(float dt)
            {
                for (int i = 0; i < lightninglings.Count; i++) if (lightninglings[i].Update(dt)) { lightninglings.RemoveAt(i); i--; }
            }
            public void Draw(Shapes shape)
            {
                foreach (Lightningling lightningling in lightninglings)
                {
                    lightningling.Draw(shape);
                }
            }
        }

        class Lightningling
        {
            public Vector2 Start;
            public Vector2 End;
            public float Thickness;
            float Variation;
            public float[] Offsets;
            public Vector2 ParentHalfThickness;
            public Lightningling(Vector2 start, Vector2 end, float thickness, int points, float variation, Random random, Vector2 parentHalfThickness)
            {
                Start = start;
                End = end;
                Thickness = thickness;
                Variation = variation;
                Offsets = new float[points];
                ParentHalfThickness = parentHalfThickness;
                for (int i = 0; i < points; i++)
                {
                    Offsets[i] = (random.NextSingle() * 2) - 1;
                }
            }
            public bool Update(float dt)
            {
                Thickness *= 0.05f * dt;
                ParentHalfThickness *= 0.05f * dt;
                if (Thickness < 0) return true;
                return false;
            }
            public void Draw(Shapes shape)
            {
                shape.DrawLightningling(Start, End, Offsets, Variation, Thickness, ParentHalfThickness, Color.White);
            }
        }
    }
}
