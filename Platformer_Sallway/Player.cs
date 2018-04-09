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
        public Vector2 position = Vector2.Zero;

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
    }
}
