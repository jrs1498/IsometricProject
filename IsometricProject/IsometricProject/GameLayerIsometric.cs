using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class GameLayerIsometric : GameLayer
    {
        #region Constructor Code
        /// <summary>
        /// A GameLayer which can hold isometric content
        /// </summary>
        public GameLayerIsometric(GameLevel gameLevel, float parallaxAmount)
            : base(gameLevel, parallaxAmount)
        {
        }
        #endregion

        #region DrawCode
        /// <summary>
        /// Draw any applicable GameObjects to the isometric coordinates specified by SpriteBatchIsometric
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Begin drawing from this GameLayer's perspective
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camTransformation);

            // Draw any applicable GameObjects in this layer
            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.DrawIsometric(gameTime, spriteBatch);
            }

            // End drawing
            spriteBatch.End();
        }
        #endregion
    }
}
