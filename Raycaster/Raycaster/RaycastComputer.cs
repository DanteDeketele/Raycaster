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

                camera.DepthBuffer[i] = (float)perpWallDist;

                int projectedHeight = (int)((height) / perpWallDist);

                int drawStart = Math.Max(0, (height - projectedHeight) / 2 );
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


                    brightness -= (int)(perpWallDist*1.5f);

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
                    RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, i, y + (int)((i - camera.Width/2)*camera.RollAngle*0.02f), patern);
                    //float virtualpixelSize = (screenRes.X / camera.Width);
                    //Vector2 distortion = screenRes.ToVector2() - new Vector2(camera.Width * (int)(virtualpixelSize), camera.Height * (int)(virtualpixelSize));
                    //spriteBatch.Draw(whiteTexture, new Rectangle((int)(i * virtualpixelSize - distortion.X / 2), (int)(y * virtualpixelSize - distortion.Y / 2), (int)virtualpixelSize, (int)virtualpixelSize), c);
                }
            }
        }

        private static void RenderPixelPatern(Point screenRes, Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture, int x, int y, int[,] patern)
        {
            if (x < 0 || x >= camera.Width || y < 0 || y >= camera.Height)
                return;

            if (camera.RenderLoadBuffer[x, y] > camera.RenderLoaded)
                return;

            if (!camera.RenderWorldBuffer[x, y])
                return;

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

        public static void DrawFrame(Texture2D frame, Camera camera, SpriteBatch spriteBatch, Point screenRes, Texture2D whiteTexture, Rectangle source, Color[] colorData)
        {
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    Color c = GetTextureColor(colorData, frame, i * (frame.Width / source.Width), j * (frame.Height / source.Height), source);

                    int br = (c.R + c.G + c.B) / 3;
                    br *= 19;
                    br /= 255;

                    int[,] patern = PixelPatern(br);
                    RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, i, j, patern);
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

        public static void DrawEntity(Point screenRes, Camera camera, Entity entity, SpriteBatch spriteBatch, Texture2D whiteTexture, bool outlined = false)
        {
            Color transparent = new Color(155, 0, 139);

            float angleDifference = MathF.Atan2(entity.Position.Y - camera.Position.Y, entity.Position.X - camera.Position.X) - camera.Angle;
            if (angleDifference < -MathF.PI)
            {
                angleDifference += 2 * MathF.PI;
            }
            else if (angleDifference > MathF.PI)
            {
                angleDifference -= 2 * MathF.PI;
            }

            Color[] colorData = new Color[entity.Texture.Width * entity.Texture.Height];
            entity.Texture.GetData(colorData);

            Vector2 cameraToEntity = entity.Position - camera.Position;

            float distance = -MathF.Abs(angleDifference * 0.4f) + cameraToEntity.Length();
            int width = (int)((camera.Height / distance)*entity.Size);
            int AngleOffset = (int)((angleDifference) / 2 * camera.Width + angleDifference * camera.Width / 9);


            // TODO: Calculate sprite to use
            float lookAngle = MathF.Atan2(entity.Position.Y - camera.Position.Y, entity.Position.X - camera.Position.X) - entity.Angle;

            if (lookAngle < -MathF.PI)
            {
                lookAngle += 2 * MathF.PI;
            }
            else if (lookAngle > MathF.PI)
            {
                lookAngle -= 2 * MathF.PI;
            }



            int spriteId = (int)(7-((lookAngle + MathF.PI) / (MathF.PI * 2) *8) + MathF.PI * 0.5f);

            if (spriteId < 0)
            {
                spriteId += 8;
            }
            else if (spriteId > 7)
            {
                spriteId -= 8;
            }
            spriteId += 8 * entity.State;

            if (entity.IsStaticSprite)
            {
                spriteId = entity.StaticSprite;
            }

            for (int i = 0; i < width; i++)
            {
                

                int xPos = i+camera.Width/2-width/2;
                xPos += AngleOffset;

                if (xPos < 0 || xPos >= camera.Width)
                    continue;

                if (camera.DepthBuffer[xPos] <= distance)
                    continue;

                for (int j = 0; j < width; j++)
                {
                    int yPos = j + camera.Height/2 - width / 2;
                    if (entity.IsBullet)
                    {
                        yPos += (int)(40/distance);
                    }

                    if (yPos < 0 || yPos >= camera.Height)
                        continue;
                    
                    if (camera.EntityBuffer[xPos, yPos] < distance && camera.EntityBuffer[xPos, yPos] != 0)
                        continue;

                    int brightness = 19;

                    brightness -= (int)(distance*0.75f);

                    if (brightness < 0)
                        brightness = 0;
                    if (brightness > 19)
                        brightness = 19;

                    Color c = GetTextureColor(colorData, entity.Texture, (int)(i* distance*5), (int)(j * distance*5), new Rectangle(entity.Texture.Width / 8 * (spriteId%8), entity.Texture.Height / 7 * (spriteId/8), entity.Texture.Width/8, entity.Texture.Height/7));

                    if (c == transparent)
                        continue;

                    int br = (c.R + c.G + c.B) / 3;
                    br = 255 - br;
                    br *= 17;
                    br /= 255;

                    brightness -= br;

                    int[,] patern = PixelPatern(brightness);
                    RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, xPos, yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f), patern);

                    if (outlined && !entity.IsBullet && (yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)) > 0 && (yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)) < camera.Height-1)
                    {
                        camera.EntityBuffer[xPos, yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)] = distance;
                    }
                }
            }
        }

        public static void DrawEntityOutlines(Point screenRes, Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture)
        {
            for (int i = 1; i < camera.Width-1; i++)
            {
                for (int j = 1; j < camera.Height-1; j++)
                {
                    if (camera.EntityBuffer[i, j] == 0)
                    {
                        if (camera.EntityBuffer[i + 1, j] != 0 ||
                            camera.EntityBuffer[i + 1, j + 1] != 0 ||
                            camera.EntityBuffer[i + 1, j - 1] != 0 ||
                            camera.EntityBuffer[i, j + 1] != 0 ||
                            camera.EntityBuffer[i, j - 1] != 0 ||
                            camera.EntityBuffer[i - 1, j] != 0 ||
                            camera.EntityBuffer[i - 1, j + 1] != 0 ||
                            camera.EntityBuffer[i - 1, j - 1] != 0)
                        {
                            int[,] patern = PixelPatern(19);
                            RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, i, j, patern);
                        }
                    }
                }
            }
        }

        public static void DrawGunOutlines(Point screenRes, Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture)
        {
            for (int i = 1; i < camera.Width - 1; i++)
            {
                for (int j = 1; j < camera.Height - 1; j++)
                {
                    if (camera.RenderWorldBuffer[i, j])
                    {
                        if (!camera.RenderWorldBuffer[i + 1, j] ||
                            !camera.RenderWorldBuffer[i + 1, j + 1]||
                            !camera.RenderWorldBuffer[i + 1, j - 1] ||
                            !camera.RenderWorldBuffer[i, j + 1]||
                            !camera.RenderWorldBuffer[i, j - 1] ||
                            !camera.RenderWorldBuffer[i - 1, j]  ||
                            !camera.RenderWorldBuffer[i - 1, j + 1] ||
                            !camera.RenderWorldBuffer[i - 1, j - 1] )
                        {
                            int[,] patern = PixelPatern(19);
                            RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, i, j, patern);
                        }
                    }
                }
            }
        }

        public static void DrawGun(Point screenRes, Camera camera, int gun, int frame, SpriteBatch spriteBatch, Texture2D texture, Texture2D whiteTexture)
        {
            Color transparent = new Color(152, 0, 136);

            int size = camera.Height / 2;

            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int brightness = 19;

                    Color c = GetTextureColor(colorData, texture, x * (texture.Width/size), y * (texture.Height / size), new Rectangle((texture.Width / 5)*(frame)+frame, (texture.Height / 4)*gun+ gun, texture.Width / 4-4-1, texture.Height / 3));

                    if (c == transparent)
                        continue;

                    int br = (c.R + c.G + c.B) / 3;
                    br = 255 - br;
                    br *= 19;
                    br /= 255;

                    brightness -= br;

                    int[,] patern = PixelPatern(brightness);
                    RenderPixelPatern(screenRes, camera, spriteBatch, whiteTexture, x + camera.Width/2 - (int)(size*0.5f), y+camera.Height-size-10, patern);
                    camera.RenderWorldBuffer[x + camera.Width / 2 -(int)(size * 0.5f), y+camera.Height - size - 10] = false;
                }
            }
        }


        public static float CalculateDistanceToCameraPlane(Vector2 cameraPosition, Vector2 spritePosition, Vector2 cameraForward)
        {
            // Calculate the vector from the camera to the sprite
            Vector2 cameraToSprite = spritePosition - cameraPosition;

            // Calculate the angle between cameraToSprite and the camera's forward vector
            float angle = MathF.Acos(Vector2.Dot(Vector2.Normalize(cameraToSprite), Vector2.Normalize(cameraForward)));

            // Calculate the distance from the sprite to its projection on the camera's plane
            float distanceToCameraPlane = cameraToSprite.Length() * MathF.Cos(angle);

            return distanceToCameraPlane;
        }


        static float CalculateAngleDifference(float angle1, float angle2)
        {
            float angle = (angle1 - angle2 + MathF.PI) % (2 * MathF.PI) - MathF.PI;
            return angle;
        }


        public static void DrawTopView(Camera camera, Level level, SpriteBatch spriteBatch, Texture2D textureSheet)
        {
            int width = camera.Width*2;

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

        public static void DrawFont(string text, Point topLeft, Point screenRes, Camera cam, Texture2D whiteTexture, SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(text)) return;

            char[] characters = text.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                int[,] fontPatern = FontPatern(c);

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        int[,] patern = PixelPatern(fontPatern[y,x]*19);

                        RenderPixelPatern(screenRes, cam, spriteBatch, whiteTexture, topLeft.X + x + i * 8, topLeft.Y + y, patern);
                    }
                }
            }
        }

        private static int[,] FontPatern(char number)
        {
            switch (number)
            {
                case '0':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,1,1,1,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,1,1,1,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '1':
                    return new int[,]
                    {
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,1,1,1,0,0,0},
                        { 0,1,1,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '2':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,0,1,1,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,1,1,0,0,0,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '3':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,1,1,1,0,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '4':
                    return new int[,]
                    {
                        { 0,0,0,1,1,1,0,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,1,1,0,0},
                        { 1,1,0,0,1,1,0,0},
                        { 1,1,1,1,1,1,1,0},
                        { 0,0,0,0,1,1,0,0},
                        { 0,0,0,0,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '5':
                    return new int[,]
                    {
                        { 0,1,1,1,1,1,1,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,1,1,1,0,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '6':
                    return new int[,]
                    {
                        { 0,0,0,1,1,1,0,0},
                        { 0,0,1,1,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '7':
                    return new int[,]
                    {
                        { 0,1,1,1,1,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,0,1,1,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '8':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case '9':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,1,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,0,0,0,1,1,0,0},
                        { 0,0,1,1,1,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                default:
                    return new int[,]
                    {
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
            }
        }
        
    }
}
