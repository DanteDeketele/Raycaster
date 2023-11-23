using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Raycaster.Levels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Raycaster
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Level[] _levels;
        private Camera _camera;
        private Vector2 _prevPos;
        private List<Entity> _entitiesToRemove = new List<Entity>();

        private InputHandeler _inputHandeler;

        private Texture2D _whiteTexture;
        private Texture2D _textureSheet;
        private Texture2D _glowTexture;
        private Texture2D _enemieTexture;
        private Texture2D _gunTexture;


        private Random _random = new Random();


        private float _blinkFase = 0;
        private float _walkPhase = 0;

        private bool _enableTopView = false;

        private Dictionary<string, SoundEffect> _soundEffects;

        private Point _screenRes;

        private int _gunIndex = 0;
        private float _prevWheelValue = 0;
        private bool _shootAnimation = false;
        private float _shootTime = 0;
        private int _shootFrame = 0;

        private float _fpsTimer = 0;

        private string _fpsCounter;
        private List<float> _fps = new List<float>();

        public MainGame()
        {
            
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


            // Set the preferred screen resolution
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _screenRes = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // Set the game to run in fullscreen mode
            //_graphics.IsFullScreen = true;

            //_graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            // Apply the changes
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _camera = new Camera(320, 180);
            _prevPos = _camera.Position;
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

            FileStream fileStream = new FileStream("Assets/sheet.png", FileMode.Open);
            _textureSheet = Texture2D.FromStream(GraphicsDevice, fileStream);
            fileStream.Dispose();
            FileStream fileStream1 = new FileStream("Assets/glow.png", FileMode.Open);
            _glowTexture = Texture2D.FromStream(GraphicsDevice, fileStream1);
            fileStream1.Dispose();
            FileStream fileStream2 = new FileStream("Assets/enemies.png", FileMode.Open);
            _enemieTexture = Texture2D.FromStream(GraphicsDevice, fileStream2);
            fileStream2.Dispose();
            FileStream fileStream3 = new FileStream("Assets/gunsheet.png", FileMode.Open);
            _gunTexture = Texture2D.FromStream(GraphicsDevice, fileStream3);
            fileStream3.Dispose();

            _soundEffects = AudioUnpacker.GetSounds("Assets/Sounds");
            _soundEffects["music"].Play(0.1f, 0, 0);

            _levels[0].entities.Add(new Entity(_enemieTexture, new Vector2(3.5f, 3.5f)));
            _levels[0].entities[0].State = 6;
            _levels[0].entities.Add(new Entity(_enemieTexture, new Vector2(4.5f, 3.5f), MathF.PI));
            _levels[0].entities[1].waypoints = new Vector2[] { new Vector2(5, 5), new Vector2(2, 2), new Vector2(3, 8) };
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

            if (Mouse.GetState().ScrollWheelValue > _prevWheelValue)
            {

                _gunIndex++;
                if (_gunIndex >= 3)
                    _gunIndex = 0;
            }

            if (Mouse.GetState().ScrollWheelValue < _prevWheelValue)
            {

                _gunIndex--;
                if (_gunIndex < 0)
                    _gunIndex = 3;
            }

            _prevWheelValue = Mouse.GetState().ScrollWheelValue;

            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!_shootAnimation)
                {
                    _shootAnimation = true;
                    Entity bullet = new Entity(_whiteTexture, _camera.Position + _camera.Forward * 0.01f);
                    bullet.waypoints = new Vector2[] { _camera.Forward * 10 + _camera.Position};
                    bullet.Speed = 20;
                    bullet.Size = 0.1f;
                    bullet.IsBullet = true;
                    _levels[0].entities.Add(bullet);
                }
                
            }


            float radius = 0.3f;

            int[,] map = _levels[0].MapData;

            float speed = 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                speed = 1.6f;

            Vector2 moveDirForward = _camera.Forward * InputHandeler.MoveDirection.Y * speed * 2;
            Vector2 moveDirRight = _camera.Right * InputHandeler.MoveDirection.X * speed*2;

            if (MathF.Abs(moveDirForward.X) + MathF.Abs(moveDirForward.Y) + MathF.Abs(moveDirRight.X) + MathF.Abs(moveDirRight.Y) != 0)
            {
                _walkPhase += deltaTime * 2;
                if (_walkPhase > 1)
                {
                    _walkPhase = 0;
                }
            }
            else
            {
                _walkPhase = 0;
            }

            try
            {

                /*if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirForward.X > 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirForward.X < 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirForward.Y > 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirForward.Y < 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirRight.X > 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirRight.X < 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirRight.Y > 0)
                    moveDirRight.Y = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirRight.Y < 0)
                    moveDirRight.Y = 0;

                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirForward.X > 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirForward.X < 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirForward.Y > 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirForward.Y < 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirRight.X > 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirRight.X < 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y + radius)] == 1 && moveDirRight.Y > 0)
                    moveDirRight.Y = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y - radius)] == 1 && moveDirRight.Y < 0)
                    moveDirRight.Y = 0;*/

                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 0 && moveDirForward.X > 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 0 && moveDirForward.X < 0)
                    moveDirForward.X = 0;
                if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 0 && moveDirForward.Y > 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 0 && moveDirForward.Y < 0)
                    moveDirForward.Y = 0;
                if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 0 && moveDirRight.X > 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 0 && moveDirRight.X < 0)
                    moveDirRight.X = 0;
                if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 0 && moveDirRight.Y > 0)
                    moveDirRight.Y = 0;
                if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 0 && moveDirRight.Y < 0)
                    moveDirRight.Y = 0;

                _camera.Position += moveDirForward * deltaTime;
                _camera.Position += moveDirRight * deltaTime;

            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message + " | " + ex.Source, "ERROR");
                Exit();
            }


            _camera.Angle += _inputHandeler.MouseChange.X * deltaTime * 0.1f;

            if (_camera.Angle > MathF.PI * 2)
                _camera.Angle -= MathF.PI * 2;

            if (_camera.Angle < 0)
                _camera.Angle += MathF.PI * 2;

            //_blinkFase = 1f + 0.5f * (float)Math.Sin(2 * Math.PI * 0.75f * gameTime.TotalGameTime.TotalSeconds);

            _blinkFase += deltaTime * 0.5f;
            while (_blinkFase > 1)
                _blinkFase--;

            // https://sound-works-12.itch.io/footsteps-small-sound-pack

            if (Vector2.Distance(_prevPos, _camera.Position) > 1f)
            {
                _prevPos = _camera.Position;
                _soundEffects["S_Stone_Mono_" + _random.Next(1,20)].Play((float)_random.NextDouble() * 0.3f + 0.2f, 0, 0);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (map[(int)(_camera.Position.X + _camera.Forward.X * 0.5f), (int)(_camera.Position.Y + _camera.Forward.Y * 0.5f)] == 100)
                {
                    map[(int)(_camera.Position.X + _camera.Forward.X * 0.5f), (int)(_camera.Position.Y + _camera.Forward.Y * 0.5f)] = 0;
                    _soundEffects["beep"].Play();
                }
            }

            foreach (var entity in _levels[0].entities)
            {
                entity.Update(deltaTime, _camera);
                if (!entity.Active)
                {
                    _entitiesToRemove.Add(entity);
                }
            }

            foreach(var entity in _entitiesToRemove)
            {
                _levels[0].entities.Remove(entity);
            }
            _entitiesToRemove.Clear();

            _camera.Render = !Keyboard.GetState().IsKeyDown(Keys.R);


            _camera.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Add your drawing code here
            _spriteBatch.Begin();


            if (_shootAnimation)
            {
                _shootTime += deltaTime * 20f;

                _shootFrame = (int)_shootTime;
                if (_shootTime >= 5)
                {
                    
                    _shootAnimation = false;
                    _shootTime = 0;
                    _shootFrame = 0;
                }
            }


            RaycastComputer.DrawGun(_screenRes, _camera, _gunIndex, _shootFrame, _spriteBatch, _gunTexture, _whiteTexture);

            //_spriteBatch.Draw(_gunTexture, new Rectangle(0, 0, 600, 600), new Rectangle(_gunTexture.Width / 6, _gunTexture.Width / 6, _gunTexture.Width/ 6, _gunTexture.Height/ 6), Color.White);

            RaycastComputer.DrawScreen(_screenRes,_camera, _levels[0], _spriteBatch, _textureSheet, _whiteTexture, _glowTexture, _blinkFase);
            _levels[0].DrawEntities(_screenRes,_camera, _spriteBatch, _whiteTexture);
            RaycastComputer.DrawEntityOutlines(_screenRes, _camera, _spriteBatch, _whiteTexture);

            if (_enableTopView)
                RaycastComputer.DrawTopView( _camera, _levels[0], _spriteBatch, _whiteTexture);

            RaycastComputer.DrawGunOutlines(_screenRes, _camera, _spriteBatch, _whiteTexture);

            _fps.Add(1 / deltaTime);

            _fpsTimer += deltaTime;

            if (_fpsTimer > 1)
            {
                float fps = 0;

                foreach (var f in _fps)
                {
                    fps += f;
                }

                fps /= _fps.Count;

                _fpsCounter = MathF.Round(fps).ToString();
                _fpsTimer = 0;
                _fps.Clear();
            }

            
            RaycastComputer.DrawFont(_fpsCounter, new Point(20,20), _screenRes, _camera, _whiteTexture, _spriteBatch);

            _spriteBatch.End();

            //Debug.WriteLine(1/deltaTime);
            

            _camera.ClearEntityBuffer();

            base.Draw(gameTime);
        }
    }
}