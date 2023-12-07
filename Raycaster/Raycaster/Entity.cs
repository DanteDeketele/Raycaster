using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Raycaster
{

    internal class Entity
    {
        public Image Texture;

        public int State = 1;

        public float timer = 0;

        public float Speed = 1.1f;
        public float Size = 1;
        public bool IsBullet = false;

        public float DistanceToCamera = 100;

        public Vector2[] waypoints = new Vector2[0];
        public int waypoint = 0;

        public bool Active = true;

        public bool IsStaticSprite = false;
        public int StaticSprite = 0;

        public float width = 1;
        public Vector2 Position;
        public float Angle = 0;
        public float Radius = 0.3f;

        public Entity(Image texture, Vector2 position, float angle = 0)
        {
            Texture = texture;
            Position = position;
            Angle = angle;
        }

        public virtual void Update(float time, Camera camera)
        {
            DistanceToCamera = Vector2.Distance(Position, camera.Position);

            if (waypoints.Length == 0)
            {
                State = 0;
                return;
            }
            
            timer += time;
            if (timer > 0.2f && State >= 1 && State <=4)
            {
                State++;
                if (State > 4)
                    State = 1;

                timer = 0;
            }
            
            Position -= Vector2.Normalize(Position - waypoints[waypoint]) * Speed * time;
            Angle = MathF.Atan2((waypoints[waypoint] - Position).Y, (waypoints[waypoint] - Position).X);

            if ((Position - waypoints[waypoint]).Length() <= 0.5f)
            {
                waypoint++;
                if (IsBullet)
                    Active = false;
                if (waypoint >= waypoints.Length)
                    waypoint = 0;
            }
        }

        public bool IsOverlapping(Entity entity)
        {
            if (Vector2.Distance(Position, entity.Position) < Radius + entity.Radius)
            {
                return true;
            }
            return false;
        }
    }

    
}
