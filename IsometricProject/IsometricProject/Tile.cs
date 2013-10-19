using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    /// <summary>
    /// This class is used to represent types of Tiles. This class is NOT directly stored
    /// in the GameLayerIsometric's 2D tile array. A TileReferencer communicates with the Tile it
    /// references when interaction occurs in the GameLevel
    /// </summary>
    public class Tile
    {
        #region Attributes
        private string _tileName;       // Name used for referencing
        private Texture2D _texture;     // This tile's texture
        private TileFlags _tileFlags;   // Information regarding tile functions
        #endregion

        #region Properties
        public string TileName
        {
            get { return _tileName; }
        }
        public Texture2D Texture
        {
            get { return _texture; }
        }
        #endregion

        #region Enum
        /// <summary>
        /// Enum which indicates how this tile is capable of being used
        /// </summary>
        [Flags]
        public enum TileFlags
        { 
            walk = 1,
            build = 2
        }
        #endregion
        
        #region Constructor Code
        /// <summary>
        /// Create a new Tile with specified flags
        /// </summary>
        /// <param name="texture">This tiles texture</param>
        /// <param name="tileFlags">Add requested enum values together to get flags value</param>
        public Tile(string tileName, Texture2D texture, int tileFlags = 0)
        {
            _tileName = tileName;
            _texture = texture;
            _tileFlags = (TileFlags)tileFlags;
        }
        #endregion

        #region Functionality
        /// <summary>
        /// Check if this tile has a specified flag
        /// </summary>
        /// <param name="tileFlag">walk, build</param>
        /// <returns>True if tile has flag, else false</returns>
        public bool HasFlag(TileFlags tileFlag)
        {
            return _tileFlags.HasFlag(tileFlag);
        }
        #endregion
    }
}
