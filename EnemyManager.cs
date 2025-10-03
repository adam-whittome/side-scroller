using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    class EnemyManager
    {
        public List<Enemy> Enemies;
        public EnemyManager(List<Enemy> enemies)
        {
            Enemies = enemies;
        }
        public void Load(List<Enemy> enemies)
        {
            Enemies = enemies;
        }
        public void Update(float deltaTime, RectangleF target, List<RectangleF> colliders, ref MagicManager magicManager, ref SoundManager soundManager)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Update(deltaTime, target, colliders, ref magicManager, ref soundManager);
                if (Enemies[i].Death()) { Enemies.RemoveAt(i); i--; }
            }
        }
        public void Draw(SpriteBatch _spriteBatch, Camera camera, float scale)
        {
            foreach (Enemy enemy in Enemies) enemy.Draw(_spriteBatch, camera, scale);
        }
    }
}
