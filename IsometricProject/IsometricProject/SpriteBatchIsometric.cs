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

        private const float pi = (float)Math.PI;
        private float _rotationY;
        private float _rotationDown;
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
        public void CreateTransformation()
        {
            _isometricTransformation =
                Matrix.CreateRotationZ(_rotationY)
                * Matrix.CreateScale(1.0f, 1.0f - (float)Math.Sin(_rotationDown), 1.0f);
        }

        public void DrawIsometric(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Vector2 position;
            position.X = destinationRectangle.X;
            position.Y = destinationRectangle.Y;

            position = Vector2.Transform(position, _isometricTransformation);
            destinationRectangle.X = (int)position.X;
            destinationRectangle.Y = (int)position.Y;

            base.Draw(texture, destinationRectangle, color);
        }
        public void DrawIsometric(Texture2D texture, Vector2 position, Color color)
        {
            base.Draw(texture, Vector2.Transform(position, _isometricTransformation), color);
        }
        public void DrawIsometric(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            Vector2 position;
            position.X = destinationRectangle.X;
            position.Y = destinationRectangle.Y;

            position = Vector2.Transform(position, _isometricTransformation);
            destinationRectangle.X = (int)position.X;
            destinationRectangle.Y = (int)position.Y;

            base.Draw(texture, destinationRectangle, sourceRectangle, color);
        }
        public void DrawIsometric(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            base.Draw(texture, Vector2.Transform(position, _isometricTransformation), sourceRectangle, color);
        }
        public void DrawIsometric(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        { 
            Vector2 position;
            position.X = destinationRectangle.X;
            position.Y = destinationRectangle.Y;

            position = Vector2.Transform(position, _isometricTransformation);
            destinationRectangle.X = (int)position.X;
            destinationRectangle.Y = (int)position.Y;

            base.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }
        public void DrawIsometric(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        { 
            base.Draw(texture, Vector2.Transform(position, _isometricTransformation), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
        public void DrawIsometric(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        { 
            base.Draw(texture, Vector2.Transform(position, _isometricTransformation), sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
