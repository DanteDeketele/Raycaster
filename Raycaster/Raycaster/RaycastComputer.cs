using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Raycaster
{
    internal static class RaycastComputer
    {
        public static void DrawScreen(Camera camera, Level level, SpriteBatch spriteBatch, Texture2D textureSheet)
        {
            Random rand = new Random();

            int width = camera.Width;
            int height = camera.Height;

            int[,] map = level.MapData;

            float angle = camera.Angle;
            Vector2 position = camera.Position;

            Vector2 playerDir = camera.Forward;

            for (int i = 0; i < width; i++)
            {
                float cameraX = 2 * i / (float)width - 1;
                Vector2 rayDir = new Vector2(
                    playerDir.X + camera.Right.X * cameraX,
                    playerDir.Y + camera.Right.Y * cameraX);

                int mapX = (int)position.X;
                int mapY = (int)position.Y;

                double sideDistX, sideDistY;
                double deltaDistX = Math.Abs(1 / rayDir.X);
                double deltaDistY = Math.Abs(1 / rayDir.Y);
                double perpWallDist;

                int stepX, stepY;
                bool hit = false;
                int side = 0;

                if (rayDir.X < 0)
                {
                    stepX = -1;
                    sideDistX = (position.X - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - position.X) * deltaDistX;
                }
                if (rayDir.Y < 0)
                {
                    stepY = -1;
                    sideDistY = (position.Y - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - position.Y) * deltaDistY;
                }

                while (!hit)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (mapX < 0 || mapX >= map.GetLength(0) || mapY < 0 || mapY >= map.GetLength(1))
                        break;

                    if (map[mapX, mapY] > 0)
                        hit = true;
                }

                if (!hit)
                    continue;

                if (side == 0)
                    perpWallDist = (mapX - position.X + (1 - stepX) / 2) / rayDir.X;
                else
                    perpWallDist = (mapY - position.Y + (1 - stepY) / 2) / rayDir.Y;

                int columnHeight = (int)(height / perpWallDist);
                int drawStart = Math.Max(0, (height - columnHeight) / 2);
                int drawEnd = Math.Min(height - 1, (height + columnHeight) / 2);

                double wallX = (side == 0) ? position.Y + perpWallDist * rayDir.Y : position.X + perpWallDist * rayDir.X;
                wallX -= Math.Floor(wallX);

                int texWidth = textureSheet.Width; // Replace with actual texture width
                int texX = (int)(wallX * texWidth);

                if ((side == 0 && rayDir.X > 0) || (side == 1 && rayDir.Y < 0))
                    texX = texWidth - texX - 1;

                for (int y = drawStart; y <= drawEnd; y++)
                {
                    int brightness = 9;
                    brightness -= side * 2;
                    
                    if (side == 0)
                        perpWallDist = (mapX - position.X + (1 - stepX) / 2) / rayDir.X;
                    else
                        perpWallDist = (mapY - position.Y + (1 - stepY) / 2) / rayDir.Y;

                    brightness -= (int)perpWallDist;

                    //float t = ((float)perpWallDist - MathF.Round((float)perpWallDist)) * 10;

                    //brightness -= (rand.Next(0, 2)) < 1? 1:0 ;

                    if (brightness < 0)
                        brightness = 0;
                    if (brightness > 9)
                        brightness = 9;



                    int[,] patern = PixelPatern(brightness);

                    for (int x1 = 0; x1 < camera.Upscale/2; x1++)
                    {
                        for (int y1 = 0; y1 < camera.Upscale/2; y1++)
                        {
                            Color color = patern[x1, y1]==1? Color.White : Color.Black;

                            spriteBatch.Draw(textureSheet, new Rectangle(i * camera.Upscale + x1*2, y * camera.Upscale + y1*2, 2, 2), color);
                        }
                    }
                }
            }
        }

        private static Color GetTextureColor(Texture2D texture, int x, int y)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            if (x >= 0 && x < texture.Width && y >= 0 && y < texture.Height)
            {
                return data[y * texture.Width + x];
            }

            return Color.White; // Handle out-of-bounds access
        }


        public static void DrawTopView(Camera camera, Level level, SpriteBatch spriteBatch, Texture2D textureSheet)
        {
            int width = camera.Width/2;
            int height = camera.Height/2;

            width *= camera.Upscale;
            height *= camera.Upscale;

            int[,] map = level.MapData;

            float angle = camera.Angle;
            Vector2 position = camera.Position;

            int cellSize = width / map.GetLength(0);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Color c = Color.Transparent;
                    switch (map[i, j])
                    {
                        case 0:
                            break;
                        case 1:
                            c = Color.White;
                            break;
                    }

                    Rectangle dest = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                    spriteBatch.Draw(textureSheet, dest, c);
                }
            }

            Rectangle playerDest = new Rectangle((int)(position.X * cellSize)-5, (int)(position.Y * cellSize)-5, 10,10);
            spriteBatch.Draw(textureSheet, playerDest, Color.Red);

            Rectangle playerViewDest = new Rectangle((int)((position.X + camera.Forward.X * 0.1f) * cellSize) - 5, (int)((position.Y + camera.Forward.Y * 0.1f) * cellSize) - 5, 10, 10);
            spriteBatch.Draw(textureSheet, playerViewDest, Color.BlueViolet);

            Rectangle playerView2Dest = new Rectangle((int)((position.X + camera.Right.X * 0.1f) * cellSize) - 5, (int)((position.Y + camera.Right.Y * 0.1f) * cellSize) - 5, 10, 10);
            spriteBatch.Draw(textureSheet, playerView2Dest, Color.Pink);
        }

        private static int[,] PixelPatern(int type)
        {
            switch (type)
            {
                case 0:
                    return new int[,] {
                        { 0, 0, 0},
                        { 0, 0, 0},
                        { 0, 0, 0}
                    };
                case 1:
                    return new int[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 0, 0, 0}
                    };
                case 2:
                    return new int[,] {
                        { 0, 0, 1},
                        { 0, 0, 0},
                        { 1, 0, 0}
                    };
                case 3:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 1, 0},
                        { 0, 0, 1}
                    };
                case 4:
                    return new int[,] {
                        { 0, 1, 0},
                        { 1, 0, 1},
                        { 0, 1, 0}
                    };
                case 5:
                    return new int[,] {
                        { 1, 0, 1},
                        { 0, 1, 0},
                        { 1, 0, 1}
                    };
                case 6:
                    return new int[,] {
                        { 0, 1, 1},
                        { 1, 0, 1},
                        { 1, 1, 0}
                    };
                case 7:
                    return new int[,] {
                        { 1, 1, 0},
                        { 1, 1, 1},
                        { 0, 1, 1}
                    };
                case 8:
                    return new int[,] {
                        { 1, 1, 1},
                        { 1, 0, 1},
                        { 1, 1, 1}
                    };
                case 9:
                    return new int[,] {
                        { 1, 1, 1},
                        { 1, 1, 1},
                        { 1, 1, 1}
                    };
            }
            return new int[,] {
                { 0, 0, 0},
                { 0, 0, 0},
                { 0, 0, 0}
            };
        }
        
    }
}
