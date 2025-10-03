using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CSCRefactor
{
    class Enemy
    {
        public RectangleF Rectangle;

        private float speedX;
        private float speedY;
        private float gravity;

        private float attackTimer = 0;
        private float attackFrequency;
        public float health;

        private Animator animator;
        private Frame currentFrame;
        public Enemy(RectangleF rectangle, float attackFrequency, float health, float gravity, Animator animator)
        {
            this.Rectangle = rectangle;
            this.gravity = gravity;
            this.attackFrequency = attackFrequency;
            this.health = health;
            this.animator = animator;
            currentFrame = animator.GetCurrentFrame(0);
        }
        public void Update(float deltaTime, RectangleF target, List<RectangleF> colliders, ref MagicManager magicManager, ref SoundManager soundManager)
        {
            // movement
            speedY += gravity * deltaTime;
            Rectangle.Y += speedY * deltaTime;
            for (int i = 0; i < magicManager.Projectiles.Count; i++)
            {
                if (Rectangle.Intersects(magicManager.Projectiles[i].Rectangle))
                {
                    magicManager.FireExplosion((Rectangle.CenterX + magicManager.Projectiles[i].Rectangle.CenterX) / 2,
                                               (Rectangle.CenterY + magicManager.Projectiles[i].Rectangle.CenterY) / 2,
                                                1f, 25, -0.14f, 20);
                    health -= magicManager.Projectiles[i].Damage;
                    magicManager.Projectiles.RemoveAt(i); i--;
                    soundManager.PlaySoundEffect("fireball_land");
                }
            }
            foreach (RectangleF collider in colliders)
            {
                if (Rectangle.Intersects(collider))
                {
                    if (speedY <= 0) { Rectangle.Bottom = collider.Top; speedY = 0; }
                    else { Rectangle.Top = collider.Bottom; speedY = 0; }
                }
            }

            // attack
            attackTimer += deltaTime;
            if (attackTimer >= attackFrequency) { attackTimer -= attackFrequency; Attack(ref magicManager, target, 1f); }

            currentFrame = animator.GetCurrentFrame(deltaTime);
        }
        public void Attack(ref MagicManager magicManager, RectangleF target, float speed)
        {
            Vector2 direction = target.Center - Rectangle.Center;
            direction.Normalize();
            direction *= speed;
            magicManager.EnemyProjectiles.Add(new EnemyProjectile(new RectangleF(Rectangle.CenterX, Rectangle.CenterY, 40, 40), direction.X, direction.Y, 0, 15));
            animator.TargetAnimation = "attack_left";
        }
        public bool Death()
        {
            return health <= 0;
        }
        public void Draw(SpriteBatch _spriteBatch, Camera camera, float scale)
        {
            _spriteBatch.Draw(currentFrame.Texture, camera.GetSpritePosition(Rectangle, currentFrame, scale), null, Color.White, 0, Vector2.Zero, scale, 0, 0);
        }
    }
}
