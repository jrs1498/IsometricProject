using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    /// <summary>
    /// Packages GameLevel data
    /// </summary>
    public class GameLevelData
    {
        #region Members
        public GameLayerData[] GameLayerData;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Parameterless constructor required for loading
        /// </summary>
        public GameLevelData() { }

        /// <summary>
        /// Packaging constructor
        /// </summary>
        /// <param name="gameLayersCount">Numbers of game layers</param>
        public GameLevelData(int gameLayersCount)
        {
            GameLayerData = new GameLayerData[gameLayersCount];
        }
        #endregion
    }

    /// <summary>
    /// Packages GameLayer data
    /// </summary>
    public class GameLayerData
    { 
        
    }

    /// <summary>
    /// Packages GameLayerIsometric data
    /// </summary>
    public class GameLayerIsometricData : GameLayerData
    {
        #region Members
        public TileReferencerData[] TileReferencers;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Parameterless constructor required for loading
        /// </summary>
        public GameLayerIsometricData() { }

        /// <summary>
        /// Packaging constructor
        /// </summary>
        /// <param name="numTiles">Number of tiles in the 2D array</param>
        public GameLayerIsometricData(int numTiles)
        {
            TileReferencers = new TileReferencerData[numTiles];
        }
        #endregion
    }

    /// <summary>
    /// Packages GameObject data
    /// </summary>
    public class GameObjectData
    {
    }

    /// <summary>
    /// Packages TileReferencer data
    /// </summary>
    public class TileReferencerData : GameObjectData
    {
        #region Members
        public int RowIndex;
        public int ColIndex;
        public string TileReferenceName;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Parameterless constructor required for loading
        /// </summary>
        public TileReferencerData() { }

        /// <summary>
        /// Packaging constructor
        /// </summary>
        /// <param name="rowIndex">2D Array row index</param>
        /// <param name="colIndex">2D Array column index</param>
        /// <param name="tileReferenceName">The reference tile</param>
        public TileReferencerData(int rowIndex, int colIndex, string tileReferenceName)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            TileReferenceName = tileReferenceName;
        }
        #endregion
    }
}
