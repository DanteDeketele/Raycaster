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

        public float Angle { get; set; }

        public float[] DepthBuffer;

        public float[,] EntityBuffer;

        public bool Render;

        public float RenderLoaded;
        public float[,] RenderLoadBuffer;

        public bool[,] RenderWorldBuffer;

        private Random random = new Random(0);

        public Vector2 Position { get; set; } = Vector2.One*1.5f;
        public Vector2 Right => new Vector2(MathF.Cos(Angle+MathF.PI/2), MathF.Sin(Angle+MathF.PI / 2));

        public Vector2 Forward => new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));
        public Camera(int width, int height)
        {
            Width = width;
            Height = height;
            DepthBuffer = new float[width];
            EntityBuffer = new float[width,height];
            RenderLoadBuffer = new float[width,height];
            RenderWorldBuffer = new bool[width,height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    RenderLoadBuffer[i, j] = random.NextSingle();
                    RenderWorldBuffer[i, j] = true;
                }
            }
        }

        public void Update(float deltaTime)
        {
            if (Render){
                if (RenderLoaded < 1)
                    RenderLoaded += deltaTime * 2;
            }
            else
            {
                if (RenderLoaded > 0)
                    RenderLoaded -= deltaTime * 2;
            }
            
        }

        public void ClearEntityBuffer()
        {
            EntityBuffer = new float[Width,Height];
            RenderWorldBuffer = new bool[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    RenderWorldBuffer[i, j] = true;
                }
            }
        }
    }
}
