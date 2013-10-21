using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace IsometricProject
{
    public class AbstractScreen
    {
        #region Attributes
        private ScreenHandler _screenHandler;
        #endregion

        #region Properties
        public ContentManager Content
        {
            get { return _screenHandler.Content; }
        }
        public GraphicsDevice GraphicsDevice
        {
            get { return _screenHandler.GraphicsDevice; }
        }
        public SpriteBatchIsometric SpriteBatch
        {
            get { return _screenHandler.SpriteBatch; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// General Screen class constructor. Derive-only
        /// </summary>
        public AbstractScreen(ScreenHandler screenHandler)
        {
            _screenHandler = screenHandler;
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update everything on this screen
        /// </summary>
        public virtual void Update(GameTime gameTime)
        { 
            // TODO: Add general screen update code here
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw everything on this screen
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        { 
            // TODO: Add general screen draw code here
        }
        #endregion
    }
}
