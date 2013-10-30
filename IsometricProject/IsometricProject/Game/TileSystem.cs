using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DataTypes;

namespace IsometricProject.Game
{
    public class TileSystem
    {
        #region Attributes
        private GameLayer _gameLayer;                   // GameLayer containing this TileSystem

        private const int TILE_SIZE = 70;               // The face size of a tile (square / cube)
        private TileRef[,] _tiles;                      // 2D tile array
        private int _numRows;                           // Used for array iteration
        private int _numCols;

        private List<Vector2> _selectedIndices;         // List of Vector2 containing index information for selected tiles
        private int _selectionSize;                     // Square size of tile selection
        private Texture2D _selectionTexture;            // Texture to draw over selected tiles
        #endregion

        #region Properties
        public ContentManager Content
        {
            get { return _gameLayer.Content; }
        }
        public ContentLibrary ContentLib
        {
            get { return _gameLayer.ContentLib; }
        }
        public Dictionary<short, CL_ObjType> TileTypes
        {
            get { return ContentLib.GetLoadedFile("tiletypes"); }
        }
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
            _gameLayer = gameLayer;

            // ---------- Load TileTypes into ContentLibrary ----------
            ContentLib.LoadTypesFromFile<TileTypeData, TileType>("ReferenceData\\", "tiletypes");

            // ---------- Allocate 2D tile array ----------
            _tiles = new TileRef[numRows, numCols];
            _numRows = numRows;
            _numCols = numCols;

            // ---------- Allocate tile selection attributes ----------
            _selectedIndices = new List<Vector2>();
            _selectionSize = 2;
            _selectionTexture = Content.Load<Texture2D>("Textures/tileselector");
        }

        /// <summary>
        /// Create a new TileSystem
        /// </summary>
        /// <param name="defaultTile">ReferenceID for default tile which will fill the system</param>
        public TileSystem(GameLayerTiled gameLayer, int numRows, int numCols, short defaultTile)
        {
            Construct(gameLayer, numRows, numCols);

            // ---------- Populate 2D tile array with specified tile ----------
            for (int i = 0; i < _numRows; i++)
                for (int j = 0; j < _numCols; j++)
                {
                    TileRef tile = new TileRef(defaultTile, 0);
                    _tiles[i, j] = tile;
                }
        }

