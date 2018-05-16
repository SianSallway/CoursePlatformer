using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParticleEffects;

namespace Platformer_Sallway
{
    class Goal
    {
        Sprite sprite = new Sprite();
        // keep a reference to the Game object to check for collisions on the map
        Game1 game = null;

        Emitter portalEmitter = null;
        Texture2D portalTexture = null;


        public Vector2 Position
        {
            get { return sprite.position; }
            set { sprite.position = value; }
        }

        public Rectangle Bounds
        {
            get { return sprite.Bounds; }
        }

        public Goal(Game1 game)
        {
            this.game = game;

        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "princess", 1, 1);

            sprite.Add(animation, 1, 0);

            portalTexture = content.Load<Texture2D>("Aura");
            portalEmitter = new Emitter(portalTexture, sprite.position);
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);

            // update the flare particle emitter
           portalEmitter.position = sprite.position;
           portalEmitter.Update(deltaTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            portalEmitter.Draw(spriteBatch);
        }


    }
}
