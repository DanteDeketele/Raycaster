using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Raycaster
{
    public class Camera
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public float Radius { get; set; } = 0.2f;

        public float Angle { get; set; }

        public float[] DepthBuffer;

        public float[,] EntityBuffer;
        public bool[,] RenderedBuffer;


        public bool Render;

        public float RenderLoaded;
        public float[,] RenderLoadBuffer;

        public bool[,] RenderWorldBuffer;

        private Random random = new Random(0);

        public float RollAngle = 0;

        public bool GrayScale = false;

        public Vector2 Position { get; set; } = Vector2.One*2.5f;
        public Vector2 Right => new Vector2(MathF.Cos(Angle+MathF.PI/2), MathF.Sin(Angle+MathF.PI / 2));

        public Vector2 Forward => new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));

        public Texture2D Texture;
        private Color[] Colors;

        public Image Overlay;

        public Color FilterColor = new Color(183, 165, 143);
        public Camera(int width, int height, GraphicsDevice graphicsDevice)
        {
            Width = width;
            Height = height;
            DepthBuffer = new float[width];
            EntityBuffer = new float[width,height];
            RenderedBuffer = new bool[width, height];

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
            Colors = new Color[width*3 * height*3];
            Texture = new Texture2D(graphicsDevice, Width*3, Height*3);
        }

        public void SetColor(bool color, int x, int y)
        {
            Colors[(x) + (y) * Texture.Width] = color ? Color.Black : Color.White;

        }
        public void SetColor(Color color, int x, int y, int size = 1)
        {
            if (x < 0 || x >= Width * size || y < 0 || y >= Height * size)
                return;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ushort overlay = 19;
                    if (Overlay.Width == 320)
                    {
                        overlay = Overlay.GetBrightness(x/size, y/size);
                    }
                    Colors[(x + i) + (y + j) * Texture.Width] = Color.Multiply(Color.Multiply(FilterColor, color.R/255f), overlay/19f);
                    
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = Color.Black;
            }
            RenderedBuffer = new bool[Width, Height];
        }

        public void Draw(SpriteBatch spriteBatch, Point ScreenRes)
        {
            Texture.SetData(Colors);
            spriteBatch.Draw(Texture, new Rectangle(0,0, ScreenRes.X, ScreenRes.Y), Color.White);
            for (int i = 0;i < Colors.Length;i++)
            {
                Colors[i] = Color.Black;
            }
            RenderedBuffer = new bool[Width, Height];
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
