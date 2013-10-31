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
        private GameLayer _gameLayer;                   // GameLayer containing this TileSystem
        private bool _updatePlayerControls;             // Indicates whether or not to update player controls

        // ----- Main tile system -----
        private const int TILE_SIZE = 70;               // The face size of a tile (square / cube)
        private TileRef[,] _tiles;                      // 2D tile array
        private int _numRows;                           // Used for array iteration
        private int _numCols;

        // ----- Edit mode -----
        private EditMode _currentEditMode;              // Indicates in what way the tile system is currently being edited
        private Tool _currentTool;                      // Refers to the tool currently being used
        private short _currentTileReference;            // Refers to the currently selected tile used for placing

        // ----- Selection -----
        private Vector2 _previousMouseTileIndex;        // Which tile the mouse was hovering over on the last frame
        private Vector2 _currentMouseTileIndex;         // Which tile the mouse is currently hovering over

        private SelectionType _currentSelectionType = SelectionType.clickdrag;    // Indicates how we are currently selecting tiles
        private List<Vector2> _selectedIndices;         // List of Vector2 containing index information for selected tiles
        private int _selectionSize;                     // Square size of tile selection
        private Texture2D _selectionTexture;            // Texture to indicate tile tool selection
        private Texture2D _selectionTextureDraw;        // Texture currently drawn to the selected area
        private Color _selectionColor;                  // Default draw color
        private Color _selectionColorDraw;              // Color which will draw for the selected tiles
        #endregion

        #region Properties
        public bool UpdatePlayerControls
        {
            get { return _updatePlayerControls; }
            set { _updatePlayerControls = value; }
        }

        public ContentManager Content
        {
            get { return _gameLayer.Content; }
        }
        public ContentLibrary ContentLib
        {
            get { return _gameLayer.ContentLib; }
        }
        private Dictionary<short, CL_ObjType> TileTypes
        {
            get { return ContentLib.GetLoadedFile("tiletypes"); }
        }

        public Tool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
                _currentEditMode = EditMode.terrain;
                _selectionTextureDraw = _selectionTexture;
            }
        }
        public short CurrentTileReference
        {
            get { return _currentTileReference; }
            set
            {
                _currentTileReference = value;
                _currentEditMode = EditMode.tile;
                _selectionTextureDraw = TileTypes[value].Texture;
            }
        }
        public SelectionType CurrentSelectionType
        {
            get { return _currentSelectionType; }
            set { _currentSelectionType = value; }
        }
        #endregion

        #region Enum
        public enum EditMode
        { 
            terrain = 1,    // Modify TileSystem terrain
            tile = 2        // Modify Tiles
        }
        public enum Tool
        { 
            elevate = 0,        // Raise tiles
            smooth = 1,         // Smooth tiles
            zeroelevation = 2   // Bring all tiles to 0 elevation
        }
        public enum SelectionType
        { 
            clickdrag = 1,
            squareselect = 2
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
            _selectionTextureDraw = _selectionTexture;
            _selectionColor = new Color(150, 150, 150, 150);
            _selectionColorDraw = _selectionColor;
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
        /// <summary>
        /// Update this TileSystem
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Only run controller updates if we currently have control
            if (_updatePlayerControls)
            {
                // Check if we are editing this TileSystem
                if (_currentEditMode != 0)
                {
                    // We are editing, let the user select tiles
                    UpdateMouseTileIndex();

                    // This handles tile selection and modification
                    switch (_currentSelectionType)
                    {
                        case SelectionType.clickdrag:
                            HandleClickDragSelection();
                            break;

                        case SelectionType.squareselect:
                            HandleSquareSelection();
                            break;
                    }
                }
            }
        }

        #region Selection
        /// <summary>
        /// Update MouseTileIndex attributes
        /// </summary>
        private void UpdateMouseTileIndex()
        {
            _previousMouseTileIndex = _currentMouseTileIndex;

            Camera2D cam = _gameLayer.GameLevel.Camera;
            SpriteBatchIsometric sb = _gameLayer.GameLevel.SpriteBatch;

            Vector2 mouseCoordinates = Controller.GetMouseLocation();
            mouseCoordinates -= cam.Origin;
            mouseCoordinates.X += cam.Displacement.X;
            mouseCoordinates.Y += cam.Displacement.Y;

            mouseCoordinates = sb.IsometricToCartesian(mouseCoordinates);

            _currentMouseTileIndex = GetIndexFromPosition(mouseCoordinates);
        }

        /// <summary>
        /// Lets the user click and drag out a selection of tiles
        /// </summary>
        private void HandleClickDragSelection()
        {
            // Once the mouse clicks, we are grabbing a new selection
            if (Controller.GetOneLeftClickDown())
            {
                _selectedIndices.Clear();
                if (VerifyIndexWithinRange(_currentMouseTileIndex))
                    _selectedIndices.Add(_currentMouseTileIndex);

                _selectionColorDraw = new Color(150, 250, 150, 150);
            }
            // If the left mouse is currently down, we are dragging a selection
            else if (Controller.IsLeftMouseDown())
            {
                // The first index acts as the origin
                // If there is no origin (clicked outside of bounds), do nothing
                if (_selectedIndices.Count == 0)
                    return;

                // If the mouse has changed tiles, grab the new selection
                if (MouseChangedTile())
                {
                    // Grab the origin, clear the list, and then reinsert the origin
                    Vector2 origin = _selectedIndices[0];
                    _selectedIndices.Clear();
                    _selectedIndices.Add(origin);

                    // Determine how many rows and columns we need
                    int numRows = (int)(_currentMouseTileIndex.X - origin.X);
                    int numCols = (int)(_currentMouseTileIndex.Y - origin.Y);

                    // Grab the indecies
                    for (int i = 0; i != numRows; i += Math.Abs(numRows) / numRows)
                        for (int j = 0; j != numCols; j += Math.Abs(numCols) / numCols)
                        {
                            Vector2 newIndex = origin + new Vector2(i, j);
                            if (newIndex != origin)
                                _selectedIndices.Add(newIndex);
                        }
                }
            }
            // If the left mouse is up, we have no selection
            else if (Controller.GetOneLeftClickUp())
            {
                _selectedIndices.Clear();
                _selectionColorDraw = _selectionColor;
            }

            // If we get a rick mouse click, apply the modification
            if (Controller.GetOneRightClickDown())
            {
                ProcessSelectionModification();
            }
        }

        /// <summary>
        /// Grabs a square selection of tiles based on the mouse location and selection size
        /// </summary>
        private void HandleSquareSelection()
        {
            // Allow the user to resize the square selection
            if (Controller.GetOneKeyPressDown(Keys.OemCloseBrackets))
                _selectionSize++;
            if (Controller.GetOneKeyPressDown(Keys.OemOpenBrackets))
            {
                _selectionSize--;
                if (_selectionSize < 1)
                    _selectionSize = 1;
            }

            // Grab the tile indices from where the mouse is
            if (MouseChangedTile())
            {
                // Grabbing new selection, clear the old one
                _selectedIndices.Clear();
                Vector2 mouseIndex = _currentMouseTileIndex;

                // -1.0f index indicates no tiles are selected
                if (mouseIndex.X != -1.0f)
                {
                    for (int i = 0; i < _selectionSize; i++)
                        for (int j = 0; j < _selectionSize; j++)
                        {
                            int rowIndex = (int)mouseIndex.X + i;
                            int colIndex = (int)mouseIndex.Y + j;

                            if (VerifyIndexWithinRange(rowIndex, colIndex))
                                _selectedIndices.Add(new Vector2(rowIndex, colIndex));
                        }
                }
            }
        }

        /// <summary>
        /// Get a tile from the 2D tile array based on the supplied index
        /// </summary>
        private TileRef GetTileFromIndex(Vector2 index)
        {
            int rowIndex = (int)index.X;
            int colIndex = (int)index.Y;
            return _tiles[rowIndex, colIndex];
        }
        #endregion

        /// <summary>
        /// Processes tile modification
        /// </summary>
        private void ProcessSelectionModification()
        {
            switch (_currentEditMode)
            { 
                case EditMode.terrain:
                    UseTool(1.0f);
                    break;

                case EditMode.tile:
                    PlaceTile();
                    break;
            }
        }

        #region Terrain Editing
        private void UseTool(float strength)
        {
            switch (_currentTool)
            { 
                case Tool.elevate:
                    Elevate((byte)strength);
                    break;

                case Tool.smooth:
                    Smooth(strength);
                    break;

                case Tool.zeroelevation:
                    ZeroElevation();
                    break;
            }
        }

        private void Elevate(byte amount)
        {
            foreach (Vector2 index in _selectedIndices)
                GetTileFromIndex(index).Elevation += amount;
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
        private void ZeroElevation()
        {
            foreach (Vector2 index in _selectedIndices)
                GetTileFromIndex(index).Elevation = 0;
        }
        #endregion

        #region Tile Editing
        private void PlaceTile()
        {
            foreach (Vector2 index in _selectedIndices)
                GetTileFromIndex(index).ReferenceID = _currentTileReference;
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
            if (_selectedIndices.Count != 0)
            {
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
                        _selectionTextureDraw,
                        drawPosition,
                        _selectionColorDraw);
                }
            }
            else
            {
                if (VerifyIndexWithinRange(_currentMouseTileIndex))
                {
                    TileRef mouseTile = _tiles[(int)_currentMouseTileIndex.X, (int)_currentMouseTileIndex.Y];
                    Vector3 drawPosition;
                    drawPosition.X = _currentMouseTileIndex.X * TILE_SIZE;
                    drawPosition.Y = mouseTile.Elevation * TILE_SIZE;
                    drawPosition.Z = _currentMouseTileIndex.Y * TILE_SIZE;

                    spriteBatch.DrawIsometric(
                        _selectionTextureDraw,
                        drawPosition,
                        _selectionColorDraw);
                }
            }
        }
        #endregion

        #region Helper Functions
        private bool MouseChangedTile()
        {
            return (_currentMouseTileIndex != _previousMouseTileIndex);
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
