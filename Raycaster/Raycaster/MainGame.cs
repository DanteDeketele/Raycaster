using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Raycaster.Levels;
using System.Diagnostics;

namespace Raycaster
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Level[] _levels;
        private Camera _camera;

        private InputHandeler _inputHandeler;

        private Texture2D _whiteTexture;

        private bool _enableTopView = false;

        public MainGame()
        {
            
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            int upscale = 6;
            _camera = new Camera(_graphics.PreferredBackBufferWidth / upscale, _graphics.PreferredBackBufferHeight / upscale, upscale);
            _inputHandeler = new InputHandeler(new Point(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/2));

            IsMouseVisible = false;
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LevelUnpacker levelUnpacker = new LevelUnpacker();
            _levels = levelUnpacker.GetLevels();
            foreach (Level level in _levels)
            {
                Debug.WriteLine(level);
            }

            _whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _spriteBatch.Dispose();
            _whiteTexture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            _inputHandeler.Update();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.U))
                _enableTopView = true;
            if (Keyboard.GetState().IsKeyDown(Keys.I))
                _enableTopView = false;

            _camera.Position += _camera.Forward * InputHandeler.MoveDirection.Y * deltaTime;
            _camera.Position += _camera.Right * InputHandeler.MoveDirection.X * deltaTime;


            _camera.Angle += _inputHandeler.MouseChange.X * deltaTime * 0.1f;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            RaycastComputer.DrawScreen(_camera, _levels[0], _spriteBatch, _whiteTexture);
            if (_enableTopView)
                RaycastComputer.DrawTopView(_camera, _levels[0], _spriteBatch, _whiteTexture);
            _spriteBatch.End();

            Debug.WriteLine(1/deltaTime);

            base.Draw(gameTime);
        }
    }
}