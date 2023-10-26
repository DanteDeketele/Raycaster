using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Raycaster
{
    internal class Camera
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public float Radius { get; set; } = 0.1f;

        public int Upscale { get; set; }

        public float Angle { get; set; }
        public Vector2 Position { get; set; } = Vector2.One;
        public Vector2 Right => new Vector2(MathF.Cos(Angle+MathF.PI/2), MathF.Sin(Angle+MathF.PI / 2));

        public Vector2 Forward => new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));
        public Camera(int width, int height, int upscale)
        {
            Width = width;
            Height = height;
            Upscale = upscale;
        }
    }
}
