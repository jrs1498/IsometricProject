using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DataTypes;

namespace IsometricProject.Game
{
    /// <summary>
    /// GameLayerIsometric acts exactly as a GameLayer, except it contains tile information
    /// and it draws all content using an isometric view (with elevation)
    /// </summary>
    public class GameLayerTiled : GameLayer
    {
        #region Attributes
        private TileSystem _tileSystem;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Creates a GameLayer with a new TileSystem
        /// </summary>
        /// <param name="gameLevel">GameLevel containing this layer</param>
        /// <param name="numRows">Number of tile rows</param>
        /// <param name="numCols">Number of tile columns</param>
        /// <param name="defaultTileReferenceCode">Default tile to populate TileSystem</param>
        public GameLayerTiled(GameLevel gameLevel, int numRows, int numCols, byte defaultTileReferenceCode)
            : base(gameLevel)
        {
            _tileSystem = new TileSystem(this, numRows, numCols, defaultTileReferenceCode);
        }

        /// <summary>
        /// Creates a GameLayerTiled from a data file
        /// </summary>
        /// <param name="gameLevel">GameLevel containing this layer</param>
        /// <param name="layerData">Data for this GameLayerTiled</param>
        public GameLayerTiled(GameLevel gameLevel, GameLayerTiledData layerData)
            : base(gameLevel)
        {
            _tileSystem = new TileSystem(this, layerData.TileSystemData);
        }
        #endregion

        #region DrawCode
        /// <summary>
        /// Draw this layer
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // ---------- Begin drawing from this layer's perspective ----------
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camTransformation);

            // ---------- Draw TileSystem ----------
            _tileSystem.Draw(gameTime, spriteBatch);

            // ---------- Draw GameObjects ----------
            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.Draw(gameTime, spriteBatch);
            }

            // ---------- Drawing complete ----------
            spriteBatch.End();
        }
        #endregion

        #region Update Code
        public override void Update(GameTime gameTime)
        {
            _tileSystem.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Data Handling Code
        new public GameLayerTiledData PackageData()
        {
            GameLayerTiledData layerData = new GameLayerTiledData();
            layerData.TileSystemData = _tileSystem.PackageData();
            return layerData;
        }
        #endregion
    }
}
