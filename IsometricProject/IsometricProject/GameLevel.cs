using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IsometricProject
{
    public class GameLevel
    {
        #region Attributes
        private GameScreen _gameScreen;
        private List<GameLayer> _gameLayers;
        private Camera2D _camera;
        #endregion

        #region Properties
        public ContentManager Content
        {
            get { return _gameScreen.Content; }
        }
        public Camera2D Camera
        {
            get { return _camera; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Create a GameLevel which handles all game world activity
        /// </summary>
        public GameLevel(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;
            _gameLayers = new List<GameLayer>();
            _camera = new Camera2D(_gameScreen.GraphicsDevice);

            // TEST CODDE
            AddLayer(5.0f);
            AddLayer(1.0f);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update all GameLevel information
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update the camera first, otherwise transformation may fail
            _camera.Update(gameTime);

            // Give the user control of the camera
            CameraControls();

            // Update all GameLayers in this GameLevel
            foreach (GameLayer layer in _gameLayers)
                layer.Update(gameTime);
        }

        /// <summary>
        /// Allows the user to control the Camera2D
        /// </summary>
        private void CameraControls()
        {
            GOCMovable camMover = _camera.GetComponent<GOCMovable>() as GOCMovable;

            // Mover controls
            float moveSpeed = 50.0f;
            if (Controller.GetKeyDown(Keys.A))
                camMover.Move(new Vector2(-moveSpeed, 0));
            if (Controller.GetKeyDown(Keys.D))
                camMover.Move(new Vector2(moveSpeed, 0));
            if (Controller.GetKeyDown(Keys.W))
                camMover.Move(new Vector2(0, -moveSpeed));
            if (Controller.GetKeyDown(Keys.S))
                camMover.Move(new Vector2(0, moveSpeed));

            // Zoom controls
            float zoomSpeed = 1.1f;
            if (Controller.GetKeyDown(Keys.OemPlus))
                _camera.Zoom *= zoomSpeed;
            if (Controller.GetKeyDown(Keys.OemMinus))
                _camera.Zoom /= zoomSpeed;
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw all GameLevel information
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Draw all GameLayers in this GameLevel
            foreach (GameLayer layer in _gameLayers)
                layer.Draw(gameTime, spriteBatch);
        }
        #endregion

        #region Containment Code
        /// <summary>
        /// Use this function to add a layer to this GameLevel
        /// </summary>
        private void AddLayer(float parallaxAmount)
        {
            GameLayerIsometric layer = new GameLayerIsometric(this, parallaxAmount);
            _gameLayers.Add(layer);

            // TEST CODE
            Texture2D testTexture = Content.Load<Texture2D>("Textures/point");

            int testrows = 20;
            int testcols = 20;
            int testwidth = 200;
            int testheight = 200;

            for (int i = 0; i < testrows; i++)
            {
                for (int j = 0; j < testcols; j++)
                {
                    TestObject testObj = new TestObject(testTexture);
                    testObj.Displacement = new Vector2(
                        i * testheight,
                        j * testwidth);
                    layer.AddObject(testObj);
                }
            }
        }
        #endregion
    }
}
