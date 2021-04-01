using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class GameState : State
    {
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

        public GameState()
        {

        }

        public void Initialize(Player player, HUD hud, ParallaxingBackground layer1, ParallaxingBackground layer2, Texture2D enemyTexture, Texture2D mainBackground, Viewport viewport)
        {
            Name = "GameState";

            random = new Random();

            this.player = player;
            this.hud = hud;
            this.viewport = viewport;
            this.mainBackground = mainBackground;

            bgLayer1 = layer1;
            bgLayer2 = layer2;
            this.enemyTexture = enemyTexture;

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
        }

        public override void Enter(object owner)
        {
            GameInfo.CurrentScore = 0;
            player.SetBindings();
            CommandManager.Instance.AddKeyboardBinding(Keys.Escape, (_,_) => GameInfo.ExitGame());
            EntityManager.Instance.AddEntity(player);
            CollisionManager.Instance.AddCollidable(player);

            player.PlayerDestroyed += GameOverSequence;
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            // Update the parallaxing background
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);
        }

        public override void Exit(object owner)
        {
            player.RemoveBindings();
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Escape);

            player.PlayerDestroyed -= GameOverSequence;
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
            GameOver = true;
        }
    }
}