        /// <summary>
        /// Load a TileSystem from data file
        /// </summary>
        public TileSystem(GameLayerTiled gameLayer, TileSystemData tileSystemData)
        {
            Construct(gameLayer, tileSystemData.NumRows, tileSystemData.NumCols);

            // ---------- Populate 2D tile array with specified data ----------
            for (int i = 0; i < _numRows; i++)
                for (int j = 0; j < _numCols; j++)
                {
                    int current1Dindex = j + (i * _numCols);
                    TileRef tile = new TileRef(tileSystemData.Tiles[current1Dindex]);
                    _tiles[i, j] = tile;
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
                Smooth(1.0f);
        }

        private void Elevate(int amount)
        {
            foreach (Vector2 index in _selectedIndices)
            {
                int rowIndex = (int)index.X;
                int colIndex = (int)index.Y;
            
                _tiles[rowIndex, colIndex].Elevation += (byte)amount;
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
                elevationSum += _tiles[(int)index.X, (int)index.Y].Elevation;
            int average = elevationSum / numItems;
            
            // Second pass lets us set the elevations
            foreach (Vector2 index in _selectedIndices)
            {
                byte currentElevation = _tiles[(int)index.X, (int)index.Y].Elevation;
                int modifyElevation = (int)((average - currentElevation) * strength);
            
                if (modifyElevation < 0)
                {
                    byte subtractBy = (byte)Math.Abs(modifyElevation);
                    _tiles[(int)index.X, (int)index.Y].Elevation -= subtractBy;
                }
                else
                {
                    _tiles[(int)index.X, (int)index.Y].Elevation += (byte)modifyElevation;
                }
            }
        }
        #endregion
        #endregion

        #region Draw Code
        public void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // ---------- Draw all tiles ----------
            for (int i = 0; i < _numRows; i++)
                for (int j = 0; j < _numCols; j++)
                {
                    TileRef currTile = _tiles[i, j];

                    Vector3 tilePosition;
                    tilePosition.X = i * TILE_SIZE;
                    tilePosition.Y = currTile.Elevation * TILE_SIZE;
                    tilePosition.Z = j * TILE_SIZE;

                    spriteBatch.DrawIsometric(
                        TileTypes[currTile.ReferenceID].Texture,
                        tilePosition,
                        Color.White);
                }

            // ---------- Draw selection indicators ----------
            foreach (Vector2 index in _selectedIndices)
            {
                int x = (int)index.X;
                int y = (int)index.Y;
                TileRef currTile = _tiles[x, y];

                Vector3 drawPosition;
                drawPosition.X = x * TILE_SIZE;
                drawPosition.Y = currTile.Elevation * TILE_SIZE;
                drawPosition.Z = y * TILE_SIZE;

                spriteBatch.DrawIsometric(
                    _selectionTexture,
                    drawPosition,
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
            TileSystemData data = new TileSystemData(_numRows, _numCols);

            // ---------- Begin data package ----------
            for (int i = 0; i < _numRows; i++)
                for (int j = 0; j < _numCols; j++)
                    data.Tiles[j + (i * _numCols)] = _tiles[i, j].PackageData();
            // ---------- End data package ----------

            return data;
        }
        #endregion
    }


    /// <summary>
    /// TileType class specifies different types of tiles which may be placed
    /// in the GameLevel. Only one of each TileType should exist in memory
    /// </summary>
    public class TileType : CL_ObjType
    {
        #region Attributes
        private TileFlags _tileFlags;               // How this TileType may be used
        [Flags]
        public enum TileFlags : byte
        { 
            walk = 1,
            build = 2
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// </summary>
        private void Construct(byte tileFlags)
        {
            _tileFlags = (TileFlags)tileFlags;
        }

        /// <summary>
        /// Create a new TileType
        /// </summary>
        /// <param name="referenceID">Assigned referenceID</param>
        /// <param name="texture">Drawing texture</param>
        /// <param name="tileFlags">Check class definition</param>
        public TileType(short referenceID, Texture2D texture, byte tileFlags = 0)
            : base(referenceID, texture)
        {
            Construct(tileFlags);
        }

        /// <summary>
        /// Load a TileType from data
        /// </summary>
        /// <param name="data">Object containing TileType data</param>
        /// <param name="texture">Drawing texture</param>
        public TileType(TileTypeData data, Texture2D texture)
            : base(data.ReferenceID, texture)
        {
            Construct(data.TileFlags);
        }
        #endregion

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
    /// TileRef is loaded into TileSystems 2D tile array
    /// References a TileType stored in the ContentLibrary
    /// </summary>
    public class TileRef : CL_Type
    {
        private byte _elevation;        // Indicates level of elevation in units of TILE_SIZE
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

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// </summary>
        private void Construct(byte elevation)
        {
            _elevation = elevation;
        }

        /// <summary>
        /// Create a new TileRef
        /// </summary>
        /// <param name="referenceID">TileType this TileRef references</param>
        /// <param name="elevation">Elevation in units of TILE_SIZE</param>
        public TileRef(short referenceID, byte elevation)
            : base(referenceID)
        {
            Construct(elevation);
        }

        /// <summary>
        /// Load a TileRef from specified data
        /// </summary>
        /// <param name="data">TileRefData object containing data</param>
        public TileRef(TileRefData data)
            : base(data.ReferenceID)
        {
            Construct(data.Elevation);
        }
        #endregion

        /// <summary>
        /// Package this TileReferencer and return its data file
        /// </summary>
        /// <returns>Packaged TileReferencerData</returns>
        public TileRefData PackageData()
        {
            return new TileRefData(_referenceID, _elevation);
        }
    }
}
