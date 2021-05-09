using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    class Enemy : ICollidable, IEntity
    {
        // Animation representing the enemy
        public Animation EnemyAnimation;

        protected Texture2D laserTexture;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position => position;

        // The state of the Enemy Ship
        public bool Active { get; set; }

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player ship
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        #region ICollidable interface
        public float BoundingRadius { get; private set; }

        public bool FlaggedForRemoval { get; private set; }
        #endregion

        protected Vector2 position;

        // The speed at which the enemy moves
        protected float enemyMoveSpeed;

        // govern how fast our laser can fire.
        protected TimeSpan laserSpawnTime;
        protected TimeSpan previousLaserSpawnTime;

        private bool left;


        public virtual void Initialize(Animation animation, Vector2 position, Texture2D laserTex, bool left = false)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            this.position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;

            // Set the health of the enemy
            Health = 30;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set how fast the enemy moves
            enemyMoveSpeed = 150f;

            // Set the score value of the enemy
            Value = 100;

            BoundingRadius = animation.FrameWidth / 2f;

            // init our laser
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 150;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            laserTexture = laserTex;

            this.left = left;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            // The enemy always moves to the left so decrement it's x position
            if (left)
                position.X += enemyMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                position.X -= enemyMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            FireLaser(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if ((Position.X < 0 && !left) || (Position.X > 1080 && left) || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Explosion.Create(position);
                Active = false;
                FlaggedForRemoval = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

        protected virtual void FireLaser(GameTime gameTime)
        {
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                // Add the laer to our list.
                AddLaser();
                // Play the laser sound!
                //laserSoundInstance.Play();
            }
        }

        protected virtual void AddLaser()
        {
            Animation laserAnimation = new Animation();
            // initlize the laser animation
            laserAnimation.Initialize(laserTexture,
                Position,
                16,
                16,
                1,
                30,
                Color.White,
                1f,
                true);

            Laser laser = new Laser();
            // Get the starting postion of the laser.

            var laserPostion = Position;
            // Adjust the position slightly to match the muzzle of the cannon.
            laserPostion.Y += 30;
            //laserPostion.X += 30;

            // init the laser
            laser.Initialize(laserAnimation, laserPostion, 0f, eBulletDirection.DOWN, true, 0.5f);

            EntityManager.Instance.AddEntity(laser);
            CollisionManager.Instance.AddCollidable(laser);

            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        public virtual void OnCollision(ICollidable obj)
        {
            if (!Active)
                return;

            Type objType = obj.GetType();

            if ((objType == typeof(Laser) && !(obj as Laser).EnemyLaser))
            {
                Health -= (obj as Laser).Damage;
                if (Health <= 0)
                    GameInfo.CurrentScore += Value;
            }
            else if (objType == typeof(Player))
            {
                Health = 0;
                GameInfo.Health -= Damage;
            }

        }

    }
}
