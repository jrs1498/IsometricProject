using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsometricProject
{
    public class GOCDrawable : GameObjectComponent
    {
        #region Attributes
        protected Texture2D _texture;       // Texture which will draw to parents displacement vector
        protected Color _drawColor;         // Color to use while drawing this object
        #endregion

        #region Properties
        public Texture2D Texture
        {
            get { return _texture; }
        }
        public Color DrawColor
        {
            get { return _drawColor; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// This component allows a GameObject to be drawn on a GameLayer
        /// </summary>
        public GOCDrawable(GameObject parent, Texture2D texture)
            : base(parent)
        {
            _texture = texture;
            _drawColor = Color.White;
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draws the GameObject using this components texture and color
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                _parent.Displacement,
                _drawColor);
        }

        /// <summary>
        /// Draws the GameObject using this components texture and color
        /// Draws to SpriteBatchIsometric's isometric coordinates
        /// </summary>
        public void DrawIsometric(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            spriteBatch.DrawIsometric(
                _texture,
                _parent.Displacement,
                _drawColor);
        }
        #endregion
    }
}
