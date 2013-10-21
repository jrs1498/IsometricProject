using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace IsometricProject
{
    public class ScreenHandler : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        private GraphicsDeviceManager _graphicsMngr;
        private SpriteBatchIsometric _spriteBatch;

        private const int SCREEN_WIDTH = 1280;
        private const int SCREEN_HEIGHT = 720;

        private AbstractScreen _screen;
        #endregion

        #region Properties
        public SpriteBatchIsometric SpriteBatch
        {
            get { return _spriteBatch; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// ScreenHandler is the main type for this game.
        /// </summary>
        public ScreenHandler()
        {
            _graphicsMngr = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphicsMngr.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphicsMngr.PreferredBackBufferHeight = SCREEN_HEIGHT;

            //IsMouseVisible = true;
        }
        #endregion

        #region Initialization Code
        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Content Code
        /// <summary>
        /// Load specified content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatchIsometric(GraphicsDevice);

            TestingContent();
        }

        /// <summary>
        /// Unload specified content
        /// </summary>
        protected override void UnloadContent()
        {
        }

        private void TestingContent()
        {
            _screen = new GameScreen(this);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update all game information once per game cycle
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Get user input
            Controller.UpdateInput();

            // Update the current screen
            _screen.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw all game information once per game cycle
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // Draw the current screen
            _screen.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
        #endregion
    }
}
