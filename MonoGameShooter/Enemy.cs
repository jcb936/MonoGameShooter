﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    class Enemy : ICollidable, IEntity
    {
        // Animation representing the enemy
        public Animation EnemyAnimation;

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

        private Vector2 position;

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            this.position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;

            // Set the health of the enemy
            Health = 10;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set how fast the enemy moves
            enemyMoveSpeed = 6f;

            // Set the score value of the enemy
            Value = 100;

            BoundingRadius = animation.FrameWidth / 2f;
        }

        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement it's x position
            position.X -= enemyMoveSpeed;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (Position.X < 0 || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Active = false;
                FlaggedForRemoval = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

        public void OnCollision(ICollidable obj)
        {
            if (!Active)
                return;

            Type objType = obj.GetType();

            if (objType == typeof(Laser) || objType == typeof(Player))
            {
                Explosion.Create(position);
                Health = 0;
                GameInfo.CurrentScore += Value;

                if (objType == typeof(Player))
                {
                    Player playerRef = obj as Player;
                    GameInfo.Health -= Damage;
                }

            }

        }

    }
}
