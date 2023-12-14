using FFmpeg.AutoGen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.MediaFoundation;
using System;
using System.Diagnostics;

namespace Raycaster
{
    internal static class RaycastComputer
    {
        public static void DrawScreen(Point screenRes,Camera camera, Level level, Image textureSheet, Image glowTexture, float interactibleFase, bool renderAllWalls = false)
        {
            int width = camera.Width;
            int height = camera.Height;

            int[,] map = level.MapData;

            Vector2 position = camera.Position;

            Vector2 playerDir = camera.Forward;

            // Use Parallel.For to parallelize the outer loop
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

                bool startedInsideWall = true;

                while (!hit)
                {
                    if ((int)position.X < 0 || (int)position.X >= map.GetLength(0) || (int)position.Y < 0 || (int)position.Y >= map.GetLength(1))
                        break;
                    // Check if the initial position is inside a wall
                    if (startedInsideWall && (map[mapX, mapY] == 0 || map[mapX, mapY] == 67))
                    {
                        startedInsideWall = false;
                    }
                    
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

                    if (map[mapX, mapY] > 0 && map[mapX, mapY] != 67 && !startedInsideWall)
                    {
                        hit = true;

                        if (!renderAllWalls)
                            break; // Break if renderAllWalls is not set
                    }
                }


                if (!hit)
                    return;

                if (side == 0)
                    perpWallDist = (mapX - position.X + (1 - stepX) / 2) / rayDir.X;
                else
                    perpWallDist = (mapY - position.Y + (1 - stepY) / 2) / rayDir.Y;

                camera.DepthBuffer[i] = (float)perpWallDist;

                int projectedHeight = (int)((height) / perpWallDist);

                int drawStart = Math.Max(0, (height - projectedHeight) / 2)-(int)(level.heightOffset* height / perpWallDist);
                int drawEnd = Math.Min(height - 1, (height + projectedHeight) / 2) - (int)(level.heightOffset * height / perpWallDist);

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


                    brightness -= (int)(perpWallDist * 0.5f);

                    if (brightness < 0)
                        brightness = 0;
                    if (brightness > 19)
                        brightness = 19;

                    ushort c = GetTextureColor(textureSheet, texX, texY, GetWallTextureRect(map[mapX, mapY]));


                    if (map[mapX, mapY] == 100)
                    {
                        ushort cGlow = GetGlowTextureColor(glowTexture, texXglow, texYglow, interactibleFase);

                        if (cGlow > 5)
                        {
                            c += 2;
                        }
                    }



                    brightness = c;

                    int[,] patern = PixelPattern(brightness);

                    if (map[mapX, mapY] == 66)
                    {
                        patern = PixelPattern(18);
                    }

                    int y1 = y + (int)((i - camera.Width / 2) * camera.RollAngle * 0.02f);
                    RenderPixelPattern(screenRes, camera, i, y1, patern, brightness);



                    if (y1 >= 0 && y1 < camera.Height - 1)
                    {
                        camera.RenderedBuffer[i, y1] = true;
                    }

                    //float virtualpixelSize = (screenRes.X / camera.Width);
                    //Vector2 distortion = screenRes.ToVector2() - new Vector2(camera.Width * (int)(virtualpixelSize), camera.Height * (int)(virtualpixelSize));
                    //spriteBatch.Draw(whiteTexture, new Rectangle(i*8, y*8 + (int)((i - camera.Width/2)*3*camera.RollAngle*0.02f), 8, 8), c);
                }

