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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Represents the player 
        Player player;

        //HUD
        HUD hud;

        // Image used to display the static background
        Texture2D mainBackground;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        //The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        // texture to hold the laser.
        Texture2D laserTexture;
        List<Laser> laserBeams;
        // govern how fast our laser can fire.
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;

        // Collections of explosions
        List<Explosion> explosions;

        //Texture to hold explosion animation.
        Texture2D explosionTexture;

        //Our Laser Sound and Instance
        private SoundEffect laserSound;
        private SoundEffectInstance laserSoundInstance;

        //Our Explosion Sound.
        private SoundEffect explosionSound;
        private SoundEffectInstance explosionSoundInstance;

        // Game Music.
        private Song gameMusic;

        //Number that holds the player score
        int score;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferHeight = 1920;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.ApplyChanges();

            // Initialize the player class
            player = new Player();

            hud = new HUD();

            //Background
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            // init our laser
            laserBeams = new List<Laser>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 800f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            // init our collection of explosions.
            explosions = new List<Explosion>();

            //Set player's score to zero
            score = 0;

            //initialize basic binding to exit the game
            CommandManager.Instance.AddKeyboardBinding(Keys.Escape, (buttonState, _) => { if (buttonState == eButtonState.DOWN) Exit(); });

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

            // TODO: use this.Content to load your game content here

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            // load the texture to serve as the laser
            laserTexture = Content.Load<Texture2D>("Graphics\\laser");

            // Load the laserSound Effect and create the effect Instance
            laserSound = Content.Load<SoundEffect>("Sound\\laserFire");
            laserSoundInstance = laserSound.CreateInstance();

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition, GraphicsDevice.Viewport, laserTexture, laserSound);

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");

            enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");

            // load the explosion sheet
            explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");

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

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            laserSoundInstance.Dispose();
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

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            //// TODO: Add your update logic here
            //// Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // TODO: Add your update logic here

            //Update the player
            UpdatePlayer(gameTime);

            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);

            // Update the collisions
            UpdateCollision();

            // Update explosions
            UpdateExplosions(gameTime);

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    //Add to the player's score
                    score += enemies[i].Value;
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to
            // determine if two objects are overlapping
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle laserRectangle;

            // Only create the rectangle once for the player
            playerRectangle = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                if (playerRectangle.Intersects(enemyRectangle))
                {
                    // kill off the enemy
                    enemies[i].Health = 0;
                    // Show the explosion where the enemy was...
                    AddExplosion(enemies[i].Position);
                    // deal damge to the player
                    player.Health -= enemies[i].Damage;

                    // if the player has no health destroy it.
                    if (player.Health <= 0)
                    {
                        player.Active = false;

                    }
                }

                // Laserbeam vs Enemy Collision
                for (var l = 0; l < laserBeams.Count; l++)
                {
                    // create a rectangle for this laserbeam
                    laserRectangle = new Rectangle(
                        (int)laserBeams[l].Position.X,
                        (int)laserBeams[l].Position.Y,
                        laserBeams[l].Width,
                        laserBeams[l].Height);

                    // test the bounds of the laser and enemy
                    if (laserRectangle.Intersects(enemyRectangle))
                    {
                        // Show the explosion where the enemy was...
                        AddExplosion(enemies[i].Position);
                        // kill off the enemy
                        enemies[i].Health = 0;

                        // kill off the laserbeam
                        laserBeams[l].Active = false;
                    }
                }
            }
        }

        protected void AddExplosion(Vector2 enemyPosition)
        {
            Animation explosionAnimation = new Animation();

            explosionAnimation.Initialize(explosionTexture,
                enemyPosition,
                134,
                134,
                12,
                30,
                Color.White,
                1.0f,
                true);

            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, enemyPosition);

            explosions.Add(explosion);

            /* play the explosion sound. */
            explosionSound.Play();
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (var e = explosions.Count - 1; e >= 0; e--)
            {
                explosions[e].Update(gameTime);

                if (!explosions[e].Active)
                    explosions.Remove(explosions[e]);
            }
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

            //Draw the Main Background Texture
            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the moving background
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            hud.Draw(spriteBatch);

            // Draw the Player
            player.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // draw explosions
            foreach (var e in explosions)
            {
                e.Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
