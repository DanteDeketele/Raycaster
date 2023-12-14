using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class BulletHole : Entity
    {
        public Point Normal;
        public BulletHole(Image texture, Vector2 position, Point normal) : base(texture, position, 0)
        {
            this.Normal = normal;
        }
    }
}
