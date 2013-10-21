using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DataTypes;

namespace IsometricProject
{
    public class TileSystem
    {
        #region Attributes
        private const int TILE_SIZE = 70;               // The face size of a tile (square / cube)
        private GameLayer _gameLayer;                   // GameLayer containing this TileSystem

        private TileReferencer[,] _tileReferencers;     // 2D array holding tile information
        private int _numRows;                           // Used for array iteration
        private int _numCols;

        private Dictionary<byte, TileType> _tileTypes;  // Dictionary holding all loaded TileTypes

        private List<Vector2> _selectedIndices;         // List of Vector2 containing index information for selected tiles
        private int _selectionSize;                     // Square size of tile selection
        private Texture2D _selectionTexture;            // Texture to draw over selected tiles
        #endregion

        #region Enum
        public enum EditMode : byte
        { 
            terrain = 1,    // Modify TileSystem terrain
            tile = 2        // Modify Tiles
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// Should be called by all constructors
        /// </summary>
        /// <param name="numRows">Number of tile rows</param>
        /// <param name="numCols">Number of tile columns</param>
        private void Construct(GameLayerTiled gameLayer, int numRows, int numCols)
        {
            // ---------- Load TileType dictionary ----------
            _gameLayer = gameLayer;
            ContentManager contentManager = gameLayer.GameLevel.Content;
            _tileTypes = new Dictionary<byte, TileType>();
            string tileLoadPath = "ReferenceData\\tiletypes";
            string textureLoadPath = "Textures\\";
            TileTypeData[] tileTypeData = contentManager.Load<TileTypeData[]>(tileLoadPath);
            for (int i = 0; i < tileTypeData.Count(); i++)
            {
                TileTypeData currData = tileTypeData[i];
                Texture2D tileTypeTexture = contentManager.Load<Texture2D>(textureLoadPath + currData.TextureFileName);
                TileType tileType = new TileType(currData.TileReferenceCode, tileTypeTexture, currData.TileFlags);
                _tileTypes.Add(currData.TileReferenceCode, tileType);
            }

            // ---------- Allocate tile array attributes ----------
            _tileReferencers = new TileReferencer[numRows, numCols];
            _numRows = numRows;
            _numCols = numCols;

            // ---------- Allocate tile selection attributes ----------
            _selectedIndices = new List<Vector2>();
            _selectionSize = 2;
            _selectionTexture = contentManager.Load<Texture2D>("Textures/tileselector");
        }

        /// <summary>
        /// Generate a new TileSystem
        /// </summary>
        /// <param name="defaultTileReferenceCode">TileReferenceCode for default tile which will fill the system</param>
        public TileSystem(GameLayerTiled gameLayer, int numRows, int numCols, byte defaultTileReferenceCode)
        {
            Construct(gameLayer, numRows, numCols);

            // ---------- Populate TileSystem with default tile ----------
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                {
                    TileReferencer tileReferencer = new TileReferencer(defaultTileReferenceCode, 0);
                    _tileReferencers[i, j] = tileReferencer;
                }
        }

        /// <summary>
        /// Create a TileSystem from specified data
        /// </summary>
        public TileSystem(GameLayerTiled gameLayer, TileSystemData tileSystemData)
        {
            int numRows = tileSystemData.NumRows;
            int numCols = tileSystemData.NumCols;
            Construct(gameLayer, numRows, numCols);

            // ---------- Populate the TileSystem with specified data ----------
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                {
                    TileReferencerData currTileData = tileSystemData.TileReferencers[j + (i * numCols)];
                    TileReferencer tileReferencer = new TileReferencer(currTileData.TileReferenceCode, currTileData.Elevation);
                    _tileReferencers[i, j] = tileReferencer;
                }
        }
        #endregion

        #region Update Code
        public void Update(GameTime gameTime)
        {
            SelectionSizeControls();
            SelectTerrainIndecies();
            ModifyTerrain();
        }

