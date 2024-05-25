using Microsoft.Xna.Framework;
using System;

namespace Raycaster
{
    internal class Bullet : Entity
    {
        private Level _level;
        public Bullet(Image texture, Vector2 position, Enemy[] enemies, Level level, int damage = 0) : base(texture, position, 0)
        {
            Damage = damage;
            _enemies = enemies;
            Radius = 0.05f;
            _level = level;
        }

        public int Damage = 1;
        private readonly Enemy[] _enemies;

        public override void Update(float time, Camera camera)
        {
            base.Update(time, camera);
            if (!Active)
                return;

            if (!(_level.MapData[(int)Position.X, (int)Position.Y] == 0 || _level.MapData[(int)Position.X, (int)Position.Y] == 67))
            {
                
                Destroy();//CreateBulletHole();
            }

            foreach (Enemy enemy in _enemies)
            {
                if (_level.entities.Contains(enemy))
                {
                    if (enemy.IsOverlapping(this) && !enemy.Dead)
                    {
                        Destroy();
                        enemy.damage(Damage);
                    }
                }
            }
        }

        private void Destroy()
        {
            this.Active = false;
        }

        private void CreateBulletHole()
        {
            BulletHole bh = new BulletHole(Texture, Position, new Point(0, 1));
            bh.Speed = 20;
            bh.Size = 0.1f;
            bh.Position.Y = MathF.Round(bh.Position.Y);

            _level.entitiesToAdd.Add(bh);
        }
    }

    
}
