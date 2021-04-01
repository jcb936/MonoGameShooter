using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace THClone
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class THGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Our Explosion Sound.
        private SoundEffect explosionSound;
        private SoundEffectInstance explosionSoundInstance;

        // Game Music.
        private Song gameMusic;

        private FSM mainLoop;

        public THGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GameInfo.GameInstance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            mainLoop = new(this);

            graphics.PreferredBackBufferHeight = 1920;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var player = new Player();
            var hud = new HUD();

            //Background
            var bgLayer1 = new ParallaxingBackground();
            var bgLayer2 = new ParallaxingBackground();

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            // load the texture to serve as the laser
            var laserTexture = Content.Load<Texture2D>("Graphics\\laser");

            // Load the laserSound Effect and create the effect Instance
            var laserSound = Content.Load<SoundEffect>("Sound\\laserFire");

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition, GraphicsDevice.Viewport, laserTexture, laserSound);

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            var mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");

            var enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");

            // load the explosion sheet
            var explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");
            Explosion.ExplosionTexture = explosionTexture;

            // Load the laserSound Effect and create the effect Instance
            explosionSound = Content.Load<SoundEffect>("Sound\\explosion");
            explosionSoundInstance = explosionSound.CreateInstance();

            // Load the game music
            gameMusic = Content.Load<Song>("Sound\\gameMusic");
            // Start playing the music.
            MediaPlayer.Play(gameMusic);

            // Load the score font
            var font = Content.Load<SpriteFont>("Graphics\\gameFont");

            // initialize HUD
            var hudTexture = Content.Load<Texture2D>("Graphics\\t_blackSquare");
            hud.Initialize(GraphicsDevice.Viewport, hudTexture, font);

            // Initialize menu state
            var mainMenuTex = Content.Load<Texture2D>("Graphics\\t_mainMenu");
            var mainMenuState = new MenuState();
            mainMenuState.Initialize(mainMenuTex, font, GraphicsDevice.Viewport);

            // Initialize game state
            var gameState = new GameState();
            gameState.Initialize(player, hud, bgLayer1, bgLayer2, enemyTexture, mainBackground, GraphicsDevice.Viewport);

            // set transitions
            mainMenuState.AddTransition(new Transition(gameState, () => mainMenuState.StartGame));

            mainLoop.AddState(mainMenuState);
            mainLoop.AddState(gameState);

            mainLoop.Initialise("MenuState");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            explosionSoundInstance.Dispose();

            //Stop playing the music
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            EntityManager.Instance.Update(gameTime);
            CommandManager.Instance.Update();
            CollisionManager.Instance.Update();

            mainLoop.Update(gameTime);

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            //player.Update(gameTime);

            // reset score if player health goes to zero
            //if (player.Health <= 0)
            //{
            //    player.Health = 100;
            //    score = 0;
            //}
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Start drawing
            spriteBatch.Begin();

            mainLoop.Draw(spriteBatch);

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
