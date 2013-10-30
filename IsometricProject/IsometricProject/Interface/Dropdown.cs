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
        #region Attributes
        private GI_DropdownSubmenu _openSubmenu;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct()
        {
            _openSubmenu = null;
        }

        /// <summary>
        /// Create a DropdownMenu which spans across the top of the screen
        /// Specify width and height from supplied GameInterface
        /// </summary>
        /// <param name="gameInterface">GameInterface containing this DropdownMenu</param>
        /// <param name="texture">Texture to apply</param>
        public GI_DropdownMenu(GameInterface gameInterface, Texture2D texture, bool initiallyVisible = true)
            : base(gameInterface, texture, gameInterface.Screen.GraphicsDevice.Viewport.Width, gameInterface.DropdownButtonHeight, 0, 0, initiallyVisible)
        {
            Construct();
        }

        /// <summary>
        /// Creates a DropdownMenu which spans across the top of the screen
        /// Specify width and height manually
        /// </summary>
        /// <param name="gameInterface">GameInterface containing this DropdownMenu</param>
        /// <param name="texture">Texture to apply</param>
        /// <param name="width">Width of this menu</param>
        /// <param name="height">Height of this menu</param>
        public GI_DropdownMenu(GameInterface gameInterface, Texture2D texture, int width, int height, bool initiallyVisible = true)
            : base(gameInterface, texture, width, height, initiallyVisible)
        {
            Construct();
        }
        #endregion

        #region Update Code
        public override bool Update(GameTime gameTime)
        {
            // Only update if base updates
            if (base.Update(gameTime))
            {
                // If we click outside the menu, close any menu that may be open
                if (Controller.GetOneLeftClickDown() && !_hovering)
                    if (_openSubmenu != null)
                    {
                        _openSubmenu.Close();
                        _openSubmenu = null;
                    }

                // This updated, so return true
                return true;
            }

            // This did not update, so return false
            return false;
        }
        #endregion

        #region Containment Code
        /// <summary>
        /// Add a submenu to this dropdown menu
        /// </summary>
        /// <param name="submenu">Submenu to add</param>
        /// <param name="title">Button title</param>
        public void AddSubMenu(GI_DropdownSubmenu submenu, string title)
        {
            int width = _gameInterface.DropdownButtonWidth;
            int height = _gameInterface.DropdownButtonHeight;
            int x = _objs.Count * width;
            int y = Top;

            GI_DropdownButton button = new GI_DropdownButton(_gameInterface, _texture, width, height, x, y, title);
            _objs.Add(button);

            submenu.Visible = false;

            button.Clicked += delegate()
            {
                if (submenu.Visible)
                    submenu.Close();
                else
                {
                    if (_openSubmenu != null)
                        _openSubmenu.Close();

                    submenu.Open();
                    _openSubmenu = submenu;
                }
            };

            submenu.Position = new Vector2(
                button.Left,
                Bottom);
        }

        /// <summary>
        /// Add an object to this dropdown menu
        /// </summary>
        /// <param name="obj">GI_Obj to open upon clicking</param>
        /// <param name="title">Button title</param>
        public virtual void AddObject(GI_Obj obj, string title)
        {
            int width = _gameInterface.DropdownButtonWidth;
            int height = _gameInterface.DropdownButtonHeight;
            int x = _objs.Count * width;
            int y = Top;

            GI_DropdownButton button = new GI_DropdownButton(_gameInterface, _texture, width, height, x, y, title);
            _objs.Add(button);

            button.Clicked += obj.Open;
        }
        #endregion
    }


    /// <summary>
    /// DropdownSubmenu handles all dropdown submenu functionality
    /// </summary>
    public class GI_DropdownSubmenu : GI_DropdownMenu
    {
        public GI_DropdownSubmenu(GameInterface gameInterface, Texture2D texture)
            : base(gameInterface, texture, gameInterface.DropdownButtonWidth, gameInterface.DropdownButtonHeight, false)
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

        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            if (_opening || _closing)
            {
                float openFactor = HandleOpenClose();

                // Draw all contained items
                Rectangle drawRect;
                drawRect.Width = _objs[0].Width;
                drawRect.Height = _objs[0].Height;
                drawRect.X = _objs[0].Left;
                for (int i = 0; i < _objs.Count; i++)
                {
                    drawRect.Y = (int)(((float)_objs[i].Top) * openFactor);
                    _objs[i].Draw(gameTime, spriteBatch, drawRect, openFactor);
                }

                return true;
            }
            else
            {
                return base.Draw(gameTime, spriteBatch, null);
            }
        }

        #region Containment Code
        public void AddButtonForObj(GI_Obj obj, string title)
        { 
            AddButton(() => { obj.Open(); }, title);
        }

        public void AddButton(EventHandler clickEvent, string title)
        {
            int width = _gameInterface.DropdownButtonWidth;
            int height = _gameInterface.DropdownButtonHeight;
            int x = Left;
            int y = _objs.Count * height;

            GI_DropdownButton button = new GI_DropdownButton(_gameInterface, _texture, width, height, x, y, title);
            _objs.Add(button);

            button.Clicked += clickEvent;

            Size = new Vector2(
                Size.X,
                _objs.Count * height);
        }
        #endregion
    }


    /// <summary>
    /// DropdownButton is any button contained in the dropdown menu or submenu
    /// </summary>
    public class GI_DropdownButton : GI_Button
    {
        public GI_DropdownButton(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, string title)
            : base(gameInterface, texture, width, height, x, y, title)
        { 
            
        }
    }
}
