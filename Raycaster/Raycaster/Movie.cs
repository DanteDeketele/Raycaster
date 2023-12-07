using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Raycaster.Movies
{
    public class Movie
    {
        private Texture2D textureSheet;
        private int frameWidth;
        private int frameHeight;
        private int columns;
        private int rows;
        private int totalFrames;
        private int currentFrame;
        private float timePerFrame;
        private float frameTimer;
        private float totalAnimationTime; // Total time for the entire animation
        private float elapsedTime; // Elapsed time since the start of the animation

        public bool IsDone = false;

        public Color[] ColorData;

        public Movie(string textureSheetPath, GraphicsDevice graphicsDevice, int columns, Texture2D frames)
        {
            frameWidth = 320;
            frameHeight = 180;
            textureSheet = frames;
            LoadTextureSheet(textureSheetPath, graphicsDevice);
            frameWidth = 320;
            frameHeight = 180;
            this.columns = columns;
            rows = (int)Math.Ceiling((float)totalFrames / columns);
            timePerFrame = 1f / 16f; // Fixed framerate of 24 fps
            currentFrame = 0;
            frameTimer = 0;

            ColorData = new Color[textureSheet.Width * textureSheet.Height];
            textureSheet.GetData(ColorData);

            totalAnimationTime = timePerFrame * totalFrames;
        }

        private void LoadTextureSheet(string textureSheetPath, GraphicsDevice graphicsDevice)
        {

            totalFrames = textureSheet.Width / frameWidth * textureSheet.Height / frameHeight;
            Console.WriteLine($"Total Frames: {totalFrames}, Texture Width: {textureSheet.Width}");
        }

        public void Update(float deltatime)
        {
            if (IsDone) return;
            elapsedTime += deltatime;

            // Calculate current frame based on elapsed time and total animation time
            float normalizedTime = elapsedTime / totalAnimationTime;
            currentFrame = (int)Math.Floor(normalizedTime * totalFrames);
            if (currentFrame >= totalFrames )
            {
                IsDone = true;
                currentFrame = totalFrames-1;
            }

        }

        public void Draw(Raycaster.Camera camera, SpriteBatch spriteBatch, Texture2D whiteTexture, Point screenRes)
        {
            int col = currentFrame % columns;
            int row = currentFrame / columns;

            int sourceX = col * frameWidth;
            int sourceY = row * frameHeight;

            Rectangle sourceRectangle = new Rectangle(sourceX, sourceY,  frameWidth, frameHeight);
            //spriteBatch.Draw(textureSheet, new Rectangle(0, 0, screenRes.X, screenRes.Y), sourceRectangle, Color.White);
                RaycastComputer.DrawFrame(textureSheet, camera, screenRes, sourceRectangle, ColorData);
            
        }

    }
}
