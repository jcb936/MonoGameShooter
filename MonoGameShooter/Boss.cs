using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class Boss : Enemy
    {
        private float rotation;

        private Vector2 shootLocation;

        public override void Initialize(Animation animation, Vector2 position, Texture2D laserTex, bool left = false)
        {
            base.Initialize(animation, position, laserTex, left);
            Health = 10000;
            Damage = 20;
            Value = 1000;
            enemyMoveSpeed = 2f;
            rotation = 0f;
            // init our laser
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 300;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            // The enemy always moves to the left so decrement it's x position
            position.Y += enemyMoveSpeed;

            position.Y = Math.Clamp(position.Y, -30f, 200f);

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            shootLocation = new Vector2(Position.X + (EnemyAnimation.FrameWidth / 2) *(float)Math.Cos(rotation), Position.Y + (EnemyAnimation.FrameWidth / 2) * (float)Math.Sin(rotation));

            // Update Animation
            EnemyAnimation.Update(gameTime);

            FireLaser(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Explosion.Create(position);
                Active = false;
                //FlaggedForRemoval = true;
            }
        }

        protected override void AddLaser()
        {
            Animation laserAnimation = new Animation();
            // initlize the laser animation
            laserAnimation.Initialize(laserTexture,
                shootLocation,
                16,
                16,
                1,
                30,
                Color.White,
                1f,
                true);

            Laser laser = new Laser();
            // Get the starting postion of the laser.

            var laserPostion = shootLocation;
            // Adjust the position slightly to match the muzzle of the cannon.
            //laserPostion.Y += 30;
            //laserPostion.X += 30;

            // init the laser
            laser.Initialize(laserAnimation, laserPostion, 0f, eBulletDirection.DOWN, true, 0.5f);

            EntityManager.Instance.AddEntity(laser);
            CollisionManager.Instance.AddCollidable(laser);

            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        protected override void FireLaser(GameTime gameTime)
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
    }
}
