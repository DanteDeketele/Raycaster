using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raycaster
{
    internal class InputHandeler
    {
        private MouseState mouseState = new MouseState();
        private Point mousePos;
        private Point prevMousePos;
        private Point mouseOffset;


        public Vector2 MouseChange{
            get
            {
                if (MouseLocked)
                    return new Vector2(-mouseOffset.X + mousePos.X, -mouseOffset.Y + mousePos.Y);
                return new Vector2(-prevMousePos.X + mousePos.X, -prevMousePos.Y + mousePos.Y);
            }
        }

        public InputHandeler(Point mouseOffset)
        {
            this.mouseOffset = mouseOffset;
        }


        public Point MousePosition => mousePos;

        public bool MouseLocked = true;

        public static Vector2 MoveDirection
        {
            get
            {
                KeyboardState state = Keyboard.GetState();

                Vector2 dir = Vector2.Zero;
                dir += Vector2.UnitY * (state.IsKeyDown(Keys.W) ? 1 : 0);
                dir -= Vector2.UnitY * (state.IsKeyDown(Keys.S) ? 1 : 0);

                dir -= Vector2.UnitX * (state.IsKeyDown(Keys.A) ? 1 : 0);
                dir += Vector2.UnitX * (state.IsKeyDown(Keys.D) ? 1 : 0);

                return dir;
            }
        }

        public void Update()
        {
            mouseState = Mouse.GetState();
            prevMousePos = mousePos;
            mousePos = mouseState.Position;
            if (MouseLocked)
                Mouse.SetPosition(mouseOffset.X, mouseOffset.Y);
        }
    }
}
