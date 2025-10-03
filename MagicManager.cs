using Computer_Science_Coursework;
using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    public class MagicManager
    {
        private float scale;
        private RenderTarget2D magicCanvas;
        public Texture2D bloom;
        private RenderTarget2D enemyMagicCanvas;
        private Texture2D enemyBloom;
        public List<Projectile> Projectiles = new List<Projectile>();
        public List<Projectile> EnemyProjectiles = new List<Projectile>();
        public List<Particle> Particles = new List<Particle>();
        public List<Particle> EnemyParticles = new List<Particle>();
        private int width, height;
        private BlendState blend = new BlendState
        {

            ColorBlendFunction = BlendFunction.Max,
            ColorSourceBlend = Blend.SourceColor,
            ColorDestinationBlend = Blend.DestinationColor,

            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.Zero,

        };
        public MagicManager(GraphicsDevice graphicsDevice, int width, int height, float scale)
        {
            this.scale = scale;
            this.magicCanvas = new RenderTarget2D(graphicsDevice, (int)(width / scale), (int)(height / scale));
            this.bloom = new Texture2D(graphicsDevice, width, height);
            this.enemyMagicCanvas = new RenderTarget2D(graphicsDevice, (int)(width / scale), (int)(height / scale));
            this.enemyBloom = new Texture2D(graphicsDevice, width, height);
            this.width = width;
            this.height = height;
        }
        public void Update(float deltaTime, List<RectangleF> colliders, RectangleF bounds)
        {
            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectiles[i].Update(deltaTime, colliders, ref Particles);
                if (Projectiles[i].Death(bounds)) { Projectiles.RemoveAt(i); i--; }
            }
            for (int i = 0; i < EnemyProjectiles.Count; i++)
            {
                EnemyProjectiles[i].Update(deltaTime, colliders, ref EnemyParticles);
                if (EnemyProjectiles[i].Death(bounds)) { EnemyProjectiles.RemoveAt(i); i--; }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update(deltaTime);
                if (Particles[i].Death()) { Particles.RemoveAt(i); i--; }
            }
            for (int i = 0; i < EnemyParticles.Count; i++)
            {
                EnemyParticles[i].Update(deltaTime);
                if (EnemyParticles[i].Death()) { EnemyParticles.RemoveAt(i); i--; }
            }
        }
        public void FireExplosion(float x, float y, float speed, float size, float sizeChange, int intensity)
        {
            for (int i = 0; i < intensity; i++)
            {
                Particles.Add(new Particle(x, y, Utils.Random.NextSingle() * 2 * speed - speed, Utils.Random.NextSingle() * 2 * speed - speed,
                                           0, size, sizeChange, 1000, MathF.Tau * Utils.Random.NextSingle(), Color.White));
            }
        }
        public void EnemyExplosion(float x, float y, float speed, float size, float sizeChange, int intensity)
        {
            for (int i = 0; i < intensity; i++)
            {
                EnemyParticles.Add(new Particle(x, y, Utils.Random.NextSingle() * 2 * speed - speed, Utils.Random.NextSingle() * 2 * speed - speed,
                                           0, size, sizeChange, 1000, MathF.Tau * Utils.Random.NextSingle(), Color.White));
            }
        }
        public void DrawRenderTargets(Shapes _shapes, Camera camera, float scale, GraphicsDevice graphicsDevice, BloomFilter bloomFilter, BloomFilter enemyBloomFilter)
        {
            _shapes.BeginRenderTarget(ref magicCanvas);
            magicCanvas.GraphicsDevice.Clear(Color.Transparent);
            foreach (Projectile projectile in Projectiles) projectile.Draw(_shapes, camera, scale);
            foreach (Particle particle in Particles) particle.Draw(_shapes, camera, scale);
            _shapes.End();
            bloom = bloomFilter.Draw(magicCanvas, width, height);
            graphicsDevice.SetRenderTarget(null);
            _shapes.BeginRenderTarget(ref enemyMagicCanvas);
            enemyMagicCanvas.GraphicsDevice.Clear(Color.Transparent);
            foreach (Projectile projectile in EnemyProjectiles) projectile.Draw(_shapes, camera, scale);
            foreach (Particle particle in EnemyParticles) particle.Draw(_shapes, camera, scale);
            _shapes.End();
            enemyBloom = enemyBloomFilter.Draw(enemyMagicCanvas, width, height);
            graphicsDevice.SetRenderTarget(null);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp);
            _spriteBatch.Draw(bloom, Vector2.Zero, Color.Red);
            _spriteBatch.Draw(magicCanvas, Vector2.Zero, null, Color.Orange, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(enemyBloom, Vector2.Zero, Color.Green);
            _spriteBatch.Draw(enemyMagicCanvas, Vector2.Zero, null, Color.LightGreen, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
        }
    }
}
