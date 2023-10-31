using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class FPSCounter
    {
        public static void Draw(float fps, SpriteBatch spriteBatch, Texture2D textureSheet)
        {
        }

        private static int[,] Number(int i)
        {
            switch (i)
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
