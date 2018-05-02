using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer_Sallway
{
    class Player
    {
        Sprite sprite = new Sprite();

        Game1 game = null;
        bool isFalling = true;
        bool isJumping = false;

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;

        //Jump Instance and Sound
        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;

        public Vector2 Position
        {
            get { return sprite.position; }
            set { sprite.position = value; }
        }

        public Player(Game1 game)
        {
            this.game = game;
            isFalling = true;
            isJumping = false;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
        }

        bool hFlipped = false;


        public void Load(ContentManager content)
        {
            //sprite.Load(content, "hero");

            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "walk", 12, 20);

            jumpSound = content.Load<SoundEffect>("Jump");
            jumpSoundInstance = jumpSound.CreateInstance();
            jumpSoundInstance.Volume = 0.1f;

            sprite.Add(animation, 0, -5);
            sprite.Pause();
        }

        public void Update(float deltaTime)
        {
            UpdateInput(deltaTime);
            sprite.Update(deltaTime);

            /* KeyboardState state = Keyboard.GetState();
             int speed = 50;

             if (state.IsKeyDown(Keys.Up) == true)
             {
                 position.Y -= speed * deltaTime;
             }
             if (state.IsKeyDown(Keys.Down) == true)
             {
                 position.Y += speed * deltaTime;
             }
             if (state.IsKeyDown(Keys.Left) == true)
             {
                 position.X -= speed * deltaTime;
                 hFlipped = true;
             }
             if (state.IsKeyDown(Keys.Right) == true)
             {
                 position.X += speed * deltaTime;
                 hFlipped = false;
             }*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);

            /*if (hFlipped == true)
                sprite.Draw(spriteBatch, position, SpriteEffects.FlipHorizontally);
            else
                sprite.Draw(spriteBatch, position); */
        }

        private void UpdateInput(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, Game1.gravity);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                acceleration.X -= Game1.acceleration;
                sprite.SetFlipped(true);
                sprite.Play();
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.friction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                acceleration.X += Game1.acceleration;
                sprite.SetFlipped(false);
                sprite.Play();
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.friction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true && this.isJumping == false && falling == false)
            {
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
                jumpSoundInstance.Play();
            }

            // integrate the forces to calculate the new position and velocity
            velocity += acceleration * deltaTime;

            // clamp the velocity so the player doesn't go too fast
            velocity.X = MathHelper.Clamp(velocity.X,
                -Game1.maxVelocity.X, Game1.maxVelocity.X);

            velocity.Y = MathHelper.Clamp(velocity.Y,
                -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            sprite.position += velocity * deltaTime;

            // One tricky aspect of using a frictional force to slow the player down
            // (as opposed to just allowing a dead-stop) is that the force is highly
            // unlikely to be exactly the force needed to come to a halt. In fact, it’s
            // likely to overshoot in the opposite direction and lead to a tiny jiggling
            // effect instead of actually stopping the player. 

            // In order to avoid this, we must clamp the horizontal velocity to zero if
            // we detect that the players direction has just changed:
            if ((wasMovingLeft && (velocity.X > 0)) ||
                (wasMovingRight && (velocity.X < 0)))
            {
                // clamp at zero to prevent friction from making us jiggle side to side
                velocity.X = 0;
                sprite.Pause();
            }

            // collision detection
            // Our collision detection logic is greatly simplified by the fact that
            // the player is a rectangle and is exactly the same size as a single tile.
            // So we know that the player can only ever occupy 1, 2 or 4 cells.
            // This means we can short-circuit and avoid building a general purpose
            // collision detection engine by simply looking at the 1 to 4 cells that
            // the player occupies:
            int tx = game.PixelToTile(position.X);
            int ty = game.PixelToTile(position.Y);
            // nx = true if player overlaps right
            bool nx = (position.X) % Game1.tile != 0;
            // ny = true if player overlaps below
            bool ny = (position.Y) % Game1.tile != 0;
            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            // If the player has vertical velocity, then check to see if they have hit
            // a platform below or above, in which case, stop their vertical velocity,
            // and clamp their y position:
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    // clamp the y position to avoid falling into platform below                 
                    position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;        // stop downward velocity
                    this.isFalling = false;     // no longer falling
                    this.isJumping = false;     // (or jumping)
                    ny = false;                 // - no longer overlaps the cells below
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    // clamp the y position to avoid jumping into platform above
                    position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;
                    sprite.Pause();
                    // stop upward velocity
                    // player is no longer really in that cell, we clamped them
                    // to the cell below
                    cell = celldown;
                    cellright = celldiag;  // (ditto)
                    ny = false;            // player no longer overlaps the cells below
                }
            }

            // Once the vertical velocity is taken care of, we can apply similar
            // logic to the horizontal velocity:
            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    // clamp the x position to avoid moving into the platform
                    // we just hit
                    sprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;      // stop horizontal velocity
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    // clamp the x position to avoid moving into the platform
                    // we just hit
                    sprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;      // stop horizontal velocity
                    sprite.Pause();
                }
            }

            // The last calculation for our update() method is to detect if the
            // player is now falling or not. We can do that by looking to see if
            // there is a platform below them
            this.isFalling = !(celldown || (nx && celldiag));

        }


    }
}