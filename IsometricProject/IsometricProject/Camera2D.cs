using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class Camera2D : GameObject
    {
        #region Attributes
        private GraphicsDevice _graphicsDevice;

        private Boolean _requiresCamTransform;

        private float _zoom;
        private const float _maxZoom = 2.0f;
        private const float _minZoom = 0.01f;
        #endregion

        #region Properties
        public Boolean RequiresCamTransform
        {
            get { return _requiresCamTransform; }
        }
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom > _maxZoom)
                    _zoom = _maxZoom;
                else if (_zoom < _minZoom)
                    _zoom = _minZoom;

                OnPositionChange();
            }
        }
        #endregion

        #region Events
        protected override void OnPositionChange()
        {
            // All layers will need to recalculate their transformation
            _requiresCamTransform = true;
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Create a Camera2D, which is used to view the GameLevel
        /// </summary>
        public Camera2D(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _zoom = 1.0f;

            // Components
            AddComponent<GOCMovable>();
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this Camera2D
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Set to false so any modifying updates can switch it to true
            _requiresCamTransform = false;

            // Base will update all components
            base.Update(gameTime);
        }
        #endregion

        #region Camera Functionality
        /// <summary>
        /// Recalculate the Camera's transformation matrix
        /// </summary>
        public Matrix GetCamTransformation(float parallaxAmount)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Displacement.X * parallaxAmount, -Displacement.Y * parallaxAmount, 0))
                * Matrix.CreateScale(_zoom)
                * Matrix.CreateTranslation(new Vector3(_graphicsDevice.Viewport.Width / 2.0f, _graphicsDevice.Viewport.Height / 2.0f, 0));
        }
        #endregion
    }
}
