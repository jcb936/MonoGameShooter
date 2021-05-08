using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    class Explosion : IEntity
    {
        public static Texture2D ExplosionTexture { get; set; }

        Animation explosionAnimation;
        Vector2 Position;
        public bool Active { get; set; }
        int timeToLive;

        public int Width
        {
            get { return explosionAnimation.FrameWidth; }
        }
        public int Height
        {
            get { return explosionAnimation.FrameWidth; }
        }

        // Factory method
        public static Explosion Create(Vector2 position)
        {
            Animation explosionAnimation = new Animation();

            explosionAnimation.Initialize(ExplosionTexture,
                position,
                64,
                64,
                6,
                30,
                Color.White,
                1.0f,
                true);

            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, position);
            EntityManager.Instance.AddEntity(explosion);

            return explosion;
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            explosionAnimation = animation;
            Position = position;
            Active = true;
            timeToLive = 30;
        }

        public void Update(GameTime gameTime)
        {
            explosionAnimation.Update(gameTime);

            timeToLive -= 1;

            if (timeToLive <= 0)
            {
                this.Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            explosionAnimation.Draw(spriteBatch);
        }
    }
}
