using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class Powerup : IEntity, ICollidable
    {
        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position => position;

        // The state of the Enemy Ship
        public bool Active { get; set; }

        #region ICollidable interface
        public float BoundingRadius { get; private set; }

        public bool FlaggedForRemoval { get; private set; }

        private Animation powerupAnim;

        private Vector2 position;

        private float speed;
        #endregion

        public void Initialise(Animation anim, Vector2 pos)
        {
            position = pos;
            Active = true;
            powerupAnim = anim;
            BoundingRadius = powerupAnim.FrameWidth / 2f;
            speed = 200f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            powerupAnim.Draw(spriteBatch);
        }

        public void OnCollision(ICollidable obj)
        {
            if (obj.GetType() == typeof(Player))
                Active = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            powerupAnim.Position = position;

            powerupAnim.Update(gameTime);

            if (position.Y > 1920)
                Active = false;
        }
    }
}
