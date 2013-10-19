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

        private Dictionary<string, Tile> _tileTypes;
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
        /// GameLevel handles all game activity
        /// This constructor creates a new GameLevel with provided tile resolution
        /// and populates with a default tile
        /// </summary>
        /// <param name="gameScreen">GameScreen which contains this GameLevel</param>
        /// <param name="numTileRows">Number of tile rows in this GameLevel</param>
        /// <param name="numTileCols">Number of tile columns in this GameLevel</param>
        /// <param name="defaultTile">TileName for specified default tile. Check tiles.xml for TileNames</param>
        public GameLevel(GameScreen gameScreen, int numTileRows, int numTileCols, string defaultTile)
        {
            Construct(gameScreen);

            NewLevel(numTileRows, numTileCols, _tileTypes[defaultTile]);
        }

        /// <summary>
        /// GameLevel handles all game activity
        /// This constructor loads a GameLevel from a provided file
        /// </summary>
        /// <param name="gameScreen">GameScreen which contains this GameLevel</param>
        /// <param name="filename">Filename to load this GameLevel from</param>
        public GameLevel(GameScreen gameScreen, string filename)
        {
            Construct(gameScreen);

            LoadLevel(filename);
        }

        /// <summary>
        /// Common constructor code
        /// Should be called by all constructors
        /// </summary>
        private void Construct(GameScreen gameScreen)
        {
            _gameScreen = gameScreen;               // Set the game screen to the screen that brought us here
            _gameLayers = new List<GameLayer>();    // Initialize GameLayers list so we can add to it
            LoadTileTypes();                        // Load all tile types so we can use them as references
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

        #region Saving & Loading Code
        /// <summary>
        /// Create a new GameLevel with the specified tile resolution
        /// and populate it using the specified default tile
        /// </summary>
        /// <param name="numTileRows">Number of tile rows</param>
        /// <param name="numTileCols">Number of tile columns</param>
        /// <param name="defaultTile">This tile will populate the entire level</param>
        public void NewLevel(int numTileRows, int numTileCols, Tile defaultTile)
        {
            // ---------- Prepare for new level ----------
            ClearLevel();

            // ---------- Create camera ----------
            _camera = new Camera2D(_gameScreen.GraphicsDevice);
            
            // ---------- Create isometric layer ----------
            GameLayerIsometric isometricLayer = new GameLayerIsometric(this, numTileRows, numTileCols, defaultTile);
            _gameLayers.Add(isometricLayer);

            // Example code, changing one of the tiles
            TileReferencer refTile = isometricLayer.TileReferences[9, 4];
            refTile.Elevation = 200;
            refTile.ReferenceTile = _tileTypes["GrassTile"];
        }

        /// <summary>
        /// Loads the specified GameLevel from a provided XML file
        /// </summary>
        /// <param name="filename">Filename to load. Do NOT include extension</param>
        public void LoadLevel(string filename)
        { 
            // ---------- Prepare for loading ----------
            ClearLevel();
            string loadPath = "Levels\\" + filename;
            GameLevelData levelData = Content.Load<GameLevelData>(loadPath);

            // ---------- Begin loading ----------
            int numLayers = levelData.GameLayerData.Count();
            for (int i = 0; i < numLayers; i++)
            {
                GameLayerData currLayerData = levelData.GameLayerData[i];
                Console.WriteLine(currLayerData);
            }

            // ---------- End loading ----------

            Console.WriteLine("Successfully loaded game level from filename: " + filename);
        }

        /// <summary>
        /// Save the current GameLevel to an XML file with the specified filename
        /// </summary>
        /// <param name="filename">Save file with this name. Do NOT include extension.</param>
        public void SaveLevel(string filename)
        {
            // ---------- Prepare for saving ----------
            string savePath = "..\\..\\..\\..\\IsometricProjectContent\\Levels\\" + filename + ".xml";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;


            // ---------- Begin data packaging ----------
            int numLayers = _gameLayers.Count;
            GameLevelData levelData = new GameLevelData(numLayers);

            for (int i = 0; i < numLayers; i++)
                levelData.GameLayerData[i] = _gameLayers[i].PackageData();
            // ---------- End data packaging ----------


            // ---------- Write the data to the specified filename ----------
            using(XmlWriter writer = XmlWriter.Create(savePath, settings))
                IntermediateSerializer.Serialize(writer, levelData, null);

            Console.WriteLine("Successfully saved game level to filename: " + filename);
        }

        /// <summary>
        /// Clear this GameLevel of all its data, but hang onto the GameScreen
        /// </summary>
        private void ClearLevel()
        {
            _gameLayers.Clear();
            Console.WriteLine("Current level cleared!");
        }

        /// <summary>
        /// Load all tile types into tile type dictionary
        /// Uses the tile's name as the key
        /// </summary>
        private void LoadTileTypes()
        {
            // ---------- Prepare for loading ----------
            _tileTypes = new Dictionary<string, Tile>();
            string tileLoadPath = "GameObjects\\";
            string textureLoadPath = "Textures\\";

            // ---------- Load our tiles.xml file ----------
            TileTypeData[] tileTypeDataArray = Content.Load<TileTypeData[]>(tileLoadPath + "tiles");
            foreach (TileTypeData tileTypeData in tileTypeDataArray)
            {
                Texture2D tileTexture = Content.Load<Texture2D>(textureLoadPath + tileTypeData.TextureFile);
                Tile tile = new Tile(tileTypeData.TileName, tileTexture, tileTypeData.TileFlags);
                _tileTypes.Add(tileTypeData.TileName, tile);
            }
        }
        #endregion
    }
}
