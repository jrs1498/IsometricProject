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
            set
            {
                _texture = value;
            }
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

        /// <summary>
        /// Draws the GameObject using this component's texture and color
        /// Draws to SpriteBatchIsometric's isometric coordinates
        /// Includes GameObject's elevation
        /// </summary>
        public void DrawIsometricElevated(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Create a Vector3 using GameObjects X and Y,
            // use elevation amount as Z component
            Vector3 displacementWithElevation;
            displacementWithElevation.X = _parent.Displacement.X;
            displacementWithElevation.Y = _parent.Displacement.Y;
            displacementWithElevation.Z = _parent.Elevation;

            // Draw as usual
            spriteBatch.DrawIsometricElevated(
                _texture,
                displacementWithElevation,
                null,
                _drawColor,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0f);
        }
        #endregion
    }
}
