using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class Level
    {
        public Level(string name, int[,] mapData)
        {
            Name = name;
            _mapData = mapData;
        }


        public string Name { get; set; }


        private int[,] _mapData;
        public int Width => _mapData.GetLength(0);
        public int Height => _mapData.GetLength(1);
        public int GetCellValue(int x, int y) => _mapData[x, y];
        public void SetCellValue(int x, int y, int value)
        {
            _mapData[x, y] = value;
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
                    text += _mapData[i, j];
                }
                text += "\n";
            }

            return text;
        }
    }
}
