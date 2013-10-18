using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class GameLayerIsometric : GameLayer
    {
        #region Attributes
        private Tile[,] _tiles;
        private const int _tileWidth = 175;
        private const int _tileHeight = 175;
        #endregion

        #region Constructor Code
        /// <summary>
        /// A GameLayer which can hold isometric content
        /// </summary>
        public GameLayerIsometric(GameLevel gameLevel, float parallaxAmount, int numRows, int numCols)
            : base(gameLevel, parallaxAmount)
        {
            _tiles = new Tile[numRows, numCols];
            BuildTiles();
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

            // Draw tiles
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    Tile currTile = _tiles[i, j];
                    currTile.GetComponent<GOCDrawable>().DrawIsometric(gameTime, spriteBatch);
                }
            }

            // Draw any applicable GameObjects in this layer
            foreach (GameObject go in _gameObjects)
            {
                GOCDrawable drawableComponent = go.GetComponent<GOCDrawable>();
                if (drawableComponent != null)
                    drawableComponent.DrawIsometric(gameTime, spriteBatch);
            }

            // End drawing
            spriteBatch.End();
        }
        #endregion

        private void BuildTiles()
        {
            Texture2D tileTexture = _gameLevel.Content.Load<Texture2D>("Textures/tile");
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    Tile tile = new Tile(tileTexture);
                    tile.Size = new Vector2(_tileWidth, _tileHeight);
                    tile.Displacement = new Vector2(
                        i * _tileWidth,
                        j * _tileHeight);
                    _tiles[i, j] = tile;
                }
            }
        }
    }
}
