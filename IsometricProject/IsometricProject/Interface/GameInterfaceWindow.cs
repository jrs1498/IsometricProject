using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject.Interface
{
    public class GameInterfaceWindow : GameInterfaceObject
    {
        #region Attributes
        private string _windowTitle;        // This window's title
        #endregion

        #region Properties
        public string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; }
        }
        public Vector2 WindowInteriorPosition
        {
            get
            {
                return new Vector2(
                    _rectangle.X + WindowTraceThickness,
                    _rectangle.Y + WindowTitleBarThickness);
            }
        }
        public Vector2 WindowInteriorSize
        {
            get
            {
                return new Vector2(
                    _rectangle.Width - (WindowTraceThickness * 2),
                    _rectangle.Height - WindowTraceThickness - WindowTitleBarThickness);
            }
        }

        public int WindowTraceThickness
        {
            get { return _gameInterface.WindowTraceThickness; }
        }
        public int WindowTitleBarThickness
        {
            get { return _gameInterface.WindowTitleBarThickness; }
        }
        public Color WindowTraceColor
        {
            get { return _gameInterface.WindowTraceColor; }
        }
        public Color WindowBackdropColor
        {
            get { return _gameInterface.WindowBackdropColor; }
        }
        public SpriteFont InterfaceFont
        {
            get { return _gameInterface.InterfaceFont; }
        }
        #endregion

        #region Constructor Code
        public GameInterfaceWindow(GameInterface gameInterface, Texture2D texture, bool initiallyOpen, string windowTitle)
            : base(gameInterface, texture, initiallyOpen)
        {
            _windowTitle = windowTitle;
        }
        #endregion

        #region Update Code
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this window and its components
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            if (_visible)
            {
                // ---------- Draw the window ----------
                DrawMainWindow(gameTime, spriteBatch);

                // ---------- Draw window components ----------
            }
        }

        /// <summary>
        /// Draws the outline, backdrop and title bar of the window
        /// </summary>
        private void DrawMainWindow(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            // Use the GameInterface's window properties for drawing this window
            Rectangle drawRect;
            Color backdropColor = ApplyFade(WindowBackdropColor);
            Color traceColor = ApplyFade(WindowTraceColor);
            int titleBarThickness = WindowTitleBarThickness;
            int traceThickness = WindowTraceThickness;
            SpriteFont interfaceFont = InterfaceFont;

            // Draw the left and right edges
            for (int i = 0; i < 2; i++)
            { 
                drawRect.Width = traceThickness;
                drawRect.Height = _rectangle.Height - traceThickness - titleBarThickness;
                drawRect.X = _rectangle.X + ((i % 2) * (_rectangle.Width - traceThickness));
                drawRect.Y = _rectangle.Y + titleBarThickness;
                spriteBatch.Draw(_texture, drawRect, traceColor);
            }

            // Draw the bottom edge
            drawRect.Width = _rectangle.Width;
            drawRect.Height = traceThickness;
            drawRect.X = _rectangle.X;
            drawRect.Y = _rectangle.Y + _rectangle.Height - traceThickness;
            spriteBatch.Draw(_texture, drawRect, traceColor);

            // Draw the title bar
            drawRect.Height = titleBarThickness;
            drawRect.Y = _rectangle.Y;
            spriteBatch.Draw(_texture, drawRect, traceColor);

            // Draw the window title
            float drawY =
                drawRect.Y + drawRect.Height - (drawRect.Height / 2.0f)
                - interfaceFont.MeasureString(_windowTitle).Y / 2.0f;
            Vector2 drawPos;
            drawPos.X = drawRect.X + 10;
            drawPos.Y = drawY;
            spriteBatch.DrawString(interfaceFont, _windowTitle, drawPos, backdropColor);

            // Draw the backdrop
            drawRect.Width = _rectangle.Width - (traceThickness * 2);
            drawRect.Height = _rectangle.Height - titleBarThickness - traceThickness;
            drawRect.X = _rectangle.X + traceThickness;
            drawRect.Y = _rectangle.Y + titleBarThickness;
            spriteBatch.Draw(_texture, drawRect, backdropColor);
        }
        #endregion
    }
}
