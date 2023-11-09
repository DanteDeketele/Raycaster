using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raycaster
{
    internal class Entity
    {
        public Texture2D Texture;

        public int State = 1;

        public float timer = 0;

        public float DistanceToCamera = 100;

        public Entity(Texture2D texture, Vector2 position, float angle = 0)
        {
            Texture = texture;
            Position = position;
            Angle = angle;
        }

        public void Update(float time, Camera camera)
        {
            timer += time;
            if (timer > 0.2f && State >= 1 && State <=4)
            {
                State++;
                if (State > 4)
                    State = 1;

                timer = 0;
            }

            DistanceToCamera = Vector2.Distance(Position, camera.Position);
        }

        public float width = 1;
        public Vector2 Position;
        public float Angle = 0;
    }
}
