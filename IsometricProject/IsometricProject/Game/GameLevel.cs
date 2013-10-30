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

namespace IsometricProject.Game
{
    /// <summary>
    /// GameLevel handles all game activity, including loading and saving of game data
    /// </summary>
    public class GameLevel
    {
        #region Attributes
        private GameScreen _gameScreen;             // GameScreen containing this GameLevel
        private GameLayerTiled _mainLayer;          // Main GameLayer, containing TileSystem and game activity
        private List<GameLayer> _gameLayers;        // Other GameLayers (aesthetic purposes)
        private Camera2D _camera;                   // The camera used to view the GameLevel
        #endregion

        #region Properties
        public ContentManager Content
        {
            get { return _gameScreen.Content; }
        }
        public ContentLibrary ContentLib
        {
            get { return _gameScreen.ContentLib; }
        }
        public GraphicsDevice Graphics
        {
            get { return _gameScreen.GraphicsDevice; }
        }
        public SpriteBatchIsometric SpriteBatch
        {
            get { return _gameScreen.SpriteBatch; }
        }
        public Camera2D Camera
        {
            get { return _camera; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// Should be called by all constructors
        /// </summary>
        private void Construct(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;               // Set the game screen to the screen that brought us here
            _gameLayers = new List<GameLayer>();    // Initialize GameLayers list so we can add to it
        }

        /// <summary>
        /// Create a new GameLevel with the specified TileSystem properties
        /// </summary>
        /// <param name="numRows">Number of tile rows</param>
        /// <param name="numCols">Number of tile columns</param>
        /// <param name="defaultTileReferenceCode">Default tile to populate the level</param>
        public GameLevel(GameScreen gameScreen, int numRows, int numCols, byte defaultTileReferenceCode)
        {
            Construct(gameScreen);
            NewLevel(numRows, numCols, defaultTileReferenceCode);
        }

        /// <summary>
        /// Load a GameLevel from a file
        /// </summary>
        /// <param name="filename">File to load from</param>
        public GameLevel(GameScreen gameScreen, string filename)
        {
            Construct(gameScreen);
            LoadLevel(filename);
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

            // Update the main layer
            _mainLayer.Update(gameTime);

            // SAVE
            if (Controller.GetKeyDown(Keys.LeftControl))
                if (Controller.GetOneKeyPressDown(Keys.S))
                    SaveLevel("turdmonkey");
        }

        /// <summary>
        /// Allows the user to control the Camera2D
        /// </summary>
        private void CameraControls()
        {
            GOCMovable camMover = _camera.GetComponent<GOCMovable>() as GOCMovable;

            // Mover controls
            float moveSpeed = 50.0f;
            if (Controller.GetKeyDown(Keys.Left))
                camMover.Move(new Vector3(-moveSpeed, 0, 0));
            if (Controller.GetKeyDown(Keys.Right))
                camMover.Move(new Vector3(moveSpeed, 0, 0));
            if (Controller.GetKeyDown(Keys.Up))
                camMover.Move(new Vector3(0, -moveSpeed, 0));
            if (Controller.GetKeyDown(Keys.Down))
                camMover.Move(new Vector3(0, moveSpeed, 0));

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
        /// Draw this GameLevel
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Draw the main layer
            _mainLayer.Draw(gameTime, spriteBatch);
        }
        #endregion

        #region Saving & Loading Code
        /// <summary>
        /// Clears the current level and generates a new level
        /// </summary>
        /// <param name="numRows">Number of tile rows</param>
        /// <param name="numCols">Number of tile columns</param>
        /// <param name="defaultTileReferenceCode">Default tile to populate the GameLevel</param>
        public void NewLevel(int numRows, int numCols, byte defaultTileReferenceCode)
        {
            ClearLevel();
            _camera = new Camera2D(Graphics);
            _mainLayer = new GameLayerTiled(this, numRows, numCols, defaultTileReferenceCode);

            // Verification WriteLine
            Console.WriteLine("New level created!\n Tile rows: " + numRows + "\nTile cols: " + numCols);
        }

        /// <summary>
        /// Load a GameLevel from a specified file
        /// </summary>
        /// <param name="filename">File to load from</param>
        public void LoadLevel(string filename)
        {
            string loadPath = "Levels\\" + filename;
            GameLevelData levelData = Content.Load<GameLevelData>(loadPath);

            ClearLevel();
            _camera = new Camera2D(Graphics);
            _mainLayer = new GameLayerTiled(this, levelData.MainLayerData);

            // Verification WriteLine
            Console.WriteLine("Load successfull!\n" + loadPath);
        }

        /// <summary>
        /// Save this GameLevel to the specified filename
        /// </summary>
        /// <param name="filename">Filename to save as</param>
        public void SaveLevel(string filename)
        {
            // ---------- Prepare for data packaging ----------
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            string savePath = "..\\..\\..\\..\\IsometricProjectContent\\Levels\\" + filename + ".xml";
            GameLevelData levelData = new GameLevelData(_gameLayers.Count());
            
            // ---------- Beging packaging level ----------
            levelData.MainLayerData = _mainLayer.PackageData();

            // ---------- Write the file ----------
            using (XmlWriter writer = XmlWriter.Create(savePath, settings))
                IntermediateSerializer.Serialize(writer, levelData, null);

            // Verification WriteLine
            Console.WriteLine("Current level saved!\n" + savePath);
        }

        /// <summary>
        /// Completely clears this GameLevel
        /// </summary>
        private void ClearLevel()
        {
            _mainLayer = null;
            _gameLayers.Clear();
            _camera = null;

            // Verification WriteLine
            Console.WriteLine("Current level cleared!");
        }
        #endregion
    }
}
