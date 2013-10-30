using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject.Interface
{
    /// <summary>
    /// This is the main type for all objects contained in the GameInterface
    /// </summary>
    public class GameInterfaceObject
    {
        #region Attributes
        protected GameInterface _gameInterface;                 // GameInterface containing this InterfaceObject

        protected bool _visible;                                // Indicates whether or not to draw this InterfaceObject
        protected bool _updating;                               // Indicates whether or not to update this InterfaceObject

        protected bool _fadingIn;                               // If true, this InterfaceObject will fade in   ONLY set these booleans with their properties!
        protected bool _fadingOut;                              // If true, this InterfaceObject will fade out
        protected int _currentFadeFrame;                        // Indicates the current frame in the fading cycle

        protected Rectangle _rectangle;                         // Used for drawing and interaction detection
        protected Rectangle _sourceRect;                        // Allows us to draw only a portion of the texture
        protected Texture2D _texture;                           // This GameInterfaceObject's texture for draw
        #endregion

        #region Properties
        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Updating = value;

                Console.WriteLine("Visible: " + value);
            }
        }
        public virtual bool Updating
        {
            get { return _updating; }
            set { _updating = value; Console.WriteLine("Updating: " + value); }
        }

        public virtual bool FadingIn
        {
            set
            {
                _fadingIn = value;
                if (value)
                {
                    FadingOut = false;
                    Visible = true;
                }
            }
        }
        public virtual bool FadingOut
        {
            set
            {
                _fadingOut = value;
                if (value)
                    FadingIn = false;
            }
        }

        public virtual Vector2 Position
        {
            get
            {
                return new Vector2(
                    _rectangle.X,
                    _rectangle.Y);
            }
            set
            {
                _rectangle.X = (int)value.X;
                _rectangle.Y = (int)value.Y;
            }
        }
        public virtual Vector2 Size
        {
            get
            {
                return new Vector2(
                    _rectangle.X,
                    _rectangle.Y);
            }
            set
            {
                _rectangle.Width = (int)value.X;
                _rectangle.Height = (int)value.Y;
            }
        }

        public virtual int X
        {
            get { return _rectangle.X; }
            set
            {
                _rectangle.X = value;
                OnPositionChange();
            }
        }
        public virtual int Y
        {
            get { return _rectangle.Y; }
            set
            {
                _rectangle.Y = value;
                OnPositionChange();
            }
        }

        public virtual int Width
        {
            get { return _rectangle.Width; }
            set
            {
                _rectangle.Width = value;
                OnSizeChange();
            }
        }
        public virtual int Height
        {
            get { return _rectangle.Height; }
            set
            {
                _rectangle.Height = value;
                OnSizeChange();
            }
        }
        #endregion

        #region Events
        public virtual void OnPositionChange()
        { 
            // TODO: Add general position change logic here
        }
        public virtual void OnSizeChange()
        { 
            // TODO: Add general size change logic here
        }
        public virtual void OnClick()
        {
            Console.WriteLine(this + " has just been clicked");
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this function at their beginning
        /// </summary>
        /// <param name="gameInterface">GameInterface containing this object</param>
        /// <param name="texture">This InterfaceObject's draw texture</param>
        private void PreConstruct(GameInterface gameInterface, Texture2D texture, bool initiallyOpen)
        {
            _gameInterface = gameInterface;
            _texture = texture;

            Visible = initiallyOpen;
            if (initiallyOpen)
                _currentFadeFrame = _gameInterface.FadeTotalFrames;
        }
        /// <summary>
        /// Common constructor code
        /// All constructors should call this function at their end
        /// </summary>
        private void PostConstruct()
        {
            _sourceRect = _rectangle;
        }

        /// <summary>
        /// Create an InterfaceObject using textures width and height properties
        /// </summary>
        public GameInterfaceObject(GameInterface gameInterface, Texture2D texture, bool initiallyOpen)
        {
            PreConstruct(gameInterface, texture, initiallyOpen);

            _rectangle.Width = texture.Width;
            _rectangle.Height = texture.Height;

            PostConstruct();
        }

        /// <summary>
        /// Create an InterfaceObject using specified width and height
        /// </summary>
        public GameInterfaceObject(GameInterface gameInterface, Texture2D texture, bool initiallyOpen, int width, int height)
        {
            PreConstruct(gameInterface, texture, initiallyOpen);

            _rectangle.Width = width;
            _rectangle.Height = height;

            PostConstruct();
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this InterfaceObject
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (_updating)
            {
                CheckForClick();
            }
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this InterfaceObject
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatchIsometric spriteBatch)
        {
            if (_visible)
            {
                // ---------- Draw this GameInterfaceObject ----------
                spriteBatch.Draw(
                    _texture,
                    _rectangle,
                    Color.White);
            }
        }

        protected Color ApplyFade(Color color)
        {
            if (_fadingIn || _fadingOut)
            {
                int fadeTotalFrames = _gameInterface.FadeTotalFrames;
                float opacity = (float)_currentFadeFrame / (float)fadeTotalFrames;
                color *= opacity;

                if (_fadingIn)
                {
                    // Fading in
                    _currentFadeFrame++;
                    if (_currentFadeFrame >= fadeTotalFrames)
                        FadingIn = false;
                }
                else
                { 
                    // Fading out
                    _currentFadeFrame--;
                    if (_currentFadeFrame <= 0)
                    {
                        FadingOut = false;
                        Visible = false;
                    }
                }
            }

            return color;
        }
        #endregion

        #region Interaction Checking Code
        /// <summary>
        /// Checks if this button has just been clicked on
        /// If it has, the OnClick function is fired
        /// </summary>
        public void CheckForClick()
        {
            if (Controller.GetOneLeftClickDown())
                if (IsMouseOver())
                    OnClick();
        }

        /// <summary>
        /// Returns true if the mouse is hovering over this InterfaceObject
        /// </summary>
        public bool IsMouseOver()
        {
            Vector2 mouseCoordinates = Controller.GetMouseLocation();
            Point mousePosition = new Point((int)mouseCoordinates.X, (int)mouseCoordinates.Y);
            return _rectangle.Contains(mousePosition);
        }
        #endregion
    }
}
