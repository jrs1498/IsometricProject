using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DataTypes;

namespace IsometricProject
{
    public class GameLayer
    {
        #region Attributes
        protected GameLevel _gameLevel;
        protected List<GameObject> _gameObjects;

        protected float _parallaxAmount;
        protected Matrix _camTransformation;
        #endregion

        #region Constructor Code
        /// <summary>
        /// GameLayers operate independently within a GameLevel
        /// </summary>
        public GameLayer(GameLevel gameLevel, float parallaxAmount = 1.0f)
        {
            _gameLevel = gameLevel;
            _gameObjects = new List<GameObject>();

            _parallaxAmount = parallaxAmount;

            GetCamTransformation();
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update all data on this GameLayer
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Check if we need to update the camera transformation
            if (_gameLevel.Camera.RequiresCamTransform)
                GetCamTransformation();

            // Update any applicable GameObjects in this layer
            foreach (GameObject go in _gameObjects)
                go.Update(gameTime);
        }

        /// <summary>
        /// Recalculates the camera's transformation matrix
        /// </summary>
        private void GetCamTransformation()
        {
            _camTransformation = _gameLevel.Camera.GetCamTransformation(_parallaxAmount);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw all data on this GameLayer
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
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

            // ---------- Start drawing GameObjects ----------

            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.Draw(gameTime, spriteBatch);
            }

            // ---------- End drawing GameObjects ----------

            // End drawing
            spriteBatch.End();
        }
        #endregion

        #region Layer Generation Code
        /// <summary>
        /// Packages this GameLayer data and returns it
        /// </summary>
        public virtual GameLayerData PackageData()
        {
            return null;
        }
        #endregion

        #region Containment Code
        /// <summary>
        /// Use this function to add a GameObject to this layer
        /// </summary>
        public void AddObject(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
        }
        #endregion
    }
}
