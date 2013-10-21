using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class GameScreen : AbstractScreen
    {
        #region Attributes
        private GameLevel _gameLevel;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Creates a GameScreen which handles gameplay
        /// </summary>
        public GameScreen(ScreenHandler screenHandler)
            : base(screenHandler)
        {
            _gameLevel = new GameLevel(this, "turdmonkey");
            //_gameLevel = new GameLevel(this, 20, 80, 3);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update everything contained in the GameScreen
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Update the GameLevel
            _gameLevel.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw everything contained in the GameScreen
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Draw the GameLevel
            _gameLevel.Draw(gameTime, spriteBatch);
            
            base.Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
