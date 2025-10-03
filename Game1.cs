using Computer_Science_Coursework;
using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSCRefactor
{
    public class Game1 : Game
    {
        private const int viewWidth = 1600;
        private const int viewHeight = 900;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Shapes _shapes;
        private BloomFilter bloomFilter;
        private BloomFilter enemyBloomFilter;

        // game variables
        static readonly float scale = viewWidth / 320.0f;

        // game objects
        Camera camera = new Camera(viewWidth, viewHeight);
        SceneManager sceneManager;
        MagicManager magicManager;
        Player player;
        EnemyManager enemyManager;
        SoundManager soundManager;

        // time
        float timeModifier = 1.0f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "Coursework";
            _graphics.PreferredBackBufferWidth = viewWidth;
            _graphics.PreferredBackBufferHeight = viewHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _shapes = new Shapes(this);

            // Load sounds
            soundManager = new SoundManager(
                new Dictionary<string, SoundEffect>()
                {
                    { "track1", Content.Load<SoundEffect>("background_music/background_music") }
                },
                new Dictionary<string, SoundEffect>()
                {
                    { "fireball_launch", Content.Load<SoundEffect>("sound_effects/fireball_launch") },
                    { "fireball_land", Content.Load<SoundEffect>("sound_effects/fireball_land") }
                }
            );
            soundManager.PlayMusic("track1");

            // load effects
            bloomFilter = new BloomFilter();
            bloomFilter.Load(GraphicsDevice, Content, viewWidth, viewHeight);
            bloomFilter.BloomPreset = BloomFilter.BloomPresets.Focussed;
            bloomFilter.BloomStrengthMultiplier = 1.2f;
            enemyBloomFilter = new BloomFilter();
            enemyBloomFilter.Load(GraphicsDevice, Content, viewWidth, viewHeight);
            enemyBloomFilter.BloomPreset = BloomFilter.BloomPresets.Focussed;
            enemyBloomFilter.BloomStrengthMultiplier = 1.2f;

            // load textures
            Dictionary<string, Texture2D> tilesets = new Dictionary<string, Texture2D>
            {
                { "grass", Content.Load<Texture2D>("tileset") }
            };
            List<Background> backgrounds = new List<Background>
            {
                new Background(0, -50 * scale, Content.Load<Texture2D>("backgrounds/far-clouds"), 0.05f),
                new Background(0, -60 * scale, Content.Load<Texture2D>("backgrounds/far-mountains"), 0.15f),
                new Background(0, -80 * scale, Content.Load<Texture2D>("backgrounds/trees"), 0.35f)
            };
            Animator playerAnimator = new Animator( new Dictionary<string, (Frame[], string)>
                {
                    { "idle_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_idle_right"), -11, -11, 200) },
                      "idle_right") },

                    { "idle_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_idle_left"), -11, -11, 200) },
                      "idle_left") },

                    { "run_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_run_right1"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right2"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right3"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right4"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right5"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right6"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right7"), -11, -11, 70),
                                                  new Frame(Content.Load<Texture2D>("player/player_run_right8"), -11, -11, 70),},
                      "run_right")},

                    { "slow_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_slow_right1"), -11, -11, 150),
                                                   new Frame(Content.Load<Texture2D>("player/player_slow_right2"), -11, -11, float.MaxValue)},
                      "slow_right")},

                    { "run_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_run_left1"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left2"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left3"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left4"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left5"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left6"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left7"), -11, -11, 70),
                                                 new Frame(Content.Load<Texture2D>("player/player_run_left8"), -11, -11, 70),},
                      "run_left")},

                    { "slow_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_slow_left1"), -11, -11, 150),
                                                  new Frame(Content.Load<Texture2D>("player/player_slow_left2"), -11, -11, float.MaxValue)},
                      "slow_left")},

                    { "jump_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_right1"), -11, -11, 300)},
                      "jump_right")},
                    { "float_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_right2"), -11, -11, 300)},
                      "float_right")},
                    { "fall_right", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_right3"), -11, -11, 300),
                                                   new Frame(Content.Load<Texture2D>("player/player_jump_right4"), -11, -11, 300)},
                      "fall_right")},

                    { "jump_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_left1"), -11, -11, 300)},
                      "jump_left")},
                    { "float_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_left2"), -11, -11, 300)},
                      "float_left")},
                    { "fall_left", (new Frame[] { new Frame(Content.Load<Texture2D>("player/player_jump_left3"), -11, -11, 300),
                                                  new Frame(Content.Load<Texture2D>("player/player_jump_left4"), -11, -11, 300)},
                      "fall_left")}
                }, "idle_right");
            Animator enemy1Animator = new Animator(new Dictionary<string, (Frame[], string)>
            {
                { "idle_left", (new Frame[] { new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_left1"), -11, -13, 200),
                                              new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_left2"), -11, -13, 150),
                                              new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_left3"), -11, -13, 170)},
                  "idle_left")
                },
                { "idle_right", (new Frame[] { new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_right1"), -11, -13, 200),
                                               new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_right2"), -11, -13, 150),
                                               new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_idle_right3"), -11, -13, 170)},
                  "idle_right")
                },
                { "attack_left", (new Frame[] { new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left1"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left2"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left3"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left4"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left5"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left6"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left7"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left8"), -11, -13, 50),
                                                new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_left9"), -11, -13, 50)},
                  "idle_left")
                },
                { "attack_right", (new Frame[] { new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right1"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right3"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right4"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right5"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right2"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right6"), -11, -13, 100),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right7"), -11, -13, 100),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right8"), -11, -13, 50),
                                                 new Frame(Content.Load<Texture2D>("enemies/enemy1/enemy1_attack_right9"), -11, -13, 50)},
                  "idle_right")
                }
            }, "idle_left");

            // initialize objects
            player = new Player(new RectangleF(200, 900, scale * 10, scale * 21), 0.12f * scale, 0.0004f * scale,
                                0.0006f * scale, -0.0006f * scale, 0.245f * scale, playerAnimator);
            enemyManager = new EnemyManager(new List<Enemy>() {
                new Enemy(new RectangleF(1500, 900, scale * 13, scale * 20), 1000, 60, -0.0006f * scale, enemy1Animator)
            });
            sceneManager = new SceneManager(GraphicsDevice, tilesets, scale);
            magicManager = new MagicManager(GraphicsDevice, viewWidth, viewHeight, scale);

            sceneManager.LoadScene("level000", backgrounds, GraphicsDevice, _spriteBatch, viewWidth, viewHeight, scale, -1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) timeModifier = 0.3f; else timeModifier = 1;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds * timeModifier;

            player.Update(deltaTime, sceneManager.Colliders, ref magicManager, camera, ref soundManager);
            enemyManager.Update(deltaTime, player.Rectangle, sceneManager.Colliders, ref magicManager, ref soundManager);
            magicManager.Update(deltaTime, sceneManager.Colliders, sceneManager.Bounds);
            camera.Update(deltaTime, player.Rectangle.CenterX, player.Rectangle.CenterY, sceneManager.Bounds);
            sceneManager.Update(deltaTime, player, GraphicsDevice, _spriteBatch, camera);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // draw to render targets
            sceneManager.DrawRenderTargets(_shapes);
            magicManager.DrawRenderTargets(_shapes, camera, scale, GraphicsDevice, bloomFilter, enemyBloomFilter);

            // draw to back buffer
            GraphicsDevice.Clear(Color.DarkSlateGray);
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            sceneManager.DrawSceneBackground(_spriteBatch, camera);
            enemyManager.Draw(_spriteBatch, camera, scale);
            player.Draw(_spriteBatch, camera, scale, _shapes);
            sceneManager.DrawScene(_spriteBatch, camera);
            magicManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}