using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        #region Properties
        public List<GameObject> GameObjects
        {
            get { return _gameObjects; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// GameLayers operate independently within a GameLevel
        /// </summary>
        public GameLayer(GameLevel gameLevel, float parallaxAmount)
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

            // Draw any applicable GameObjects in this layer
            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.Draw(gameTime, spriteBatch);
            }

            // End drawing
            spriteBatch.End();
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
