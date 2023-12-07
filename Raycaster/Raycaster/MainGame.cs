﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Raycaster.Levels;
using Raycaster.Sounds;
using Raycaster.Movies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FFmpeg.AutoGen;

namespace Raycaster
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Level[] _levels;
        private int _currentLevelId = 0;
        private Camera _camera;
        private Vector2 _prevPos;
        private List<Entity> _entitiesToRemove = new List<Entity>();

        private InputHandeler _inputHandeler;

        private Texture2D _whiteTexture;
        private Texture2D _skyTexture;
        private Texture2D _textureSheet;
        private Texture2D _glowTexture;
        private Texture2D _enemieTexture;
        private Texture2D _gunTexture;
        private Texture2D _movieTexture;



        private Random _random = new Random();


        private float _blinkFase = 0;
        private float _walkPhase = 0;

        private bool _enableTopView = false;

        private Dictionary<string, SoundEffect> _soundEffects;
        private Dictionary<string, SoundEffect> _movieSoundEffects;


        private Point _screenRes;

        private int _gunIndex = 0;
        private float _prevWheelValue = 0;
        private bool _shootAnimation = false;
        private float _shootTime = 0;
        private int _shootFrame = 0;

        private float _fpsTimer = 0;

        private float _fpsCounter;
        private List<float> _fps = new List<float>();

        private List<Enemy> _enemies = new List<Enemy>();

        private Movie _introMovie;
        private bool _paused = false;
        private bool _loadMovie = false;
        private bool _loadedMovie = false;

        private bool _loadMovieQue = false;
        private string _loadMovieQueName;
        private bool _drawnLoadingScreen = false;
        private int _waitedAFrameLoadingScreen = 0;


        Color[] _colorData;
        Color[] _colorSkyData;
        Color[] _colorGlowData;
        Color[] _colorEnemyData;
        Color[] _colorGunData;


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
            _camera = new Camera(320, 180, GraphicsDevice);
            _camera.GrayScale = true;
            //_camera = new Camera(1560/10, 1440/10, GraphicsDevice);

            _prevPos = _camera.Position;
            _inputHandeler = new InputHandeler(new Point(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2));

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
            FileStream fileStream0 = new FileStream("Assets/sky.png", FileMode.Open);
            _skyTexture = Texture2D.FromStream(GraphicsDevice, fileStream0);
            fileStream0.Dispose();
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
            //_soundEffects["music"].Play(0.1f, 0, 0);
            _movieSoundEffects = AudioUnpacker.GetSounds("Assets/Movies");

            Enemy enemy1 = new Enemy(_enemieTexture, new Vector2(1.5f, 7.5f), _soundEffects);
            enemy1.State = 6;
            enemy1.IsStaticSprite = true;
            enemy1.StaticSprite = 47;
            _levels[_currentLevelId].entities.Add(enemy1);
            _enemies.Add(enemy1);

            Enemy enemy2 = new Enemy(_enemieTexture, new Vector2(8.5f, 6.5f), _soundEffects, MathF.PI);
            enemy2.waypoints = new Vector2[] { new Vector2(8.5f, 1.5f), new Vector2(8.5f, 6.5f) };
            _levels[_currentLevelId].entities.Add(enemy2);
            _enemies.Add(enemy2);

            _loadMovieQue = true;
            _loadMovieQueName = "intro";
            _paused = true;
            _drawnLoadingScreen = false;
            _waitedAFrameLoadingScreen = 0;

            _colorData = new Color[_textureSheet.Width * _textureSheet.Height];
            _textureSheet.GetData(_colorData);
            _colorSkyData = new Color[_skyTexture.Width * _skyTexture.Height];
            _skyTexture.GetData(_colorSkyData);

            _colorGlowData = new Color[_glowTexture.Width * _glowTexture.Height];
            _glowTexture.GetData(_colorGlowData);

            _colorEnemyData = new Color[_enemieTexture.Width * _enemieTexture.Height];
            _enemieTexture.GetData(_colorEnemyData);

            _colorGunData = new Color[_gunTexture.Width * _gunTexture.Height];
            _gunTexture.GetData(_colorGunData);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _spriteBatch.Dispose();
            _whiteTexture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.S) && _introMovie != null)
            {
                _introMovie = null;
                _paused = false;
                _camera.RenderLoaded = 0;
            }
                

            if (_loadMovieQue && _drawnLoadingScreen && _waitedAFrameLoadingScreen > 2)
            {
                _paused = true;
                _loadMovieQue = false;
                _drawnLoadingScreen = false;
                _waitedAFrameLoadingScreen = 0;
                LoadMovie(_loadMovieQueName);
            }


            _introMovie?.Update(deltaTime);
            if (_introMovie != null && _introMovie.IsDone)
            {
                _introMovie = null;
                _paused = false;
                _camera.RenderLoaded = 0;
            }

            _inputHandeler.Update();

            

            

            if (!_paused)
            {
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
                        Bullet bullet = new Bullet(_whiteTexture, _camera.Position + _camera.Forward * 0.01f, _enemies.ToArray(), 1);
                        bullet.waypoints = new Vector2[] { _camera.Forward * 10 + _camera.Position };
                        bullet.Speed = 20;
                        bullet.Size = 0.1f;
                        bullet.IsBullet = true;
                        _levels[_currentLevelId].entities.Add(bullet);
                        _soundEffects["shot"].Play(0.1f, 0, 0);
                    }

                }


                float radius = 0.3f;

                int[,] map = _levels[_currentLevelId].MapData;

                float speed = 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    speed = 1.6f;

                Vector2 moveDirForward = _camera.Forward * InputHandeler.MoveDirection.Y * speed * 2;
                Vector2 moveDirRight = _camera.Right * InputHandeler.MoveDirection.X * speed * 2;

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

                    if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 0 && map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 66 && moveDirForward.X > 0)
                        moveDirForward.X = 0;
                    if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 0 && map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 66 && moveDirForward.X < 0)
                        moveDirForward.X = 0;
                    if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 0 && map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 66 && moveDirForward.Y > 0)
                        moveDirForward.Y = 0;
                    if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 0 && map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 66 && moveDirForward.Y < 0)
                        moveDirForward.Y = 0;
                    if (map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 0 && map[(int)(_camera.Position.X + radius), (int)(_camera.Position.Y)] != 66 && moveDirRight.X > 0)
                        moveDirRight.X = 0;
                    if (map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 0 && map[(int)(_camera.Position.X - radius), (int)(_camera.Position.Y)] != 66 && moveDirRight.X < 0)
                        moveDirRight.X = 0;
                    if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 0 && map[(int)(_camera.Position.X), (int)(_camera.Position.Y + radius)] != 66 && moveDirRight.Y > 0)
                        moveDirRight.Y = 0;
                    if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 0 && map[(int)(_camera.Position.X), (int)(_camera.Position.Y - radius)] != 66 && moveDirRight.Y < 0)
                        moveDirRight.Y = 0;

                    _camera.Position += moveDirForward * deltaTime;
                    _camera.Position += moveDirRight * deltaTime;

                    if (map[(int)(_camera.Position.X), (int)(_camera.Position.Y)] == 66)
                    {
                        _loadMovieQue = true;
                        _loadMovieQueName = "prologue";
                        _camera.Position = new Vector2(1.5f, 1.5f);
                        _currentLevelId++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + " | " + ex.Source, "ERROR");
                    Exit();
                }

                _camera.RollAngle = MathHelper.Lerp(_camera.RollAngle, -InputHandeler.MoveDirection.X * speed * 1.1f, deltaTime * 15);

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
                    _soundEffects["S_Stone_Mono_" + _random.Next(1, 20)].Play((float)_random.NextDouble() * 0.3f + 0.2f, 0, 0);
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (map[(int)(_camera.Position.X + _camera.Forward.X * 0.5f), (int)(_camera.Position.Y + _camera.Forward.Y * 0.5f)] == 100)
                    {
                        map[(int)(_camera.Position.X + _camera.Forward.X * 0.5f), (int)(_camera.Position.Y + _camera.Forward.Y * 0.5f)] = 0;
                        _soundEffects["beep"].Play();
                    }
                }

                foreach (var entity in _levels[_currentLevelId].entities)
                {
                    entity.Update(deltaTime, _camera);

                    if (entity is Bullet)
                    {
                        if (_levels[_currentLevelId].MapData[(int)entity.Position.X, (int)entity.Position.Y] != 0)
                        {
                            entity.Active = false;
                            _soundEffects["shot"].Play(0.05f, 1f, 0);
                        }
                    }

                    if (!entity.Active)
                    {
                        _entitiesToRemove.Add(entity);
                    }
                }

                foreach (var entity in _entitiesToRemove)
                {
                    _levels[_currentLevelId].entities.Remove(entity);
                    if (entity is Enemy)
                    {
                        _enemies.Remove(entity as Enemy);
                    }
                }
            }
            _entitiesToRemove.Clear();

            
            
                
            _camera.Render = !Keyboard.GetState().IsKeyDown(Keys.R);
            _camera.Update(deltaTime);

            base.Update(gameTime);
        }

        private void LoadMovie(string movieName)
        {
            FileStream fileStream4 = new FileStream($"Assets/Movies/{movieName}.png", FileMode.Open);
            _movieTexture = Texture2D.FromStream(GraphicsDevice, fileStream4);
            fileStream4.Dispose();
            _introMovie = new Movie($"Assets/Movies/{movieName}.png", _graphics.GraphicsDevice, 100, _movieTexture);
            _movieSoundEffects[movieName].Play(0.1f, 0, 0);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend
        );

            if (_drawnLoadingScreen)
            {
                _waitedAFrameLoadingScreen++;
            }

            if (!_paused && !_loadMovieQue)
            {

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

                

                RaycastComputer.DrawGun(_screenRes, _camera, _gunIndex, _shootFrame, _gunTexture, _colorGunData);

                RaycastComputer.DrawScreen(_screenRes, _camera, _levels[_currentLevelId],  _textureSheet, _glowTexture, _blinkFase, _colorData, _colorGlowData);
                RaycastComputer.DrawFrame(_skyTexture, _camera, _screenRes, new Rectangle(0, 0, _camera.Width, _camera.Height), _colorSkyData, true);
                _levels[_currentLevelId].DrawEntities(_screenRes, _camera, _colorEnemyData);

                RaycastComputer.DrawEntityOutlines(_screenRes, _camera);


                if (_enableTopView)
                    RaycastComputer.DrawTopView(_camera, _levels[_currentLevelId], _spriteBatch, _whiteTexture);

                

            }
           
            

            _fps.Add(deltaTime);
            _fpsTimer += deltaTime;
            if (_fpsTimer > 1 && !_loadMovieQue)
            {
                _fpsTimer = 0;
                _fpsCounter = 0;
                foreach (var item in _fps)
                {
                    _fpsCounter += 1/item;
                }
                _fpsCounter /= _fps.Count;
                _fps.Clear();
            }

            RaycastComputer.DrawFont(((int)(_fpsCounter)).ToString(), new Point(1,1), _screenRes, _camera);
          
            
            if (_paused)
            {
                RaycastComputer.DrawFont("S TO SKIPP", new Point(0,_camera.Height - 16), _screenRes, _camera);
            }
            
            RaycastComputer.DrawGunOutlines(_screenRes, _camera);
                
            _camera.Draw(_spriteBatch, _screenRes);
            
            _introMovie?.Draw(_camera, _spriteBatch, _whiteTexture, _screenRes);

            if (_loadMovieQue)
            {
                _camera.RenderLoaded = 1;
                _camera.Clear();
                RaycastComputer.DrawFont("LOADING...", new Point(0, _camera.Height - 16), _screenRes, _camera);
                RaycastComputer.DrawGunOutlines(_screenRes, _camera);
                _camera.Draw(_spriteBatch, _screenRes);
                _drawnLoadingScreen = true;
            }

            

            _camera.ClearEntityBuffer();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}