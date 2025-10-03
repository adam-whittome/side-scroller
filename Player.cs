using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CSCRefactor
{
    public class Player
    {
        public RectangleF Rectangle;

        public float SpeedX;
        private float maxSpeed;
        private float airAcceleration;
        private float groundAcceleration;

        public float SpeedY;
        private float gravity;
        private float jumpPower;
        private bool isGrounded;

        private Animator animator;
        private Frame currentFrame;
        private bool isFacingLeft;


        private bool heldAttack1 = false;
        private bool heldAttack2 = false;

        public Player(RectangleF Rectangle, float maxSpeed, float airAcceleration, float groundAcceleration, float gravity, float jumpPower,
                      Animator animator)
        {
            this.Rectangle = Rectangle;

            this.maxSpeed = maxSpeed;
            this.airAcceleration = airAcceleration;
            this.groundAcceleration = groundAcceleration;

            this.gravity = gravity;
            this.jumpPower = jumpPower;

            this.animator = animator;
            currentFrame = animator.GetCurrentFrame(0);
        }
        public void Update(float deltaTime, List<RectangleF> colliders, ref MagicManager magicManager,
                           Camera camera, ref SoundManager soundManager)
        {
            Move(deltaTime, colliders, ref magicManager);
            Attack(ref magicManager.Projectiles, camera, ref soundManager);
            currentFrame = animator.GetCurrentFrame(deltaTime);
        }
        public void Move(float deltaTime, List<RectangleF> colliders, ref MagicManager magicManager)
        {
            // get inputs
            KeyboardState state = Keyboard.GetState();
            bool left = state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A);
            bool right = state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D);
            bool up = state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space);
            // x movement
            if (left && !right)
            {
                if (isGrounded) SpeedX -= groundAcceleration * deltaTime;
                else SpeedX -= airAcceleration * deltaTime;
                if (SpeedX < -maxSpeed) SpeedX += groundAcceleration * deltaTime;
                animator.TargetAnimation = "run_left";
                if (SpeedX > 0) animator.TargetAnimation = "slow_right";
                isFacingLeft = true;
            }
            else if (right && !left)
            {
                if (isGrounded) SpeedX += groundAcceleration * deltaTime;
                else SpeedX += airAcceleration * deltaTime;
                if (SpeedX > maxSpeed) SpeedX -= groundAcceleration * deltaTime;
                animator.TargetAnimation = "run_right";
                if (SpeedX < 0) animator.TargetAnimation = "slow_left";
                isFacingLeft = false;
            }
            else
            {
                if (SpeedX > 0)
                {
                    SpeedX -= airAcceleration * deltaTime;
                    if (SpeedX < 0) SpeedX = 0;
                    animator.TargetAnimation = "slow_right";
                }
                else if (SpeedX < 0)
                {
                    SpeedX += airAcceleration * deltaTime;
                    if (SpeedX > 0) SpeedX = 0;
                    animator.TargetAnimation = "slow_left";
                }
                else
                {
                    if (isFacingLeft) animator.TargetAnimation = "idle_left";
                    else animator.TargetAnimation = "idle_right";
                }
            }
            Rectangle.X += SpeedX * deltaTime;
            // x collision TODO: Implement Zoning https://gamedev.stackexchange.com/questions/76185/how-to-optimize-collision-detection/76201
            foreach (RectangleF collider in colliders)
            {
                if (Rectangle.Intersects(collider))
                {
                    if (SpeedX < 0) Rectangle.Left = collider.Right;
                    else Rectangle.Right = collider.Left;
                    SpeedX = 0;
                }
            }
            // y movement
            SpeedY += gravity * deltaTime;
            if (up && isGrounded)
            {
                isGrounded = false;
                SpeedY = jumpPower;
                SpeedX *= 1.7f;
            }
            if (!isGrounded)
            {
                if (SpeedX < 0 || (isFacingLeft && SpeedX == 0))
                {
                    if (SpeedY > 0.15f) animator.TargetAnimation = "jump_left";
                    else if (SpeedY < -0.3f) animator.TargetAnimation = "fall_left";
                    else animator.TargetAnimation = "float_left";
                }
                else if (SpeedX > 0 || (!isFacingLeft && SpeedX == 0))
                {
                    if (SpeedY > 0.15f) animator.TargetAnimation = "jump_right";
                    else if (SpeedY < -0.3f) animator.TargetAnimation = "fall_right";
                    else animator.TargetAnimation = "float_right";
                }
            }
            Rectangle.Y += SpeedY * deltaTime;
            // y collision
            isGrounded = false;
            foreach (RectangleF collider in colliders)
            {
                if (Rectangle.Intersects(collider))
                {
                    if (SpeedY < 0) { Rectangle.Bottom = collider.Top; isGrounded = true; }
                    else Rectangle.Top = collider.Bottom;
                    SpeedY = 0;
                }
            }
            for (int i = 0; i < magicManager.EnemyProjectiles.Count; i++)
            {
                if (Rectangle.Intersects(magicManager.EnemyProjectiles[i].Rectangle))
                {
                    magicManager.EnemyExplosion((Rectangle.CenterX + magicManager.EnemyProjectiles[i].Rectangle.CenterX) / 2,
                                               (Rectangle.CenterY + magicManager.EnemyProjectiles[i].Rectangle.CenterY) / 2,
                                                1f, 25, -0.14f, 20);
                    float health = magicManager.EnemyProjectiles[i].Damage;
                    magicManager.EnemyProjectiles.RemoveAt(i); i--;
                    //soundManager.PlaySoundEffect("fireball_land");
                }
            }
        }

        public void Attack(ref List<Projectile> projectiles, Camera camera, ref SoundManager soundManager)
        {
            // get inputs
            bool attack1 = Mouse.GetState().LeftButton == ButtonState.Pressed && !heldAttack1;

            Vector2 velocity = new Vector2(Mouse.GetState().X - camera.XOffset - Rectangle.CenterX,
                                           camera.Height - Mouse.GetState().Y - camera.YOffset - Rectangle.CenterY);
            velocity.Normalize();
            velocity *= 2f;

            // do the attack
            if (attack1)
            {
                soundManager.PlaySoundEffect("fireball_launch");
                projectiles.Add(new Fireball(new RectangleF(Rectangle.CenterX, Rectangle.CenterY, 50, 50), velocity.X, velocity.Y, -0.005f, 15, 3, 5));
            }

            heldAttack1 = Mouse.GetState().LeftButton == ButtonState.Pressed;
        }
        public void Draw(SpriteBatch _spriteBatch, Camera camera, float scale, Shapes shapes)
        {
            _spriteBatch.Draw(currentFrame.Texture, camera.GetSpritePosition(Rectangle, currentFrame, scale), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
