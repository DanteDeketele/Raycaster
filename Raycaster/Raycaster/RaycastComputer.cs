using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Raycaster
{
    internal static class RaycastComputer
    {
        public static void DrawScreen(Point screenRes,Camera camera, Level level, SpriteBatch spriteBatch, Texture2D textureSheet, Texture2D whiteTexture, Texture2D glowTexture, float interactibleFase)
        {
            int width = camera.Width;
            int height = camera.Height;

            int[,] map = level.MapData;

            Vector2 position = camera.Position;

            Vector2 playerDir = camera.Forward;

            Color[] colorData = new Color[textureSheet.Width * textureSheet.Height];
            textureSheet.GetData(colorData);

            Color[] colorGlowData = new Color[glowTexture.Width * glowTexture.Height];
            glowTexture.GetData(colorGlowData);

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

                int projectedHeight = (int)(height / perpWallDist);

                int drawStart = Math.Max(0, (height - projectedHeight) / 2);
                int drawEnd = Math.Min(height - 1, (height + projectedHeight) / 2);

                int offset = height - projectedHeight;


                double wallX = (side == 0) ? position.Y + perpWallDist * rayDir.Y : position.X + perpWallDist * rayDir.X;
                wallX -= Math.Floor(wallX);

                int texWidth = textureSheet.Width;
                int texX = (int)(wallX * texWidth);

                if ((side == 0 && rayDir.X > 0) || (side == 1 && rayDir.Y < 0))
                    texX = texWidth - texX - 1;

                int texWidthGlow = glowTexture.Width;
                int texXglow = (int)(wallX * texWidthGlow);

                if ((side == 0 && rayDir.X > 0) || (side == 1 && rayDir.Y < 0))
                    texXglow = texWidthGlow - texXglow - 1;

                for (int y = drawStart; y <= drawEnd; y++)
                {
                    int texY = (int)((y - drawStart - MathF.Min(0, offset / 2)) * (float)textureSheet.Height / projectedHeight);
                    int texYglow = (int)((y - drawStart - MathF.Min(0, offset / 2)) * (float)glowTexture.Height / projectedHeight);


                    int brightness = 19;
                    brightness -= side * 2;

                    if (side == 0)
                        perpWallDist = (mapX - position.X + (1 - stepX) / 2) / rayDir.X;
                    else
                        perpWallDist = (mapY - position.Y + (1 - stepY) / 2) / rayDir.Y;

                    brightness -= (int)(perpWallDist * 2);

                    if (brightness < 0)
                        brightness = 0;
                    if (brightness > 19)
                        brightness = 19;

                    Color c = GetTextureColor(colorData, textureSheet, texX, texY, GetWallTextureRect(map[mapX, mapY]));

                    if (map[mapX, mapY] == 100)
                    {
                        Color cGlow = GetGlowTextureColor(colorGlowData, glowTexture, texXglow, texYglow, interactibleFase);

                        if (cGlow.Equals(Color.White))
                        {
                            c *= 1.5f;
                        }
                    }

                    int br = (c.R + c.G + c.B) / 3;
                    br = 255 - br;
                    br *= 12;
                    br /= 255;

                    brightness -= br;

                    int[,] patern = PixelPatern(brightness);
                    RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, i, y, patern);
                }
            }
        }

        private static void RenderPixelPatern(Point screenRes, Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture, int x, int y, int[,] patern)
        {
            float virtualpixelSize = (screenRes.X / camera.Width)/3 + 1;

            Vector2 distortion = screenRes.ToVector2() - new Vector2(camera.Width * (int)(virtualpixelSize*3), camera.Height * (int)(virtualpixelSize*3));

            distortion = -distortion;

            if (distortion.X > 0)
            {
                //virtualpixelSize++;
            }

            for (int x1 = 0; x1 < 3; x1++)
            {
                for (int y1 = 0; y1 < 3; y1++)
                {
                    Color color = patern[x1, y1] == 1 ? Color.White : Color.Black;

                    spriteBatch.Draw(whiteTexture, new Rectangle((int)(x * virtualpixelSize * 3 + x1 * virtualpixelSize - distortion.X/2), (int)(y * virtualpixelSize * 3 + y1 * virtualpixelSize - distortion.Y / 2), (int)virtualpixelSize, (int)virtualpixelSize), color);
                }
            }
        }


        private static Color GetTextureColor(Color[] data, Texture2D texture, int x, int y, Rectangle source)
        {
            int texX = x * source.Width / texture.Width + source.X;
            int texY = y * source.Height / texture.Height + source.Y;

            if (texX >= source.Left && texX < source.Right && texY >= source.Top && texY <= source.Bottom)
            {
                int sourceX = texX - source.Left;
                int sourceY = texY - source.Top;
                return data[(sourceY + source.Top) * texture.Width + (sourceX + source.Left)];
            }

            return Color.White;
        }

        private static Color GetGlowTextureColor(Color[] data, Texture2D texture, int x, int y, float glowProgress)
        {

            x += (int)(glowProgress * texture.Width);
            x %= texture.Width;

            if (x >= 0 && x < texture.Width && y >= 0 && y < texture.Height)
            {
                return data[y * texture.Width + x];
            }

            return Color.White;
        }

        private static Rectangle GetWallTextureRect(int i)
        {
            int tileWidth = 384 / 6;
            int tileHeight = 1216 / 19;

            int column = (i - 1) % 6;
            int row = (i - 1) / 6;

            int sourceX = column * tileWidth;
            int sourceY = row * tileHeight;

            return new Rectangle(sourceX, sourceY, tileWidth, tileHeight);
        }



        public static void DrawTopView(Camera camera, Level level, SpriteBatch spriteBatch, Texture2D textureSheet)
        {
            int width = camera.Width/2;
            int height = camera.Height/2;

            width *= camera.Upscale;
            height *= camera.Upscale;

            int[,] map = level.MapData;

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
                        case 3:
                            c = Color.White;
                            break;
                        case 100:
                            c = Color.Gold;
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
                        { 0, 0, 0},
                        { 0, 0, 0}
                    };
                case 2:
                    return new int[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 0, 0, 0}
                    };
                case 3:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 0, 0},
                        { 0, 0, 0}
                    };
                case 4:
                    return new int[,] {
                        { 0, 0, 1},
                        { 0, 0, 0},
                        { 1, 0, 0}
                    };
                case 5:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 0, 0},
                        { 0, 0, 1}
                    };
                case 6:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 1, 0},
                        { 0, 0, 1}
                    };
                case 7:
                    return new int[,] {
                        { 0, 0, 1},
                        { 0, 1, 0},
                        { 1, 0, 0}
                    };
                case 8:
                    return new int[,] {
                        { 0, 1, 0},
                        { 1, 0, 1},
                        { 0, 1, 0}
                    };
                case 9:
                    return new int[,] {
                        { 1, 0, 1},
                        { 0, 0, 0},
                        { 1, 1, 1}
                    };
                case 10:
                    return new int[,] {
                        { 1, 0, 1},
                        { 0, 1, 0},
                        { 1, 0, 1}
                    };
                case 11:
                    return new int[,] {
                        { 0, 1, 0},
                        { 1, 1, 1},
                        { 0, 1, 0}
                    };
                case 12:
                    return new int[,] {
                        { 0, 1, 1},
                        { 1, 0, 1},
                        { 1, 1, 0}
                    };
                case 13:
                    return new int[,] {
                        { 1, 1, 0},
                        { 1, 0, 1},
                        { 0, 1, 1}
                    };
                case 14:
                    return new int[,] {
                        { 1, 1, 0},
                        { 1, 1, 1},
                        { 0, 1, 1}
                    };
                case 15:
                    return new int[,] {
                        { 0, 1, 1},
                        { 1, 1, 1},
                        { 1, 1, 0}
                    };
                case 16:
                    return new int[,] {
                        { 1, 1, 1},
                        { 1, 0, 1},
                        { 1, 1, 1}
                    };
                case 17:
                    return new int[,] {
                        { 1, 0, 1},
                        { 1, 1, 1},
                        { 1, 1, 1}
                    };
                case 18:
                    return new int[,] {
                        { 1, 1, 1},
                        { 1, 1, 1},
                        { 1, 1, 1}
                    };
                case 19:
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
