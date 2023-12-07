using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raycaster
{
    internal class Bullet : Entity
    {
        public Bullet(Image texture, Vector2 position, Enemy[] enemies, int damage = 0) : base(texture, position, 0)
        {
            Damage = damage;
            _enemies = enemies;
            Radius = 0.05f;
        }

        public int Damage = 1;
        private readonly Enemy[] _enemies;

        public override void Update(float time, Camera camera)
        {
            base.Update(time, camera);
            if (!Active)
                return;

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.IsOverlapping(this) && !enemy.Dead)
                {
                    enemy.damage(Damage);
                    this.Active = false;
                }
            }
        }
    }

    
}
