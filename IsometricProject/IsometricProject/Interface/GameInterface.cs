using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject.Interface
{
    /// <summary>
    /// GameInterface is the main container for all interface objects
    /// </summary>
    public class GameInterface
    {
        #region Attributes
        private AbstractScreen _screen;
        private List<GI_Obj> _objs;

        // ========== Interface Style Attributes ==========
        private SpriteFont _interfaceFont;

        private int _traceThickness         = 1;
        private int _titlebarThickness      = 20;
        private Color _traceColor           = new Color(200, 200, 200, 175);
        private Color _backdropColor        = new Color(0, 0, 0, 175);

        private int _initialWindowWidth     = 500;
        private int _initialWindowHeight    = 500;

        private int _windowButtonWidth      = 100;
        private int _windowButtonHeight     = 30;

        private int _dropdownButtonWidth    = 100;
        private int _dropdownButtonHeight   = 20;
        // ================================================
        // ======== Interface Animation Attributes ========
        private int _fadeOverFrames         = 10;
        private int _expandOverFrames       = 5;
        // ================================================
        #endregion

        #region Properties
        public AbstractScreen Screen
        {
            get { return _screen; }
        }

        public SpriteFont InterfaceFont
        {
            get { return _interfaceFont; }
        }
        public int TraceThickness
        {
            get { return _traceThickness; }
            set { _traceThickness = value; }
        }
        public int TitlebarThickness
        {
            get { return _titlebarThickness; }
            set { _titlebarThickness = value; }
        }
        public Color TraceColor
        {
            get { return _traceColor; }
            set { _traceColor = value; }
        }
        public Color BackdropColor
        {
            get { return _backdropColor; }
            set { _backdropColor = value; }
        }

        public int WindowButtonWidth
        {
            get { return _windowButtonWidth; }
        }
        public int WindowButtonHeight
        {
            get { return _windowButtonHeight; }
        }

        public int DropdownButtonWidth
        {
            get { return _dropdownButtonWidth; }
        }
        public int DropdownButtonHeight
        {
            get { return _dropdownButtonHeight; }
        }

        public int FadeOverFrames
        {
            get { return _fadeOverFrames; }
        }
        public int ExpandOverFrames
        {
            get { return _expandOverFrames; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Creates a new GameInterface, which provides access to all menus, windows and buttons
        /// </summary>
        public GameInterface(AbstractScreen screen)
        {
            _screen = screen;
            _objs = new List<GI_Obj>();

            // ---------- Load interface objects ----------
            _interfaceFont = _screen.Content.Load<SpriteFont>("Fonts\\interfacefont");
            Texture2D interfaceTexture = ColorTexture.Create(_screen.GraphicsDevice, Color.White);
            LoadInterface(interfaceTexture);
        }
        #endregion

        #region Load Interface Code
        private void LoadInterface(Texture2D texture)
        {
            GI_DropdownSubmenu submenu1 = new GI_DropdownSubmenu(this, texture);
            _objs.Add(submenu1);

            for (int i = 0; i < 5; i++)
            {
                GI_Obj randomobj = new GI_Obj(this, texture, 400, 400);
                submenu1.AddObject(randomobj, "item " + i);
            }

            submenu1.Position = new Vector2(200, 200);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this GameInterface
        /// </summary>
        public void Update(GameTime gameTime)
        {
            foreach (GI_Obj obj in _objs)
                obj.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this GameInterface
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (GI_Obj obj in _objs)
                obj.Draw(gameTime, spriteBatch, null);

            spriteBatch.End();
        }
        #endregion
    }


    public delegate void EventHandler();

    /// <summary>
    /// GI_Obj is the main type for all interface objects
    /// </summary>
    public class GI_Obj
    {
        #region Attributes
        protected static GameInterface  _gameInterface;
        protected Texture2D             _texture;
        protected Rectangle             _rectangle;
        protected Boolean               _hovering;
        protected Boolean               _visible;
        protected Boolean               _isOpen;
        #endregion

        #region Properties
        public Texture2D Texture
        {
            get { return _texture; }
        }

        public Vector2 Position
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
                if (PositionChanged != null)
                    PositionChanged();
            }
        }
        public Vector2 Size
        {
            get
            {
                return new Vector2(
                    _rectangle.Width,
                    _rectangle.Height);
            }
            set
            {
                _rectangle.Width = (int)value.X;
                _rectangle.Height = (int)value.Y;
                if (SizeChanged != null)
                    SizeChanged();
            }
        }

        public int Left
        {
            get { return _rectangle.X; }
        }
        public int Right
        {
            get { return _rectangle.Right; }
        }
        public int Top
        {
            get { return _rectangle.Y; }
        }
        public int Bottom
        {
            get { return _rectangle.Bottom; }
        }
        public Point Center
        {
            get { return _rectangle.Center; }
        }

        public int Width
        {
            get { return _rectangle.Width; }
        }
        public int Height
        {
            get { return _rectangle.Height; }
        }

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public virtual Boolean Hovering
        {
            get { return _hovering; }
            set { _hovering = value; }
        }
        public virtual Boolean IsOpen
        {
            get { return _isOpen; }
            set { _isOpen = value; }
        }

        public SpriteFont InterfaceFont
        {
            get { return _gameInterface.InterfaceFont; }
        }
        public int TraceThickness
        {
            get { return _gameInterface.TraceThickness; }
        }
        public int TitlebarThickness
        {
            get { return _gameInterface.TitlebarThickness; }
        }
        public Color TraceColor
        {
            get { return _gameInterface.TraceColor; }
        }
        public Color BackdropColor
        {
            get { return _gameInterface.BackdropColor; }
        }
        #endregion

        #region Events
        public event EventHandler Clicked;
        public event EventHandler PositionChanged;
        public event EventHandler SizeChanged;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct(GameInterface gameInterface, Texture2D texture, int width, int height)
        {
            _gameInterface = gameInterface;
            _texture = texture;

            _rectangle.Width = width;
            _rectangle.Height = height;

            _hovering = false;
            _visible = true;
            _isOpen = true;
        }

        /// <summary>
        /// Create a new GI_Obj with specified size
        /// </summary>
        public GI_Obj(GameInterface gameInterface, Texture2D texture, int width, int height)
        {
            Construct(gameInterface, texture, width, height);
        }

        /// <summary>
        /// Create a new GI_Obj with specified size and position
        /// </summary>
        public GI_Obj(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y)
        {
            Construct(gameInterface, texture, width, height);

            _rectangle.X = x;
            _rectangle.Y = y;
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this GI_Obj
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            CheckHovering();

            if (_hovering)
                if (Controller.GetOneLeftClickDown())
                    OnClick();
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this GI_Obj
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            if (_visible)
            {
                Rectangle drawRect;
                if (rectangle != null)
                    drawRect = (Rectangle)rectangle;
                else
                    drawRect = _rectangle;

                spriteBatch.Draw(
                    _texture,
                    drawRect,
                    BackdropColor * fadeAmount);
            }
        }
        #endregion

        #region Interaction Code
        /// <summary>
        /// Fired whenever the mouse clicks on this GI_Obj
        /// </summary>
        private void OnClick()
        {
            if (Clicked != null)
                Clicked();
        }

        /// <summary>
        /// Check if the mouse is currently hovering over this GI_Obj
        /// </summary>
        private void CheckHovering()
        {
            Vector2 mousePosition = Controller.GetMouseLocation();
            Point mousePoint;
            mousePoint.X = (int)mousePosition.X;
            mousePoint.Y = (int)mousePosition.Y;

            if (_rectangle.Contains(mousePoint))
            {
                if (!Hovering)
                    Hovering = true;
            }
            else
            {
                if (Hovering)
                    Hovering = false;
            }
        }

        public virtual void Open()
        {
            Console.WriteLine(this + " will open");
        }

        public virtual void Close()
        { 
            
        }
        #endregion
    }


    /// <summary>
    /// GI_Container is any interface object which contains other interface object
    /// </summary>
    public class GI_Container : GI_Obj
    {
        #region Attributes
        protected List<GI_Obj> _objs;
        #endregion

        #region Properties
        public override bool Hovering
        {
            get { return base.Hovering; }
            set
            {
                base.Hovering = value;
                foreach (GI_Obj obj in _objs)
                    obj.Hovering = false;
            }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct()
        {
            _objs = new List<GI_Obj>();
        }

        /// <summary>
        /// Create a new GI_Container with specified size
        /// </summary>
        public GI_Container(GameInterface gameInterface, Texture2D texture, int width, int height)
            : base(gameInterface, texture, width, height)
        {
            Construct();
        }

        /// <summary>
        /// Create a new GI_Container with specified size and position
        /// </summary>
        public GI_Container(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y)
            : base(gameInterface, texture, width, height, x, y)
        {
            Construct();
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this GI_Container and all of its contained objects
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // ---------- Update this ----------
            base.Update(gameTime);

            // ---------- Update contained objs ----------
            if (_hovering)
                foreach (GI_Obj obj in _objs)
                    obj.Update(gameTime);
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this GI_Container and all of its contained objects
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="fadeAmount"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, float fadeAmount = 1.0f)
        {
            if (_visible)
            {
                // ---------- Draw contained objs ----------
                foreach (GI_Obj obj in _objs)
                    obj.Draw(gameTime, spriteBatch, null, fadeAmount);
            }
        }
        #endregion
    }
}
