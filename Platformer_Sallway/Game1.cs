using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

//GitHub Yay!

namespace Platformer_Sallway
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Song gameMusic;

        public static int tile = 70;
        // abitrary choice for 1m (1 tile = 1 meter)
        public static float meter = tile;
        // very exaggerated gravity (6x)
        public static float gravity = meter * 9.8f * 6.0f;
        // max vertical speed (10 tiles/sec horizontal, 15 tiles/sec vertical)
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15);
        // horizontal acceleration -  take 1/2 second to reach max velocity
        public static float acceleration = maxVelocity.X * 2;
        // horizontal friction     -  take 1/6 second to stop from max velocity
        public static float friction = maxVelocity.X * 6;
        // (a large) instantaneous jump impulse
        public static float jumpImpulse = meter * 1500;

        List<Enemy> enemies = new List<Enemy>();
        Sprite crystal = null;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;

        SpriteFont arialFont;
        int score = 0;

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

       /* //constant values for states
        const int State_Splash = 0;
        const int State_Menu = 1;
        const int State_Playing = 2;
        const int State_GameOver = 3;

        int gameState = State_Splash; */

        enum GameState
        {
            Splash_State,
            Menu_State,
            Playing_State,
            GameOver_State
        }
        GameState GetGameState = GameState.Splash_State; 

        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player(this);
            player.Position = new Vector2(300, 0); 
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

           // TODO: use this.Content to load your game content here
           // AIE.StateManager.CreateState("SPLASH", new SplashState());
           // AIE.StateManager.CreateState("GAME", new GameState());
           // AIE.StateManager.CreateState("GAMEOVER", new GameOverState());

          //  AIE.StateManager.PushState("SPLASH");


            player.Load(Content);

            arialFont = Content.Load<SpriteFont>("Arial");

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice,
              ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, ScreenHeight);

            map = Content.Load<TiledMap>("firstLevel");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Playable")
                {
                    collisionLayer = layer;
                }
            }

            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "Enemies")
                {
                    foreach (TiledMapObject obj in layer.Objects)
                    {
                        Enemy enemy = new Enemy(this);
                        enemy.Load(Content);
                        enemy.Position = new Vector2(obj.Position.X, obj.Position.Y);
                        enemies.Add(enemy);
                    }
                }
                
                if (layer.Name == "Collectables")
                {
                    TiledMapObject obj = layer.Objects[0];

                    if (obj != null)
                    {
                        AnimatedTexture anim = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
                        anim.Load(Content, "chest_closed", 1, 1);

                        crystal = new Sprite();
                        crystal.Add(anim, 0, 5);
                        crystal.position = new Vector2(obj.Position.X, obj.Position.Y);
                    }
                }
            }

            //Loading game music
            gameMusic = Content.Load<Song>("SuperHero_original_no_Intro");
            MediaPlayer.Play(gameMusic);
            MediaPlayer.Volume = 0.1f;
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /* switch (gameState)
             {
                 case gameState.DrawSplashState(spriteBatch);
                     break;
                 case gameState.DrawMenuState(spriteBatch);
                     break;
                 case gameState.DrawPlayingState(spriteBatch);
                     break;
                 case gameState.DrawGameOverState(spriteBatch);
                     break;
             } */

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            //AIE.StateManager.Update(Content, gameTime);

            // Add update logic here.
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(deltaTime);

            foreach (Enemy e in enemies)
            {
                e.Update(deltaTime);
            }

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            CheckCollisions();

            base.Update(gameTime);
        } 

        //Game state functions
        private void UpdateSplashState(float deltaTime)
        {

        }
        private void DrawSplashState(SpriteBatch spriteBatch)
        {

        }
        private void UpdateGameState(float deltaTime)
        {

        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //AIE.StateManager.Draw(spriteBatch);

            Matrix viewMatrix = camera.GetViewMatrix();
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0,
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix: viewMatrix);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);

            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch);
            }
            crystal.Draw(spriteBatch);

            // draw all the GUI components in a separte SpriteBatch section 
            spriteBatch.DrawString(arialFont, "Score : " + score.ToString(),
                new Vector2(20, 20), Color.Green);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }

        public int TileToPixel(int tileCoord)
        {
            return tile * tileCoord;
        }

        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
            {
                return 1;
            }
            
            // let the player drop of the bottom of the screen (this means death)
            if (pixelCoords.Y > map.HeightInPixels)
            {
                return 0;
            }
            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        }

        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
            {
                return 1;
            }
            
            // let the player drop of the bottom of the screen (this means death) 
            if (ty >= map.Height)
            {
                return 0;
            }

            TiledMapTile? tile;
            collisionLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;

        }

        private void CheckCollisions()
        {
            foreach (Enemy e in enemies)
            {
                if (IsColliding(player.Bounds, e.Bounds) == true)
                {
                    if (player.IsJumping && player.Velocity.Y > 0)
                    {
                        player.JumpOnCollision();
                        enemies.Remove(e);
                        break;
                    }
                    else
                    {
                        // player just died
                    }
                }
            }
        }

        private bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X ||
            rect1.X > rect2.X + rect2.Width ||
            rect1.Y + rect1.Height < rect2.Y ||
            rect1.Y > rect2.Y + rect2.Height)
            {
                // these two rectangles are not colliding
                return false;
            }
            // else, the two AABB rectangles overlap, therefore collision
            return true;
        }

    }
}
