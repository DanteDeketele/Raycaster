﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Raycaster
{
    internal class Level
    {
        public Level(string name, int[,] mapData)
        {
            Name = name;
            MapData = mapData;
        }


        public string Name { get; set; }


        public int[,] MapData { get; private set; }
        public int Width => MapData.GetLength(0);
        public int Height => MapData.GetLength(1);

        public List<Entity> entities = new List<Entity>();

        public int GetCellValue(int x, int y) => MapData[x, y];
        public void SetCellValue(int x, int y, int value)
        {
            MapData[x, y] = value;
        }

        public override string ToString()
        {
            string text = string.Empty;
            text += $"Level: {Name}\n";
            text += $"  Dimentions: {Width}x{Height}\n";
            text += $"  Data:\n";

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    text += MapData[i, j];
                }
                text += "\n";
            }

            return text;
        }

        public void DrawEntities(Point screenRes,Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture)
        {
            foreach (Entity entity in entities) {
                RaycastComputer.DrawEntity(screenRes,camera, entity, spriteBatch, whiteTexture, entity.DistanceToCamera < 4);
            }
        }
    }
}
