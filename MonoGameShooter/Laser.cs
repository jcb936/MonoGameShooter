using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    enum eBulletDirection
    {
        UP = -1,
        DOWN = 1
    }

    class Laser : ICollidable, IEntity
    {
        // laser animation.
        public Animation LaserAnimation;

        // the speed the laser travels
        float laserMoveSpeed = 2f;

        // position of the laser
        public Vector2 Position => position;

        // The damage the laser deals.
        public int Damage = 10;

        // set the laser to active
        public bool Active { get; set; }

        //private float lifeTime;

        private float rotation;

        private eBulletDirection direction;

        // Laser beams range.
        int Range;

        // the width of the laser image.
        public int Width
        {
            get { return LaserAnimation.FrameWidth; }
        }

        // the height of the laser image.
        public int Height
        {
            get { return LaserAnimation.FrameHeight; }
        }

        // ICollidable interface
        public float BoundingRadius { get; private set; }

        public bool FlaggedForRemoval { get; private set; }

        public bool EnemyLaser { get; private set; }

        private Vector2 position;

        public void Initialize(Animation animation, Vector2 position, float rotation = 0f, eBulletDirection dir = eBulletDirection.UP, bool enemyLaser = false, float speed = 2f)
        {
            //lifeTime = 2f;
            LaserAnimation = animation;
            this.position = position;
            BoundingRadius = animation.FrameWidth / 2f;
            Active = true;
            this.rotation = rotation;
            direction = dir;
            EnemyLaser = enemyLaser;
            laserMoveSpeed = speed;
            //LaserAnimation.Rotation = MathHelper.ToRadians(90f);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            position.Y += laserMoveSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds * (int)direction;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);

            //lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (position.Y < 0f || position.Y > 1920)
            {
                Active = false;
                FlaggedForRemoval = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }

        #region ICollidable interface
        public void OnCollision(ICollidable obj)
        {
            if (!Active)
                return;

            // we don't want to hit ourselves
            if (obj.GetType() == typeof(Player) && EnemyLaser)
            {
                // add explosion
                FlaggedForRemoval = true;
                Active = false;
                GameInfo.Health -= Damage;
            }

            if (obj.GetType() == typeof(Enemy) && !EnemyLaser)
            {
                if ((obj as Enemy).Active)
                 {
                    // add explosion
                    FlaggedForRemoval = true;
                    Active = false;
                }
            }
        }
        #endregion
    }
}
