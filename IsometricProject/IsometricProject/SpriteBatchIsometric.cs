using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class SpriteBatchIsometric : SpriteBatch
    {
        #region Attributes
        private Matrix _isometricTransformation;
        private Matrix _isometricTransformationInverse;

        private const float pi = (float)Math.PI;
        private float _rotationY;
        private float _rotationDown;
        private float _rotationDownScale;
        #endregion

        #region Properties
        public float RotationY
        {
            get { return _rotationY; }
            set
            {
                _rotationY = value;

                // TODO: Add code to preserve 0 <= theta <= 2 * pi

                CreateTransformation();
            }
        }
        public float RotationDown
        {
            get { return _rotationDown; }
            set
            {
                _rotationDown = value;

                if (_rotationDown < 0)
                    _rotationDown = 0;
                else if (_rotationDown > pi * 0.5f)
                    _rotationDown = pi * 0.5f;

                CreateTransformation();
            }
        }
        #endregion

        /// <summary>
        /// Creates a SpriteBatch capable of drawing content with an isometric perspective
        /// </summary>
        public SpriteBatchIsometric(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _rotationY      = pi / 4;   // 45 degrees
            _rotationDown   = pi / 6;   // 30 degrees

            CreateTransformation();     // Calculate matrix
        }

        /// <summary>
        /// Calculate the isometric transformation matrix
        /// </summary>
        private void CreateTransformation()
        {
            _rotationDownScale = 1.0f - (float)Math.Sin(_rotationDown);

            _isometricTransformation =
                Matrix.CreateRotationZ(_rotationY)
                * Matrix.CreateScale(1.0f, _rotationDownScale, 1.0f);

            _isometricTransformationInverse = Matrix.Invert(_isometricTransformation);
        }

        public Vector2 IsometricToCartesian(Vector2 isometricCoordinates)
        {
            Vector2 cartesianCoordinates = Vector2.Transform(isometricCoordinates, _isometricTransformationInverse);

            return cartesianCoordinates;
        }
        private Vector2 CartesianToIsometric(Vector3 cartesianCoordinates)
        {
            Vector2 isometricCoordinates;
            isometricCoordinates.X = cartesianCoordinates.X;
            isometricCoordinates.Y = cartesianCoordinates.Z;
            isometricCoordinates = Vector2.Transform(isometricCoordinates, _isometricTransformation);
            isometricCoordinates.Y -= (cartesianCoordinates.Y * _rotationDownScale);

            return isometricCoordinates;
        }

        public void DrawIsometric(Texture2D texture, Vector3 position, Color color)
        {
            base.Draw(texture, CartesianToIsometric(position), color);
        }
        public void DrawIsometric(Texture2D texture, Vector3 position, Rectangle? sourceRectangle, Color color)
        {
            base.Draw(texture, CartesianToIsometric(position), sourceRectangle, color);
        }
        public void DrawIsometric(Texture2D texture, Vector3 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            base.Draw(texture, CartesianToIsometric(position), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
        public void DrawIsometric(Texture2D texture, Vector3 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            base.Draw(texture, CartesianToIsometric(position), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
