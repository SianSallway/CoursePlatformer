using Microsoft.Xna.Framework;
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

        public Vector2 Position
        {
            get { return position; }
        }

        public Player(Game1 game)
        {
            this.game = game;
            isFalling = true;
            isJumping = true;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
        }
       
        bool hFlipped = false;


        public Player()
        {
        }

        public void Load(ContentManager content)
        {
            sprite.Load(content, "hero");
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);

            KeyboardState state = Keyboard.GetState();
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
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(hFlipped == true)
                sprite.Draw(spriteBatch, position, SpriteEffects.FlipHorizontally);
            else
                sprite.Draw(spriteBatch, position);
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
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.friction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                acceleration.X += Game1.acceleration;
            }
            else if (wasMovingRight == true) { acceleration.X -= Game1.friction; }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true && this.isJumping == false && falling == false)
            {
                acceleration.Y -= Game1.jumpImpulse; this.isJumping = true;
            }

            // integrate the forces to calculate the new position and velocity
            velocity += acceleration * deltaTime;

            // clamp the velocity so the player doesn't go too fast
            velocity.X = MathHelper.Clamp(velocity.X,  -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y,  -Game1.maxVelocity.Y, Game1.maxVelocity.Y); 

            position += velocity * deltaTime;
            
        }
    }
}
