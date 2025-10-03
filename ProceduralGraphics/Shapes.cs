using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor.ProceduralGraphics
{
    public partial class Shapes : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private BasicEffect effect;
        private RasterizerState rasterizerState;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int vertexCount;
        private int indexCount;
        private int shapeCount;

        private bool isStarted = false;

        public Shapes(Game game)
        {
            isDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");

            effect = new BasicEffect(game.GraphicsDevice);
            effect.TextureEnabled = false;
            effect.FogEnabled = false;
            effect.LightingEnabled = false;
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.Identity;
            effect.VertexColorEnabled = true;

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            game.GraphicsDevice.RasterizerState = rasterizerState;

            const int MaxVertexCount = 2048;
            const int MaxIndexCount = MaxVertexCount * 3;

            vertices = new VertexPositionColor[MaxVertexCount];
            indices = new int[MaxIndexCount];

            vertexCount = 0;
            indexCount = 0;
            shapeCount = 0;

            this.isStarted = false;
        }
        public void Dispose()
        {
            if (isDisposed) return;
            effect?.Dispose();
            isDisposed = true;
        }
        public void EnsureStarted()
        {
            if (!isStarted) throw new Exception("batching was not started");
        }
        public void EnsureSpace(int vertexCount, int indexCount)
        {
            if (vertexCount > vertices.Length)
                throw new Exception($"maximum shape vertex count is {vertices.Length}");
            if (indexCount > indices.Length)
                throw new Exception($"maximum shape index count is {indices.Length}");
            if (this.vertexCount + vertexCount > vertices.Length || this.indexCount + indexCount > indices.Length)
                Flush();
        }
        public void Begin()
        {
            if (isStarted) throw new Exception("batching has already started");
            game.GraphicsDevice.RasterizerState = rasterizerState;
            Viewport vp = game.GraphicsDevice.Viewport;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);

            this.isStarted = true;
        }
        public void BeginRenderTarget(ref RenderTarget2D renderTarget)
        {
            if (isStarted) throw new Exception("batching has already started");
            game.GraphicsDevice.SetRenderTarget(renderTarget);
            renderTarget.GraphicsDevice.RasterizerState = rasterizerState;
            Viewport vp = renderTarget.GraphicsDevice.Viewport;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
            renderTarget.GraphicsDevice.Clear(Color.Transparent);

            this.isStarted = true;
        }
        public void End()
        {
            EnsureStarted();
            Flush();
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            game.GraphicsDevice.SetRenderTarget(null);
            this.isStarted = false;
        }
        public void Flush()
        {
            if (this.shapeCount == 0) return;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    vertices, 0, vertexCount,
                    indices, 0, indexCount / 3);
            }
            vertexCount = 0;
            indexCount = 0;
            shapeCount = 0;
        }
    }
}
