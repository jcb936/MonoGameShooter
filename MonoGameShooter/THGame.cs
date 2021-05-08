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

            // Load the game music
            gameMusic = Content.Load<Song>("Sound\\gameMusic");
            // Start playing the music.
            MediaPlayer.Play(gameMusic);

            // Initialize menu state
            var mainMenuState = new MenuState();
            mainMenuState.Initialize(GraphicsDevice.Viewport);

            // Initialize game state
            var gameState = new GameState();
            gameState.Initialize(GraphicsDevice.Viewport);

            // Initialize pause state
            var pauseState = new PauseState();
            pauseState.Initialise(GraphicsDevice.Viewport);

            // set transitions
            mainMenuState.AddTransition(new Transition(gameState, () => mainMenuState.StartGame));
            gameState.AddTransition(new Transition(mainMenuState, () => gameState.GameOver));
            gameState.AddTransition(new Transition(pauseState, () => gameState.Pausing));
            pauseState.AddTransition(new Transition(gameState, () => pauseState.Resume));

            mainLoop.AddState(mainMenuState);
            mainLoop.AddState(gameState);
            mainLoop.AddState(pauseState);

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
            CommandManager.Instance.Update();
            CollisionManager.Instance.Update();

            mainLoop.Update(gameTime);

            base.Update(gameTime);
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
