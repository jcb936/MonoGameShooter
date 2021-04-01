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

        // laser
        private List<Laser> laserBeams;

        private Texture2D laserTexture;

        //Our Laser Sound and Instance
        private SoundEffect laserSound;
        private SoundEffectInstance laserSoundInstance;

        // govern how fast our laser can fire.
        private TimeSpan laserSpawnTime;
        private TimeSpan previousLaserSpawnTime;

        private bool isShooting;

        public event Action PlayerDestroyed;

        public void Initialize(Animation animation, Vector2 position, Viewport viewport, Texture2D laserTexture, SoundEffect laserSound)
        {
            deltaPosition = new Vector2();

            this.viewport = viewport;

            this.laserTexture = laserTexture;

            this.laserSound = laserSound;

            FlaggedForRemoval = false;

            BoundingRadius = animation.FrameWidth / 2f;

            laserSoundInstance = laserSound.CreateInstance();

            PlayerAnimation = animation;

            PlayerAnimation.Rotation = MathHelper.ToRadians(-90f);

            // Set the starting position of the player around the middle of the screen and to the back
            this.position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            GameInfo.Health = 100;

            // Set speed
            Speed = 500;

            // init our laser
            laserBeams = new List<Laser>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 800f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            deltaPosition *= (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += deltaPosition;

            // Make sure that the player does not go out of bounds
            position.X = MathHelper.Clamp(Position.X, Width / 2, viewport.Width - Width / 2);
            position.Y = MathHelper.Clamp(Position.Y, Height / 2, viewport.Height - Height / 2);

            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);

            if (isShooting)
                FireLaser(gameTime);

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

                // Play the laser sound!
                laserSoundInstance.Play();
            }
        }

        protected void AddLaser()
        {
            Animation laserAnimation = new Animation();
            // initlize the laser animation
            laserAnimation.Initialize(laserTexture,
                Position,
                46,
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
            laserPostion.Y -= 60;
            laserPostion.X -= 25;

            // init the laser
            laser.Initialize(laserAnimation, laserPostion);
            laserBeams.Add(laser);

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
            // do stuff
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