        #region Terrain Editing
        private void SelectionSizeControls()
        {
            if (Controller.GetOneKeyPressDown(Keys.OemCloseBrackets))
                _selectionSize++;
            if (Controller.GetOneKeyPressDown(Keys.OemOpenBrackets))
            {
                _selectionSize--;
                if (_selectionSize < 1)
                    _selectionSize = 1;
            }
        }
        private void SelectTerrainIndecies()
        {
            _selectedIndices.Clear();
            Vector2 mouseIndex = GetMouseOverTileIndex();

            if (mouseIndex.X != -1.0f)
                for (int i = 0; i < _selectionSize; i++)
                    for (int j = 0; j < _selectionSize; j++)
                    {
                        int rowIndex = (int)mouseIndex.X + i;
                        int colIndex = (int)mouseIndex.Y + j;

                        if (VerifyIndexWithinRange(rowIndex, colIndex))
                            _selectedIndices.Add(new Vector2(rowIndex, colIndex));
                    }
        }
        private void ModifyTerrain()
        {
            if (Controller.GetOneLeftClickDown())
                //Elevate(1);
                Smooth(0.5f);
        }

        private void Elevate(int amount)
        {
            foreach (Vector2 index in _selectedIndices)
            {
                int rowIndex = (int)index.X;
                int colIndex = (int)index.Y;

                _tileReferencers[rowIndex, colIndex].Elevation += (byte)amount;
            }
        }
        private void Smooth(float strength)
        {
            int numItems = _selectedIndices.Count;
            if (numItems == 0)
                return;
            int elevationSum = 0;

            // First pass lets us calculate the average
            foreach (Vector2 index in _selectedIndices)
                elevationSum += _tileReferencers[(int)index.X, (int)index.Y].Elevation;
            int average = elevationSum / numItems;
            
            // Second pass lets us set the elevations
            foreach (Vector2 index in _selectedIndices)
            {
                byte currentElevation = _tileReferencers[(int)index.X, (int)index.Y].Elevation;
                int modifyElevation = (int)((average - currentElevation) * strength);

                if (modifyElevation < 0)
                {
                    byte subtractBy = (byte)Math.Abs(modifyElevation);
                    _tileReferencers[(int)index.X, (int)index.Y].Elevation -= subtractBy;
                }
                else
                {
                    _tileReferencers[(int)index.X, (int)index.Y].Elevation += (byte)modifyElevation;
                }
            }
        }
        #endregion
        #endregion

        #region Draw Code
        public void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // ---------- Draw all tiles in this system ----------
            TileReferencer currTile;
            Texture2D currTexture;
            
            for (int i = 0; i < _numRows; i++)
                for (int j = 0; j < _numCols; j++)
                {
                    currTile = _tileReferencers[i, j];
                    currTexture = _tileTypes[currTile.TileReferenceCode].Texture;
            
                    spriteBatch.DrawIsometric(
                        currTexture,
                        GetPositionFromIndex(i, currTile.Elevation, j),
                        Color.White);
                }

            // ---------- Draw tile selection indicator ----------
            foreach (Vector2 selectedIndex in _selectedIndices)
            {
                int rowIndex = (int)selectedIndex.X;
                int colIndex = (int)selectedIndex.Y;
                byte elevation = _tileReferencers[rowIndex, colIndex].Elevation;

                spriteBatch.DrawIsometric(
                    _selectionTexture,
                    GetPositionFromIndex(rowIndex, elevation, colIndex),
                    Color.White);
            }
                    
        }
        #endregion

        #region Helper Functions
        private Vector2 GetMouseOverTileIndex()
        {
            Camera2D cam = _gameLayer.GameLevel.Camera;
            SpriteBatchIsometric sb = _gameLayer.GameLevel.SpriteBatch;

            Vector2 mouseCoordinates = Controller.GetMouseLocation();
            mouseCoordinates -= cam.Origin;
            mouseCoordinates.X += cam.Displacement.X;
            mouseCoordinates.Y += cam.Displacement.Y;

            mouseCoordinates = sb.IsometricToCartesian(mouseCoordinates);

            return GetIndexFromPosition(mouseCoordinates);
        }

        private Vector2 GetIndexFromPosition(Vector2 position)
        {
            float rowIndex = (position.X - (position.X % TILE_SIZE)) / TILE_SIZE;
            float colIndex = (position.Y - (position.Y % TILE_SIZE)) / TILE_SIZE;

            if (!VerifyIndexWithinRange((int)rowIndex, (int)colIndex))
                return new Vector2(-1.0f, -1.0f);

            return new Vector2(rowIndex, colIndex);
        }
        private Vector3 GetPositionFromIndex(int rowIndex, byte elevation, int colIndex)
        {
            Vector3 position = new Vector3(
                rowIndex * TILE_SIZE,
                elevation * TILE_SIZE,
                colIndex * TILE_SIZE);

            return position;
        }

