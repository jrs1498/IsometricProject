using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DataTypes;

namespace IsometricProject
{
    public class GameLayerIsometric : GameLayer
    {
        #region Attributes
        private const int TILE_WIDTH    = 70;
        private const int TILE_HEIGHT   = 70;

        private TileReferencer[,] _tileReferences;      // This 2D array holds tile information
        #endregion

        #region Properties
        public TileReferencer[,] TileReferences
        {
            get { return _tileReferences; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// GameLayer which draws content using an isometric view
        /// This constructor creates a new GameLayerIsometric
        /// </summary>
        /// <param name="gameLevel">GameLevel containing this layer</param>
        /// <param name="numTileRows">Number of tile rows</param>
        /// <param name="numTileCols">Number of tile columns</param>
        /// <param name="defaultTile">Fills the entire layer with this tile</param>
        public GameLayerIsometric(GameLevel gameLevel, int numTileRows, int numTileCols, Tile defaultTile)
            : base(gameLevel)
        {
            // ---------- Initialize tile array resolution ----------
            _tileReferences = new TileReferencer[numTileRows, numTileCols];
            
            // ---------- Populate tile array with default tile ----------
            for (int i = 0; i < numTileRows; i++)
                for (int j = 0; j < numTileCols; j++)
                {
                    TileReferencer tileReferencer = new TileReferencer(TILE_WIDTH, TILE_HEIGHT, defaultTile);
                    tileReferencer.Displacement = new Vector2(i * TILE_WIDTH, j * TILE_HEIGHT);
                    _tileReferences[i, j] = tileReferencer;
                }
        }
        #endregion

        #region DrawCode
        /// <summary>
        /// Draw any applicable GameObjects to the isometric coordinates specified by SpriteBatchIsometric
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Begin drawing from this GameLayer's perspective
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camTransformation);

            // ---------- Start drawing tiles ----------

            for (int i = 0; i < _tileReferences.GetLength(0); i++)
                for (int j = 0; j < _tileReferences.GetLength(1); j++)
                    _tileReferences[i, j].GetComponent<GOCDrawable>().DrawIsometricElevated(gameTime, spriteBatch);

            // ---------- End drawing tiles ----------


            // ---------- Start drawing GameObjects ----------

            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.DrawIsometric(gameTime, spriteBatch);
            }

            // ---------- End drawing GameObjects ----------

            // End drawing from this GameLayer
            spriteBatch.End();
        }
        #endregion

        #region Saving & Loading Code
        /// <summary>
        /// Packages this GameLayerIsometric data and returns it
        /// </summary>
        public override GameLayerData PackageData()
        {
            int numRows = _tileReferences.GetLength(0);
            int numCols = _tileReferences.GetLength(1);
            GameLayerIsometricData layerData = new GameLayerIsometricData(numRows * numCols);

            // ---------- Package the TileReferences in a 1D array ----------
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                    layerData.TileReferencers[j + (numRows * i)] = _tileReferences[i, j].PackageData(i, j);

            return layerData;
        }
        #endregion
    }
}
