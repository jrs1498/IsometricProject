using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTypes;

namespace IsometricProject
{
    public class TileReferencer : GameObject
    {
        #region Attributes
        private Tile _tileReference;    // The Tile represented here
        #endregion

        #region Properties
        public Tile ReferenceTile
        {
            get { return _tileReference; }
            set
            {
                _tileReference = value;
                GetComponent<GOCDrawable>().Texture = _tileReference.Texture;
            }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// A TileReferencer is loaded into the GameLayerIsometric's 2D tile array.
        /// When a TileReferencer is interacted with, it grabs information from the
        /// Tile it references to see how it should respond.
        /// </summary>
        /// <param name="referenceTile">The Tile which will be referenced by this</param>
        public TileReferencer(int tileWidth, int tileHeight, Tile referenceTile)
            : base(tileWidth, tileHeight)
        {
            AddComponent<GOCDrawable>();        // Allow this tile to draw itself
            ReferenceTile = referenceTile;      // Never directly assign the reference tile, ALWAYS use this property
        }
        #endregion

        #region Data Packaging Code
        /// <summary>
        /// Package this TileReferencer data and return it
        /// </summary>
        /// <param name="rowIndex">GameLayerIsometric row index</param>
        /// <param name="colIndex">GameLayerIsometric column index</param>
        /// <returns>Packaged data for this TileReferencer</returns>
        public TileReferencerData PackageData(int rowIndex, int colIndex)
        {
            TileReferencerData tileReferencerData = new TileReferencerData(rowIndex, colIndex, "name placeholder");
            return tileReferencerData;
        }
        #endregion
    }
}
