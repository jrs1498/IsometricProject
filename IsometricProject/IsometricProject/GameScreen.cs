using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IsometricProject.Interface;

namespace IsometricProject
{
    public class GameScreen : AbstractScreen
    {
        #region Attributes
        private ContentLibrary  _contentLibrary;
        private GameLevel       _gameLevel;
        private GameInterface   _gameInterface;
        #endregion

        #region Properties
        public ContentLibrary ContentLib
        {
            get { return _contentLibrary; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Creates a GameScreen which handles gameplay
        /// </summary>
        public GameScreen(ScreenHandler screenHandler)
            : base(screenHandler)
        {
            // ---------- Create ContentLibrary ----------
            _contentLibrary = new ContentLibrary(Content);

            // ---------- Create GameLevel ----------
            _gameLevel = new GameLevel(this, "turdmonkey");
            //_gameLevel = new GameLevel(this, 20, 80, 3);

            // ---------- Create GameInterface ----------
            _gameInterface = new GameInterface(this);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update everything contained in the GameScreen
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // ---------- Update GameLevel ----------
            _gameLevel.Update(gameTime);

            // ---------- Update GameInterface ----------
            _gameInterface.Update(gameTime);

            // ---------- Update Base ----------
            base.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw everything contained in the GameScreen
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // ---------- Draw GameLevel ----------
            _gameLevel.Draw(gameTime, spriteBatch);

            // ---------- Draw GameInterface ----------
            _gameInterface.Draw(gameTime, spriteBatch);
            
            // ---------- Draw Base ----------
            base.Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
