using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class GameState : State
    {
        private ContentManager manager;

        private Player player;

        private HUD hud;

        private ParallaxingBackground bgLayer1;

        private ParallaxingBackground bgLayer2;

        private Texture2D enemyTexture;

        private Texture2D mainBackground;

        private Viewport viewport;

        private Random random;

        //The rate at which the enemies appear
        private TimeSpan enemySpawnTime;
        private TimeSpan previousSpawnTime;

        public bool GameOver { get; private set; }

        public bool Pausing { get; private set; }

        public GameState()
        {

        }

        public void Initialize(Viewport viewport)
        {
            Name = "GameState";

            manager = new ContentManager(GameInfo.GameInstance.Services, GameInfo.GameInstance.Content.RootDirectory);

            random = new Random();

            this.player = new();
            this.hud = new();
            this.viewport = viewport;

            bgLayer1 = new();
            bgLayer2 = new();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
        }

        public void LoadContent()
        {
            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerAnimTex = manager.Load<Texture2D>("Graphics\\engine-Sheet");

            Texture2D playerTex = manager.Load<Texture2D>("Graphics\\ship");

            playerAnimation.Initialize(playerAnimTex, Vector2.Zero, 32, 32, 4, 30, Color.White, 1f, true);
            // Load the parallaxing background
            bgLayer1.Initialize(manager, "Graphics/big_stars", viewport.Width, viewport.Height, 2);
            bgLayer2.Initialize(manager, "Graphics/small_stars", viewport.Width, viewport.Height, 4);
            mainBackground = manager.Load<Texture2D>("Graphics/black_background");
            enemyTexture = manager.Load<Texture2D>("Graphics/mineAnimation");

            // load the texture to serve as the laser
            var bulletTexture = manager.Load<Texture2D>("Graphics\\bullet");

            // Load the laserSound Effect and create the effect Instance
            var laserSound = manager.Load<SoundEffect>("Sound\\laserFire");

            Vector2 playerPosition = new Vector2(viewport.TitleSafeArea.X, viewport.TitleSafeArea.Y + viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerTex, playerPosition, viewport, bulletTexture, laserSound);

            // load the explosion sheet
            var explosionTexture = manager.Load<Texture2D>("Graphics\\explosion-Sheet");
            Explosion.ExplosionTexture = explosionTexture;

            // Load the score font
            var font = manager.Load<SpriteFont>("Graphics\\gameFont");

            // initialize HUD
            var hudTexture = manager.Load<Texture2D>("Graphics\\t_blackSquare");
            hud.Initialize(viewport, hudTexture, font);
        }

        public void UnloadContent()
        {
            manager.Unload();
        }

        public override void Enter(object owner, State prevState)
        {
            Pausing = false;

            if (prevState?.GetType() == typeof(MenuState))
            {
                LoadContent();
                GameInfo.CurrentScore = 0;
                EntityManager.Instance.AddEntity(player);
                CollisionManager.Instance.AddCollidable(player);

                player.PlayerDestroyed += GameOverSequence;
            }

            player.SetBindings();
            CommandManager.Instance.AddKeyboardBinding(Keys.Escape, (bState, _) => Pausing = bState == eButtonState.PRESSED);
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);
            EntityManager.Instance.Update(gameTime);
        }

        public override void Exit(object owner, State nextState)
        {
            player.RemoveBindings();
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Escape);

            // do cleanup
            if (nextState?.GetType() == typeof(MenuState))
            {
                player.PlayerDestroyed -= GameOverSequence;
                UnloadContent();
            }
        }

        public override void Draw(object owner, SpriteBatch spriteBatch)
        {
            //Draw the Main Background Texture
            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the moving background
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            EntityManager.Instance.DrawEntities(spriteBatch);

            hud.Draw(spriteBatch);
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(viewport.Width + enemyTexture.Width / 2, random.Next(100, viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            EntityManager.Instance.AddEntity(enemy);
            CollisionManager.Instance.AddCollidable(enemy);
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
        }

        private void GameOverSequence()
        {
            GameInfo.CheckIfHighscore();
            GameOver = true;
        }
    }
}
