using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raycaster
{
    internal struct Image
    {
        private readonly ushort[,] _pixels;
        private readonly int _width;
        private readonly int _height;

        public Image(int width, int height, ushort[,] pixels)
        {
            _width = width;
            _height = height;
            _pixels = pixels;
        }

        public ushort GetBrightness(int x, int y)
        {
            return _pixels[x, y];
        }

        public bool IsOpaque(int x, int y)
        {
            return _pixels[x, y] == 255;
        }

        public int Width => _width;
        public int Height => _height;

        public static Image TextureToImage(Texture2D texture)
        {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            ushort[,] pixels = new ushort[texture.Width, texture.Height];

            for (int i = 0; i < texture.Width; i++)
            {
                for (int j = 0; j < texture.Height; j++)
                {
                    Color color = colorData[i+j*texture.Width];

                    if (color.A != 255)
                    {
                        pixels[i, j] = 255;
                    }
                    else
                    {
                        int brightness = 19;
                        int br = (int)(0.21 * color.R + 0.69 * color.G + 0.15 * color.B);

                        br = 255 - br;
                        br *= 17;
                        br /= 255;

                        brightness -= br;

                        pixels[i, j] = (ushort)brightness;
                    }
                }
            }

            Image image = new Image(texture.Width, texture.Height, pixels);
            return image;
        }
    }
}
