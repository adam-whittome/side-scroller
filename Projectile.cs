using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CSCRefactor.ProceduralGraphics;

namespace CSCRefactor
{
    public class Projectile
    {
        public RectangleF Rectangle;
        public float SpeedX;
        public float SpeedY;
        public float Gravity;
        public float Damage;
        public Projectile(RectangleF rectangle, float speedX, float speedY, float gravity, float damage)
        {
            Rectangle = rectangle;
            SpeedX = speedX;
            SpeedY = speedY;
            Gravity = gravity;
            Damage = damage;
        }
        public virtual void Update(float deltaTime, List<RectangleF> colliders, ref List<Particle> particles)
        {
            Rectangle.X += SpeedX * deltaTime;
            SpeedY += Gravity * deltaTime;
            Rectangle.Y += SpeedY * deltaTime;
        }
        public virtual bool Death(RectangleF bounds)
        {
            return !bounds.Intersects(Rectangle);
        }
        public virtual void Draw(Shapes _shapes, Camera camera, float scale)
        {
            _shapes.DrawCircle((Rectangle.CenterX + camera.XOffset) / scale,
                               (Rectangle.CenterY + camera.YOffset) / scale,
                                Rectangle.Width / scale / 2, 8, Color.White);
        }
    }

    public class EnemyProjectile : Projectile
    {
        public EnemyProjectile(RectangleF rectangle, float speedX, float speedY, float gravity, float damage) :
               base(rectangle, speedX, speedY, gravity, damage) { }
        public override void Update(float deltaTime, List<RectangleF> colliders, ref List<Particle> particles)
        {
            base.Update(deltaTime, colliders, ref particles);
            for (int i = 0; i < 3; i++)
                particles.Add(new Particle(Rectangle.CenterX + Utils.Random.NextSingle() * Rectangle.Width - Rectangle.Width / 2,
                                           Rectangle.CenterY + Utils.Random.NextSingle() * Rectangle.Height - Rectangle.Height / 2,
                                           0, 0, 0, 16, -0.06f, 1000, Utils.Random.NextSingle() * MathF.Tau, Color.White));
        }
    }

    public class Fireball : Projectile
    {
        public int ParticleRate;
        public int Bounces;
        public Fireball(RectangleF rectangle, float speedX, float speedY, float gravity, float damage, int bounces, int particleRate) :
               base(rectangle, speedX, speedY, gravity, damage)
        {
            Bounces = bounces;
            ParticleRate = particleRate;
        }
        public override void Update(float deltaTime, List<RectangleF> colliders, ref List<Particle> particles)
        {
            
            Rectangle.X += SpeedX * deltaTime;
            foreach (RectangleF collider in colliders)
            {
                if (Rectangle.Intersects(collider))
                {
                    if (SpeedX > 0) Rectangle.Right = collider.Left;
                    else Rectangle.Left = collider.Right;
                    SpeedX = -SpeedX;
                    Bounces--;
                }
            }
            SpeedY += Gravity * deltaTime;
            Rectangle.Y += SpeedY * deltaTime;
            foreach (RectangleF collider in colliders)
            {
                if (Rectangle.Intersects(collider))
                {
                    if (SpeedY < 0) { Rectangle.Bottom = collider.Top; SpeedY = -SpeedY * 0.78f; Bounces--; }
                    else { Rectangle.Top = collider.Bottom; SpeedY = -SpeedY; }
                }
            }
            for (int i = 0; i < 3; i++)
            particles.Add(new Particle(Rectangle.CenterX + Utils.Random.NextSingle() * Rectangle.Width - Rectangle.Width / 2,
                                       Rectangle.CenterY + Utils.Random.NextSingle() * Rectangle.Height - Rectangle.Height / 2,
                                       0, 0, 0, 16, -0.06f, 1000, Utils.Random.NextSingle() * MathF.Tau, Color.White));
        }
        public override bool Death(RectangleF bounds)
        {
            return Bounces < 0 || !bounds.Intersects(Rectangle);
        }
    }
}