        private bool VerifyIndexWithinRange(Vector2 index)
        {
            int rowIndex = (int)index.X;
            int colIndex = (int)index.Y;

            return VerifyIndexWithinRange(rowIndex, colIndex);
        }
        private bool VerifyIndexWithinRange(int rowIndex, int colIndex)
        {
            return !(rowIndex < 0 || rowIndex > _numRows - 1 || colIndex < 0 || colIndex > _numCols - 1);
        }
        #endregion

        #region Data Handling Code
        /// <summary>
        /// Package this TileSystems data and return the data type
        /// </summary>
        /// <returns>Packaged TileSystemData</returns>
        public TileSystemData PackageData()
        {
            // ---------- Prepare to package data ----------
            int numRows = _tileReferencers.GetLength(0);
            int numCols = _tileReferencers.GetLength(1);
            TileSystemData data = new TileSystemData(numRows, numCols);

            // ---------- Begin data package ----------
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                    data.TileReferencers[j + (i * numCols)] = _tileReferencers[i, j].PackageData();
            // ---------- End data package ----------

            return data;
        }
        #endregion
    }

    /// <summary>
    /// TileType class specifies different types of tiles which may be placed
    /// in the GameLevel. Only one of each TileType should exist in memory
    /// </summary>
    public class TileType
    {
        #region Attributes
        private byte _tileReferenceCode;    // Used by TileReferencers
        private Texture2D _texture;         // This TileType's texture
        private TileFlags _tileFlags;       // How this TileType may be used
        #endregion

        #region Properties
        public Texture2D Texture
        {
            get { return _texture; }
        }
        #endregion

        #region Enum
        [Flags]
        public enum TileFlags : byte
        { 
            walk = 1,
            build = 2
        }
        #endregion

        /// <summary>
        /// Creates a new TileType
        /// </summary>
        /// <param name="tileReferenceCode">Used by TileReferencers</param>
        /// <param name="texture">Texture to draw for this tile</param>
        /// <param name="tileFlags">TileType functionality</param>
        public TileType(byte tileReferenceCode, Texture2D texture, byte tileFlags = 0)
        {
            _tileReferenceCode = tileReferenceCode;
            _texture = texture;
            _tileFlags = (TileFlags)tileFlags;
        }

        /// <summary>
        /// Check if this TileType has the specified flag
        /// </summary>
        /// <param name="tileFlag">TileFlag to check</param>
        /// <returns>True if TileType has flag</returns>
        public bool HasFlag(TileFlags tileFlag)
        {
            return _tileFlags.HasFlag(tileFlag);
        }
    }

    /// <summary>
    /// A TileReferencer is loaded into a TileSystem's 2D tile array
    /// to represent a TileType somewhere in the world.
    /// </summary>
    public struct TileReferencer
    {
        #region Attributes
        private byte _tileReferenceCode;    // Corresponds to TileType.TileReferenceCode
        private byte _elevation;            // Elevation measured in units of TILE_SIZE
        #endregion

        #region Properties
        public byte TileReferenceCode
        {
            get { return _tileReferenceCode; }
            set { _tileReferenceCode = value; }
        }
        public byte Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;
                if (_elevation < 0)
                    _elevation = 0;
            }
        }
        #endregion

        /// <summary>
        /// Represents a TileType in the GameLevel
        /// </summary>
        /// <param name="tileReferenceCode">TileType reference code</param>
        /// <param name="elevation">Elevation in units of TILE_SIZE</param>
        public TileReferencer(byte tileReferenceCode, byte elevation)
        {
            _tileReferenceCode = tileReferenceCode;
            _elevation = elevation;
        }

        /// <summary>
        /// Package this TileReferencer and return its data file
        /// </summary>
        /// <returns>Packaged TileReferencerData</returns>
        public TileReferencerData PackageData()
        {
            return new TileReferencerData(_tileReferenceCode, _elevation);
        }
    }
}
