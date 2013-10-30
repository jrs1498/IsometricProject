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
        private GameScreen _screen;
        private List<GI_Obj> _objs;

        // ========== Interface Functionality Attributes ==========
        private char[] _validChars = 
        {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
        'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1',
        '2', '3', '4', '5', '6', '7', '8', '9'};
        private int _textInputCooldown      = 30;
        private int _labelLeftSpacing       = 10;
        private int _textBoxTopSpacing      = 20;
        // ========================================================

        // ========== Interface Style Attributes ==========
        private SpriteFont _interfaceFont;

        private int _traceThickness         = 2;
        private int _titlebarThickness      = 20;
        private Color _traceColor           = new Color(200, 200, 200, 175);
        private Color _backdropColor        = new Color(0, 0, 0, 175);

        private int _initialWindowWidth     = 700;
        private int _initialWindowHeight    = 500;

        private int _windowButtonWidth      = 100;
        private int _windowButtonHeight     = 30;

        private int _dropdownButtonWidth    = 100;
        private int _dropdownButtonHeight   = 20;
        // ================================================

        // ======== Interface Animation Attributes ========
        private int _openOverFrames         = 5;
        // ================================================
        #endregion

        #region Properties
        public GameScreen Screen
        {
            get { return _screen; }
        }

        public char[] ValidChars
        {
            get { return _validChars; }
        }
        public int TextInputCooldown
        {
            get { return _textInputCooldown; }
        }
        public int LabelLeftSpacing
        {
            get { return _labelLeftSpacing; }
        }
        public int TextBoxTopSpacing
        {
            get { return _textBoxTopSpacing; }
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

        public int OpenOverFrames
        {
            get { return _openOverFrames; }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Creates a new GameInterface, which provides access to all menus, windows and buttons
        /// </summary>
        public GameInterface(GameScreen screen)
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
            GI_DropdownMenu mainmenu = new GI_DropdownMenu(this, texture);
            _objs.Add(mainmenu);

            // ---------- File submenu ----------
            GI_DropdownSubmenu file = new GI_DropdownSubmenu(this, texture);
            _objs.Add(file);

            file.AddButton(() => { Console.WriteLine("New level placeholder button"); }, "New");
            file.AddButton(() => { _screen.GameLevel.SaveLevel("poopies"); }, "Save");
            file.AddButton(() => { Console.WriteLine("save as button"); }, "Save As");
            file.AddButton(() => { Console.WriteLine("Open file placeholder button"); }, "Open");

            mainmenu.AddSubMenu(file, "File");

            // ---------- Tools submenu ----------
            GI_DropdownSubmenu tools = new GI_DropdownSubmenu(this, texture);
            _objs.Add(tools);

            tools.AddButton(() => { _screen.GameLevel.MainLayer.TileSystem.CurrentTool = TileSystem.Tool.elevate; }, "Elevate");
            tools.AddButton(() => { _screen.GameLevel.MainLayer.TileSystem.CurrentTool = TileSystem.Tool.smooth; }, "Smooth");

            mainmenu.AddSubMenu(tools, "Tools");

            // ========== WINDOWS SUBMENU START ==========
            GI_DropdownSubmenu windows = new GI_DropdownSubmenu(this, texture);
            _objs.Add(windows);

            // ----- Content Browser -----
            GI_Window contentBrowser = new GI_Window(this, texture, _initialWindowWidth, _initialWindowHeight, 200, 200, "Content Browser");
            windows.AddButtonForObj(contentBrowser, "Content Browser");
            _objs.Add(contentBrowser);

            contentBrowser.MainCell.Split(true, 0.6f);
            contentBrowser.MainCell.Child2.Split(false, 0.7f);

            GI_WindowCell contentCell   = contentBrowser.MainCell.Child1;
            GI_WindowCell infoCell      = contentBrowser.MainCell.Child2.Child1;
            GI_WindowCell fileCell      = contentBrowser.MainCell.Child2.Child2;

            // Info cell
            Vector2 selectorSize = new Vector2(100, 140);
            Vector2 selectorPosition;
            selectorPosition.X = (infoCell.Left + (infoCell.Width / 2)) - (selectorSize.X / 2);
            selectorPosition.Y = infoCell.Top + 10;
            GI_WindowCellObj selectionIndicator = new GI_WindowCellObj(this, texture, (int)selectorSize.X, (int)selectorSize.Y, (int)selectorPosition.X, (int)selectorPosition.Y, infoCell);

            Vector2 textFieldSize = new Vector2(120, 16);
            GI_TextField textfieldRefernceID = new GI_TextField(this, texture, (int)textFieldSize.X, (int)textFieldSize.Y, infoCell.Left + (infoCell.Width / 2), infoCell.Bottom - 100, infoCell, "Reference ID:");
            GI_TextField textfieldTextureName = new GI_TextField(this, texture, (int)textFieldSize.X, (int)textFieldSize.Y, infoCell.Left + (infoCell.Width / 2), textfieldRefernceID.Bottom + 10, infoCell, "Texture Name:");

            // Content cell
            Dictionary<short, CL_ObjType> content = _screen.ContentLib.GetLoadedFile("tiletypes");
            int i = 0;
            foreach (KeyValuePair<short, CL_ObjType> element in content)
            {
                CL_ObjType tile = new CL_ObjType(element.Value.ReferenceID, element.Value.Texture);
                GI_ContentButton button = new GI_ContentButton(this, 50, 70, contentCell.Left + (i * 60), contentCell.Top, tile);
                contentCell.AddObject(button);
                button.Clicked += delegate()
                {
                    _screen.GameLevel.MainLayer.TileSystem.CurrentTileReference = tile.ReferenceID;
                    selectionIndicator.Texture = tile.Texture;
                    textfieldRefernceID.Text = "" + tile.ReferenceID;
                    textfieldTextureName.Text = tile.Texture.ToString();
                };
                i++;
            }

            // ----- Another Window -----

            mainmenu.AddSubMenu(windows, "Windows");
            // ========== WINDOWS SUBMENU END ==========
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
        #endregion

        #region Properties
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Rectangle Rectangle
        {
            get { return _rectangle; }
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

        public virtual Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public virtual Boolean Hovering
        {
            get { return _hovering; }
            set { _hovering = value; }
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
        public event EventHandler Opened;
        public event EventHandler Closed;
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
            Visible = true;
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
        /// Base update checks for mouse hovering and mouse clicks
        /// </summary>
        /// <returns>True if this is currently updatig</returns>
        public virtual bool Update(GameTime gameTime)
        {
            // Only update if this GI_Obj is visible
            if (_visible)
            {
                CheckHovering();

                // If mouse is hovering, check for a click
                if (_hovering)
                    if (Controller.GetOneLeftClickDown())
                        OnClick();

                // This has updated, so return true
                return true;
            }

            // This did not update, so return false
            return false;
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
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this GI_Obj
        /// </summary>
        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            // Only draw if this is currently visible
            if (_visible)
            {
                // If a rectangle was passed in, draw to it
                // Otherwise, draw to the objs rectangle
                Rectangle drawRect;
                if (rectangle != null)
                    drawRect = (Rectangle)rectangle;
                else
                    drawRect = _rectangle;

                // Draw
                spriteBatch.Draw(
                    _texture,
                    drawRect,
                    BackdropColor * fadeAmount);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }

        /// <summary>
        /// Draw this GI_Obj with a specified color
        /// </summary>
        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, Color backdropColor, float fadeAmount = 1.0f)
        {
            // Only draw if this is currently visible
            if (_visible)
            {
                // If a rectangle was passed in, draw to it
                // Otherwise, draw to the objs rectangle
                Rectangle drawRect;
                if (rectangle != null)
                    drawRect = (Rectangle)rectangle;
                else
                    drawRect = _rectangle;

                // Draw
                spriteBatch.Draw(
                    _texture,
                    drawRect,
                    backdropColor * fadeAmount);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
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
        /// Open this object
        /// </summary>
        public virtual void Open()
        {
            // Fire the Opened event
            Opened();
        }

        /// <summary>
        /// Close this object
        /// </summary>
        public virtual void Close()
        {
            // Fire the Closed event
            Closed();
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
        protected bool _opening;
        protected bool _closing;
        protected int _openCurrentFrame;
        protected float _openAnimationPercentage;

        protected Vector2 _previousPosition;
        #endregion

        #region Properties
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                _previousPosition = Position;
                base.Position = value;
            }
        }

        public override bool Hovering
        {
            get { return base.Hovering; }
            set
            {
                base.Hovering = value;
                if (!value)
                    foreach (GI_Obj obj in _objs)
                        obj.Hovering = false;
            }
        }
        public virtual bool Opening
        {
            get { return _opening; }
            set
            {
                _opening = value;
                if (value)
                    _closing = false;
            }
        }
        public virtual bool Closing
        {
            get { return _closing; }
            set
            {
                _closing = value;
                if (value)
                    _opening = false;
            }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct(bool visible)
        {
            // ---------- List to hold all contained objs ----------
            _objs = new List<GI_Obj>();

            // ---------- Set initial visibility ----------
            _visible = visible;
            if (visible)
                _openCurrentFrame = _gameInterface.OpenOverFrames;
            else
                _openCurrentFrame = 0;

            // ---------- Hook up events ----------
            PositionChanged += delegate()
            {
                foreach (GI_Obj obj in _objs)
                {
                    Vector2 relativePosition = obj.Position - _previousPosition;
                    obj.Position = Position + relativePosition;
                }
            };

            Opened += delegate() { Opening = true; Visible = true; };
            Closed += delegate() { Closing = true; };
        }

        /// <summary>
        /// Create a new GI_Container with specified size
        /// </summary>
        public GI_Container(GameInterface gameInterface, Texture2D texture, int width, int height, bool visible)
            : base(gameInterface, texture, width, height)
        {
            Construct(visible);
        }

        /// <summary>
        /// Create a new GI_Container with specified size and position
        /// </summary>
        public GI_Container(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, bool visible)
            : base(gameInterface, texture, width, height, x, y)
        {
            Construct(visible);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this GI_Container and all of its contained objects
        /// </summary>
        /// <param name="gameTime"></param>
        public override bool Update(GameTime gameTime)
        {
            // If base updates, then update this as well
            if (base.Update(gameTime))
            {
                // If mouse is hovering, update all contaied objs
                if (_hovering)
                    foreach (GI_Obj obj in _objs)
                        obj.Update(gameTime);

                // This updated, so return true
                return true;
            }

            // This did not update, so return false
            return false;
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this GI_Container and all of its contained objects
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="fadeAmount"></param>
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            // Adjust for fading
            if (_opening || _closing)
            {
                _openAnimationPercentage = HandleOpenClose();
                fadeAmount *= _openAnimationPercentage;
            }

            // If base draws, then draw this as well
            if (base.Draw(gameTime, spriteBatch, rectangle, fadeAmount))
            {
                // Draw all cotaied objs
                DrawContainedObjs(gameTime, spriteBatch, rectangle, fadeAmount);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }

        /// <summary>
        /// Draws all contained GI_Objs
        /// </summary>
        protected void DrawContainedObjs(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            foreach (GI_Obj obj in _objs)
                if (_rectangle.Intersects(obj.Rectangle))
                    obj.Draw(gameTime, spriteBatch, rectangle, fadeAmount);
        }

        /// <summary>
        /// Helps with opening and closing animation
        /// </summary>
        /// <returns>Value indicating how far open or close this item is</returns>
        protected float HandleOpenClose()
        {
            if (_opening)
                _openCurrentFrame++;
            else
                _openCurrentFrame--;

            if (_openCurrentFrame <= 0)
            {
                _openCurrentFrame = 0;
                Closing = false;
                Visible = false;
            }
            else if (_openCurrentFrame >= _gameInterface.OpenOverFrames)
            {
                _openCurrentFrame = _gameInterface.OpenOverFrames;
                Opening = false;
            }

            return (float)((float)_openCurrentFrame / (float)_gameInterface.OpenOverFrames);
        }
        #endregion
    }
}
