using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    class Laser : ICollidable
    {
        // laser animation.
        public Animation LaserAnimation;

        // the speed the laser travels
        float laserMoveSpeed = 30f;

        // position of the laser
        public Vector2 Position => position;

        // The damage the laser deals.
        public int Damage = 10;

        // set the laser to active
        public bool Active;

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

        private Vector2 position;

        public void Initialize(Animation animation, Vector2 position)
        {
            LaserAnimation = animation;
            this.position = position;
            BoundingRadius = animation.FrameWidth / 2f;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            position.Y -= laserMoveSpeed;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }

        #region ICollidable interface
        public void OnCollision(ICollidable obj)
        {
            // we don't want to hit ourselves
            if (obj.GetType() == typeof(Player))
                return;

            if (obj.GetType() == typeof(Enemy))
            {
                // add explosion
                FlaggedForRemoval = true;
                Active = false;
            }
        }
        #endregion
    }
}
