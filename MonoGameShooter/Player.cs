using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace THClone
{
    class Player : ICollidable, IEntity
    {
        // Animation representing the player
        //public Texture2D PlayerTexture;
        public Animation PlayerAnimation;

        private Texture2D playerSprite;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position => position;

        // State of the player
        public bool Active { get; set; }

        // Amount of hit points that player has
        public int Health;

        // Speed of the player
        public int Speed { get; private set; }

        public int Width
        { 
            get { return PlayerAnimation.FrameWidth; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        // ICollidable interface
        public float BoundingRadius { get; private set; }

        public bool FlaggedForRemoval { get; private set; }

        private Vector2 position;

        private Vector2 deltaPosition;

        private Viewport viewport;

        private Texture2D laserTexture;

        //Our Laser Sound and Instance
        private SoundEffect laserSound;
        private SoundEffectInstance laserSoundInstance;

        // govern how fast our laser can fire.
        private TimeSpan laserSpawnTime;
        private TimeSpan previousLaserSpawnTime;

        private bool isShooting;

        private bool powerupActive;

        private float powerupTimer;

        public event Action PlayerDestroyed;

        public void Initialize(Animation animation, Texture2D tex, Vector2 position, Viewport viewport, Texture2D laserTexture, SoundEffect laserSound)
        {
            deltaPosition = new Vector2();

            this.viewport = viewport;

            this.laserTexture = laserTexture;

            this.laserSound = laserSound;

            FlaggedForRemoval = false;

            laserSoundInstance = laserSound.CreateInstance();

            PlayerAnimation = animation;

            playerSprite = tex;

            powerupActive = false;

            powerupTimer = -1f;

            //PlayerAnimation.Rotation = MathHelper.ToRadians(-90f);

            // Set the starting position of the player around the middle of the screen and to the back
            this.position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            GameInfo.Health = 100;

            // Set speed
            Speed = 500;

            // init our laser
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 800f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            BoundingRadius = playerSprite.Width / 2f;

        }

        public void Update(GameTime gameTime)
        {
            deltaPosition *= (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += deltaPosition;

            // Make sure that the player does not go out of bounds
            position.X = MathHelper.Clamp(position.X, playerSprite.Width / 2, viewport.Width - playerSprite.Width / 2);
            position.Y = MathHelper.Clamp(position.Y, playerSprite.Height / 2, viewport.Height - playerSprite.Height / 2);

            PlayerAnimation.Position = new Vector2(position.X, position.Y + 50f);
            PlayerAnimation.Update(gameTime);

            if (isShooting)
                FireLaser(gameTime);

            if (powerupTimer > 0f)
            {
                powerupTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (powerupTimer < 0f)
                    powerupActive = false;
            }

            // reset score if player health goes to zero
            if (GameInfo.Health <= 0)
            {
                Active = false;
                PlayerDestroyed?.Invoke();
                FlaggedForRemoval = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw ship
            var destinationRect = new Rectangle((int)Position.X - (int)(playerSprite.Width / 2f), (int)Position.Y - (int)(playerSprite.Height / 2f), (int)(playerSprite.Width), (int)(playerSprite.Height));
            var pos = new Vector2(Position.X - playerSprite.Width / 2, Position.Y - playerSprite.Height / 2);
            //var sourceRect = new Rectangle(0, 0, playerSprite.Width, playerSprite.Height);
            //spriteBatch.Draw(playerSprite, destinationRect, Color.White);
            spriteBatch.Draw(playerSprite, pos, null, Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 0f);

            // draw engine animation
            PlayerAnimation.Draw(spriteBatch);
        }

        protected void FireLaser(GameTime gameTime)
        {
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                // Add the laer to our list.
                AddLaser();
                if (powerupActive)
                    PowerupLaser();
                // Play the laser sound!
                laserSoundInstance.Play();
            }
        }

        private void PowerupLaser()
        {
            Animation laserAnimationLeft = new Animation();
            Animation laserAnimationRight = new Animation();

            // initlize the laser animation
            laserAnimationLeft.Initialize(laserTexture,
                Position,
                32,
                43,
                1,
                30,
                Color.White,
                1f,
                true);

            // initlize the laser animation
            laserAnimationRight.Initialize(laserTexture,
                Position,
                32,
                43,
                1,
                30,
                Color.White,
                1f,
                true);

            Laser laserLeft = new Laser();
            Laser laserRight = new Laser();

            // Get the starting postion of the laser.

            var laserPositionLeft = Position;
            var laserPositionRight = Position;

            // Adjust the position slightly to match the muzzle of the cannon.
            laserPositionLeft.Y += 20;
            laserPositionLeft.X -= 40;

            laserPositionRight.Y += 20;
            laserPositionRight.X += 40;

            // init the laser
            laserLeft.Initialize(laserAnimationLeft, laserPositionLeft, -45f);
            laserRight.Initialize(laserAnimationRight, laserPositionRight, 45f);

            EntityManager.Instance.AddEntity(laserLeft);
            EntityManager.Instance.AddEntity(laserRight);
            CollisionManager.Instance.AddCollidable(laserRight);
            CollisionManager.Instance.AddCollidable(laserLeft);

            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        protected void AddLaser()
        {
            Animation laserAnimation = new Animation();
            // initlize the laser animation
            laserAnimation.Initialize(laserTexture,
                Position,
                32,
                43,
                1,
                30,
                Color.White,
                1f,
                true);

            Laser laser = new Laser();
            // Get the starting postion of the laser.

            var laserPostion = Position;
            // Adjust the position slightly to match the muzzle of the cannon.
            laserPostion.Y -= 60;
            //laserPostion.X += 30;

            // init the laser
            laser.Initialize(laserAnimation, laserPostion);

            EntityManager.Instance.AddEntity(laser);
            CollisionManager.Instance.AddCollidable(laser);

            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        public void SetBindings()
        {
            CommandManager.Instance.AddKeyboardBinding(Keys.W, MoveUp);
            CommandManager.Instance.AddKeyboardBinding(Keys.A, MoveLeft);
            CommandManager.Instance.AddKeyboardBinding(Keys.S, MoveDown);
            CommandManager.Instance.AddKeyboardBinding(Keys.D, MoveRight);
            CommandManager.Instance.AddKeyboardBinding(Keys.Space, Shoot);
        }

        public void RemoveBindings()
        {
            CommandManager.Instance.RemoveKeyboardBinding(Keys.W);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.A);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.S);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.D);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Space);
        }

        #region ICollidable interface
        public void OnCollision(ICollidable obj)
        {
            if (obj.GetType() == typeof(Powerup))
            {
                powerupTimer = 5f;
                powerupActive = true;
            }
        }
        #endregion

        #region Callbacks
        private void MoveLeft(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
                deltaPosition.X -= Speed;
            else if (buttonState == eButtonState.UP)
                deltaPosition.X = 0;
        }
        private void MoveRight(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
                deltaPosition.X += Speed;
            else if (buttonState == eButtonState.UP)
                deltaPosition.X = 0;
        }

        private void MoveUp(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
                deltaPosition.Y -= Speed;
            else if (buttonState == eButtonState.UP)
                deltaPosition.Y = 0;
        }

        private void MoveDown(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
                deltaPosition.Y += Speed;
            else if (buttonState == eButtonState.UP)
                deltaPosition.Y = 0;
        }

        private void Shoot(eButtonState buttonState, Vector2 amount)
        {
            isShooting = buttonState == eButtonState.DOWN;
        }

        #endregion

    }
}
