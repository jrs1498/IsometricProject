using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsometricProject.Interface
{
    public class GI_DropdownMenu : GI_Container
    {
        #region Constructor Code
        /// <summary>
        /// Create a DropdownMenu which spans across the top of the screen
        /// </summary>
        /// <param name="gameInterface"></param>
        /// <param name="texture"></param>
        public GI_DropdownMenu(GameInterface gameInterface, Texture2D texture)
            : base(gameInterface, texture, gameInterface.Screen.GraphicsDevice.Viewport.Width, gameInterface.DropdownButtonHeight, 0, 0)
        { 
            
        }

        public GI_DropdownMenu(GameInterface gameInterface, Texture2D texture, int width, int height)
            : base(gameInterface, texture, width, height)
        { 
            
        }
        #endregion

        #region Draw Code
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            if (_visible)
            { 
                // ---------- Draw backdrop ----------
                spriteBatch.Draw(_texture, _rectangle, BackdropColor * fadeAmount);

                // ---------- Base draws buttons ----------
                base.Draw(gameTime, spriteBatch, fadeAmount);
            }
        }
        #endregion

        public void AddSubMenu(GI_DropdownSubmenu submenu, string title)
        {
            int width = _gameInterface.DropdownButtonWidth;
            int height = _gameInterface.DropdownButtonHeight;
            int x = _objs.Count * width;
            int y = Top;
            GI_DropdownButton button = new GI_DropdownButton(_gameInterface, _texture, width, height, x, y, title);
            _objs.Add(button);

            button.Clicked += delegate()
            {
                if (submenu.IsOpen)
                    submenu.Close();
                else
                    submenu.Open();
            };

            submenu.Position = new Vector2(
                Left,
                Bottom);
        }
    }

    public class GI_DropdownSubmenu : GI_DropdownMenu
    {
        public GI_DropdownSubmenu(GameInterface gameInterface, Texture2D texture)
            : base(gameInterface, texture, gameInterface.DropdownButtonWidth, gameInterface.DropdownButtonHeight)
        {
            PositionChanged += delegate()
            {
                for (int i = 0; i < _objs.Count; i++)
                {
                    GI_Obj currObj = _objs[i];
                    currObj.Position = new Vector2(
                        Left,
                        Top + (i * gameInterface.DropdownButtonHeight));
                }
            };
        }

        public override void Open()
        {
            Console.WriteLine("opening");
        }

        public override void Close()
        {
            Console.WriteLine("closing");
        }

        public void AddObject(GI_Obj obj, string title)
        { 
            int width = _gameInterface.DropdownButtonWidth;
            int height = _gameInterface.DropdownButtonHeight;
            int x = Left;
            int y = _objs.Count * height;
            GI_DropdownButton button = new GI_DropdownButton(_gameInterface, _texture, width, height, x, y, title);
            _objs.Add(button);

            button.Clicked += obj.Open;

            Size = new Vector2(
                Size.X,
                _objs.Count * height);
        }
    }


    public class GI_DropdownButton : GI_Button
    {
        public GI_DropdownButton(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, string title)
            : base(gameInterface, texture, width, height, x, y, title)
        { 
            
        }

        #region Draw Code
        /// <summary>
        /// Draw this GI_DropdownButton
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="rectangle"></param>
        /// <param name="fadeAmount"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            // ---------- Rectangle used for drawing ----------
            Rectangle drawRect;
            if (rectangle != null)
                drawRect = (Rectangle)rectangle;
            else
                drawRect = _rectangle;


            // ---------- Set colors and adjust fade ----------
            Color textColor;
            Color backdropColor;
            if (!_hovering)
            {
                textColor       = TraceColor;
                backdropColor   = BackdropColor;
            }
            else
            {
                textColor       = BackdropColor;
                backdropColor   = TraceColor;
            }
            textColor           *= fadeAmount;
            backdropColor       *= fadeAmount;


            // ---------- Draw backdrop ----------
            spriteBatch.Draw(_texture, drawRect, backdropColor);
            

            // ---------- Draw title ----------
            Vector2 textSize = InterfaceFont.MeasureString(_title);
            Point center = drawRect.Center;
            Vector2 textPosition;
            textPosition.X = center.X - (textSize.X / 2);
            textPosition.Y = center.Y - (textSize.Y / 2);
            spriteBatch.DrawString(InterfaceFont, _title, textPosition, textColor);
        }
        #endregion
    }
}
