using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using DataTypes;

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
            
            // TEST CODE
            AddIsometricLayer(1.0f, 20, 20);
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

        #region Loading & Saving Code
        public void LoadLevel(string filename)
        { 
            
        }

        public void SaveLevel(string filename)
        {
            // Set the save path and writer settings
            string savePath = "..\\..\\..\\..\\IsometricProjectContent\\Levels\\" + filename + ".xml";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            // ---------- Start Data Packaging ----------
            GameLevelData levelData = new GameLevelData(_gameLayers.Count);
            for (int i = 0; i < _gameLayers.Count; i++)
            {
                GameLayer currLayer = _gameLayers[i];
                GameLayerData layerData = new GameLayerData(currLayer.GetType(), currLayer.GameObjects.Count);
                for (int j = 0; j < currLayer.GameObjects.Count; j++)
                {
                    GameObject currObject = currLayer.GameObjects[j];
                    GameObjectData objectData = new GameObjectData();
                    objectData.position = currObject.Displacement;

                    layerData.gameObjects[j] = objectData;
                }

                levelData.layers[i] = layerData;
            }

            // ---------- End Data Packaging ----------

            // Write the file
            using (XmlWriter writer = XmlWriter.Create(savePath, settings))
                IntermediateSerializer.Serialize(writer, levelData, null);
        }

        /// <summary>
        /// Add a GameLayer to this level
        /// </summary>
        private void AddLayer(float parallaxAmount)
        {
            GameLayer layer = new GameLayer(this, parallaxAmount);
            _gameLayers.Add(layer);
        }

        /// <summary>
        /// Add an IsometricGameLayer with specified tile resolution to this level
        /// </summary>
        private void AddIsometricLayer(float parallaxAmount, int numRows, int numCols)
        {
            GameLayerIsometric layer = new GameLayerIsometric(this, parallaxAmount, numRows, numCols);
            _gameLayers.Add(layer);
        }
        #endregion
    }
}