                for (int y = drawEnd + 1; y < height; y++)
                {
                    if (y < 0 || y >= height || i < 0 || i >= width)
                        continue;
                    camera.RenderedBuffer[i, y] = true;
                    // Calculate the ray direction for the floor
                    Vector2 floorRayDir = new Vector2(
                        playerDir.X + camera.Right.X * (2 * i / (float)width - 1),
                        playerDir.Y + camera.Right.Y * (2 * i / (float)width - 1));

                    // Calculate the current distance to the floor
                     double currentDist = height / (2.0 * y - height);

                    // Calculate the position of the floor intersection
                    Vector2 floorIntersection = position + (float)currentDist * floorRayDir;
                    if ((int)floorIntersection.Y < 0 || (int)floorIntersection.Y >= map.GetLength(1) || (int)floorIntersection.X < 0 || (int)floorIntersection.X >= map.GetLength(0))
                        continue;
                    if (map[(int)floorIntersection.X, (int)floorIntersection.Y] != 0)
                        continue;

                    // Get the texture coordinates for the floor
                    int floorTexX = (int)(floorIntersection.X * textureSheet.Width) % textureSheet.Width;
                    int floorTexY = (int)(floorIntersection.Y * textureSheet.Height) % textureSheet.Height;

                    // Render the floor pixel
                    ushort floorColor = GetTextureColor(textureSheet, floorTexX, floorTexY, GetWallTextureRect(3)); // Adjust this based on your floor texture


                    int brightness = floorColor;

                    int[,] patern = PixelPattern(brightness);

                    int y1 = y + (int)((i - camera.Width/2) * camera.RollAngle * 0.02f);
                    if (y1 >= 0 && y1 < camera.Height - 1)
                    {
                        camera.RenderedBuffer[i, y1] = true;
                    }
                    RenderPixelPattern(screenRes, camera, i, y1, patern, brightness);
                }
            };
        }

        public static Point ActualUsedRegion(Camera camera, Point screenRes)
        {
            float virtualpixelSize = (screenRes.X / camera.Width) / 3 + 1;

            float x = screenRes.X - virtualpixelSize * camera.Width;
            float y = screenRes.Y - virtualpixelSize * camera.Height;
            return new Point((int)(x),(int)( y));
        }

        private static void RenderPixelPattern(Point screenRes, Camera camera, int x, int y, int[,] patern, int brightness)
        {
            if (x < 0 || x >= camera.Width || y < 0 || y >= camera.Height)
                return;

            if (camera.RenderLoadBuffer[x, y] > camera.RenderLoaded)
                return;

            if (!camera.RenderWorldBuffer[x, y])
                return;

            float virtualpixelSize = (screenRes.X / camera.Width) / 3 + 1;

            Vector2 distortion = screenRes.ToVector2() - new Vector2(camera.Width * (int)(virtualpixelSize * 3), camera.Height * (int)(virtualpixelSize * 3));

            distortion = -distortion;

            if (distortion.X > 0)
            {
                //virtualpixelSize++;
            }
            
            if (camera.GrayScale)
            {
                int br = brightness * 255/19;
                camera.SetColor(new Color(br,br,br), x * 3, y * 3, 3);
            }
            else
            {
                for (int x1 = 0; x1 < 3; x1++)
                {
                    for (int y1 = 0; y1 < 3; y1++)
                    {
                        camera.SetColor(patern[x1, y1] != 1, x1 + x * 3, y1 + y * 3);
                        
                        //Color color = patern[x1, y1] == 1 ? Color.White : Color.Black;

                        //spriteBatch.Draw(whiteTexture, new Rectangle((int)(x * virtualpixelSize * 3 + x1 * virtualpixelSize - distortion.X/2), (int)(y * virtualpixelSize * 3 + y1 * virtualpixelSize - distortion.Y / 2), (int)virtualpixelSize, (int)virtualpixelSize), color);
                    }
                }
            }
        }

        public static void DrawFrame(Image frame, Camera camera, Point screenRes, Rectangle source, bool scroll = false)
        {
            Random rand = new Random(0);

            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    

                    int offset = i;
                    if (scroll)
                    {
                        offset += (int)(camera.Angle / (2 * MathF.PI) * source.Width * MathF.PI);
                        if (camera.RenderedBuffer[i, j])
                            continue;
                    }

                    if (offset >= source.Width)
                    {
                        offset -= source.Width;
                    }
                    if (offset < 0)
                    {
                        offset += source.Width;
                    }
                    if (offset >= source.Width)
                    {
                        offset -= source.Width;
                    }
                    if (offset < 0)
                    {
                        offset += source.Width;
                    }
                    if (offset >= source.Width)
                    {
                        offset -= source.Width;
                    }
                    if (offset < 0)
                    {
                        offset += source.Width;
                    }

                    ushort c = GetTextureColor(frame, (offset) * (frame.Width / source.Width), j * (frame.Height / source.Height), source);

                    int br = c;
                    float fade = br % 19;

                    

                    if (rand.Next(0, 255) < fade)
                    {
                        br++;
                    }

                    int[,] pattern = PixelPattern(br);
                    RenderPixelPattern(screenRes, camera, i, j, pattern, br);
                }
            }
        }



        private static ushort GetTextureColor(Image texture, int x, int y, Rectangle source)
        {
            int texX = x * source.Width / texture.Width + source.X;
            int texY = y * source.Height / texture.Height + source.Y;

            if (texX >= source.Left && texX < source.Right && texY >= source.Top && texY <= source.Bottom)
            {
                int sourceX = texX - source.Left;
                int sourceY = texY - source.Top;
                return texture.GetBrightness(sourceX + source.Left,sourceY + source.Top);
            }

            return 19;
        }

        private static ushort GetGlowTextureColor(Image texture, int x, int y, float glowProgress)
        {

            x += (int)(glowProgress * texture.Width);
            x %= texture.Width;

            if (x >= 0 && x < texture.Width && y >= 0 && y < texture.Height)
            {
                return texture.GetBrightness(x, y);
            }

            return 19;
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

        

        public static void DrawEntity(Point screenRes, Camera camera, Entity entity, bool outlined = false)
        {
            float angleDifference = MathF.Atan2(entity.Position.Y - camera.Position.Y, entity.Position.X - camera.Position.X) - camera.Angle;
            if (angleDifference < -MathF.PI)
            {
                angleDifference += 2 * MathF.PI;
            }
            else if (angleDifference > MathF.PI)
            {
                angleDifference -= 2 * MathF.PI;
            }

            

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

                    ushort c = GetTextureColor(entity.Texture, (int)(i* distance*5), (int)(j * distance*5), new Rectangle(entity.Texture.Width / 8 * (spriteId%8), entity.Texture.Height / 7 * (spriteId/8), entity.Texture.Width/8, entity.Texture.Height/7));

                    if (c == 255)
                        continue;

                    int br = 19 - c;

                    brightness -= br;

                    int[,] patern = PixelPattern(brightness);
                    RenderPixelPattern(screenRes, camera, xPos, yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f), patern, brightness);
                    //camera.RenderedBuffer[xPos, yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)] = true;

                    if (outlined && !entity.IsBullet && (yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)) > 0 && (yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)) < camera.Height-1)
                    {
                        camera.EntityBuffer[xPos, yPos + (int)((xPos - camera.Width / 2) * camera.RollAngle * 0.02f)] = distance;
                    }
                }
            }
        }

        public static void DrawEntityOutlines(Point screenRes, Camera camera)
        {
            for (int i = 1; i < camera.Width - 1; i++)
            {
                for (int j = 1; j < camera.Height - 1; j++)
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
                            int[,] patern = PixelPattern(19);
                            RenderPixelPattern(screenRes, camera, i, j, patern, 19);
                        }
                    }
                }
            };
        }

        public static void DrawGunOutlines(Point screenRes, Camera camera)
        {
            for (int i = 1; i < camera.Width - 1; i++)
            {
                for (int j = 1; j < camera.Height - 1; j++)
                {
                    if (camera.RenderWorldBuffer[i, j])
                    {
                        if (!camera.RenderWorldBuffer[i + 1, j] ||
                            !camera.RenderWorldBuffer[i + 1, j + 1] ||
                            !camera.RenderWorldBuffer[i + 1, j - 1] ||
                            !camera.RenderWorldBuffer[i, j + 1] ||
                            !camera.RenderWorldBuffer[i, j - 1] ||
                            !camera.RenderWorldBuffer[i - 1, j] ||
                            !camera.RenderWorldBuffer[i - 1, j + 1] ||
                            !camera.RenderWorldBuffer[i - 1, j - 1])
                        {
                            int[,] patern = PixelPattern(19);
                            RenderPixelPattern(screenRes, camera, i, j, patern, 19);
                        }
                    }
                }

            };
        

            for (int i = 1; i < camera.Width - 1; i++)
            {
                int j = 0;
                if (camera.RenderWorldBuffer[i, j])
                {
                    if (!camera.RenderWorldBuffer[i + 1, j] ||
                        !camera.RenderWorldBuffer[i + 1, j + 1] ||
                        !camera.RenderWorldBuffer[i, j + 1] ||
                        !camera.RenderWorldBuffer[i - 1, j] ||
                        !camera.RenderWorldBuffer[i - 1, j + 1])
                    {
                        int[,] patern = PixelPattern(19);
                        RenderPixelPattern(screenRes, camera,  i, j, patern,19);
                    }
                }

                j = camera.Height-1;
                if (camera.RenderWorldBuffer[i, j])
                {
                    if (!camera.RenderWorldBuffer[i + 1, j] ||
                        !camera.RenderWorldBuffer[i + 1, j - 1] ||
                        !camera.RenderWorldBuffer[i, j - 1] ||
                        !camera.RenderWorldBuffer[i - 1, j] ||
                        !camera.RenderWorldBuffer[i - 1, j - 1])
                    {
                        int[,] patern = PixelPattern(19);
                        RenderPixelPattern(screenRes, camera, i, j, patern,19);
                    }
                }
            }

            for (int j = 1; j < camera.Height - 1; j++)
            {
                int i = 0;
                if (camera.RenderWorldBuffer[i, j])
                {
                    if (!camera.RenderWorldBuffer[i + 1, j] ||
                        !camera.RenderWorldBuffer[i + 1, j + 1] ||
                        !camera.RenderWorldBuffer[i + 1, j - 1] ||
                        !camera.RenderWorldBuffer[i, j + 1] ||
                        !camera.RenderWorldBuffer[i, j - 1])
                    {
                        int[,] patern = PixelPattern(19);
                        RenderPixelPattern(screenRes, camera, i, j, patern, 19);
                    }
                    i = camera.Width - 1;
                    if (camera.RenderWorldBuffer[i, j])
                    {
                        if (!camera.RenderWorldBuffer[i - 1, j] ||
                            !camera.RenderWorldBuffer[i - 1, j + 1] ||
                            !camera.RenderWorldBuffer[i - 1, j - 1] ||
                            !camera.RenderWorldBuffer[i, j + 1] ||
                            !camera.RenderWorldBuffer[i, j - 1])
                        {
                            int[,] patern = PixelPattern(19);
                            RenderPixelPattern(screenRes, camera, i, j, patern, 19);
                        }
                    }
                }
            }

            
        }

        public static void DrawGun(Point screenRes, Camera camera, int gun, int frame, Image texture)
        {
            Color transparent = new Color(152, 0, 136);

            int size = camera.Height / 2;

            

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int brightness = 19;

                    ushort c = GetTextureColor(texture, x * (texture.Width/size), y * (texture.Height / size), new Rectangle((texture.Width / 5)*(frame)+frame, (texture.Height / 4)*gun+ gun, texture.Width / 4-4-1, texture.Height / 3));

                    if (c == 255)
                        continue;

                    int br = 19 - c;

                    brightness -= br;

                    int[,] patern = PixelPattern(brightness);
                    RenderPixelPattern(screenRes, camera, x + camera.Width/2 - (int)(size*0.5f), y+camera.Height-size, patern, brightness);
                    camera.RenderWorldBuffer[x + camera.Width / 2 -(int)(size * 0.5f), y+camera.Height - size] = false;
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


        /*public static void DrawTopView(Camera camera, Level level, SpriteBatch spriteBatch)
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
        }*/

        private static int[,] PixelPattern(int type)
        {
            if (type < 0)
                type = 0;
            if (type > 19)
                type = 19;

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
                        { 1, 0, 0},
                        { 0, 0, 0},
                        { 0, 0, 0}
                    };
                case 2:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 1, 0},
                        { 0, 0, 0}
                    };
                case 3:
                    return new int[,] {
                        { 1, 0, 0},
                        { 0, 0, 0},
                        { 0, 0, 1}
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
                        { 1, 1, 0},
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

        public static void DrawFont(string text, Point topLeft, Point screenRes, Camera cam)
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
                        if (fontPatern[y, x] == 0)
                            continue;
                        int[,] patern = PixelPattern(19-fontPatern[y,x]*19);
                        
                        RenderPixelPattern(screenRes, cam, topLeft.X + x + i * 8, topLeft.Y + y, patern, 19 - fontPatern[y, x] * 19);
                        cam.RenderWorldBuffer[topLeft.X + x + i * 8, topLeft.Y + y] = false;
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
                case '.':
                    return new int[,]
                    {
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'A':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'P':
                    return new int[,]
                    {
                        { 0,1,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,1,1,1,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'L':
                    return new int[,]
                    {
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'I':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'O':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'S':
                    return new int[,]
                    {
                        { 0,0,1,1,1,1,1,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,0,0,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,0,1,1,1,1,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'T':
                    return new int[,]
                    {
                        { 0,1,1,1,1,1,1,0},
                        { 0,1,1,1,1,1,1,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,1,1,0,0,0},
                        { 0,0,0,0,0,0,0,0}
                    };
                case 'K':
                    return new int[,]
                    {
                        { 0,1,1,0,0,0,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,1,1,1,0,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
                        { 0,1,1,0,0,1,1,0},
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
