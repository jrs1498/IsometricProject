using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IsometricProject.Interface
{
    /// <summary>
    /// GI_Window is the main container and controller class for all interface objects contained in a window.
    /// Windows do not contain content, they contain GI_WindowCells which contain content
    /// </summary>
    public class GI_Window : GI_Container
    {
        #region Attributes
        private string _title;
        private GI_WindowCell _mainCell;

        private bool _repositioning;
        private Vector2 _grabPosition;
        #endregion

        #region Properties
        public GI_WindowCell MainCell
        {
            get { return _mainCell; }
        }
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;
                if (!value)
                    _repositioning = false;
            }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct(string title)
        {
            _title = title;

            // ---------- Create main cell ----------
            _mainCell = new GI_WindowCell(_gameInterface, _texture,
                Width - (TraceThickness * 2), Height - (TraceThickness + TitlebarThickness),
                Left + TraceThickness, Top + TitlebarThickness, this);
            _objs.Add(_mainCell);

            // ---------- Add main window buttons ----------
            int buttonSize = TitlebarThickness - (TraceThickness * 2);

            GI_Button closeButton = new GI_Button(_gameInterface, _texture, buttonSize, buttonSize,
                Right - (buttonSize + TraceThickness), Top + TraceThickness, "X");
            closeButton.Clicked += delegate() { this.Close(); };
            _objs.Add(closeButton);
        }

        /// <summary>
        /// Create a new window
        /// </summary>
        /// <param name="gameInterface"></param>
        public GI_Window(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, string title)
            : base(gameInterface, texture, width, height, x, y, false)
        {
            Construct(title);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// GI_Window update hadles window modification
        /// </summary>
        /// <returns>True if this is updating</returns>
        public override bool Update(GameTime gameTime)
        {
            // Only update this if base updates
            if (base.Update(gameTime))
            {
                HandleRepositioning();

                // This updated, so return true
                return true;
            }

            // This did not update, so return false
            return false;
        }

        /// <summary>
        /// Checks if the user is trying to reposition the window
        /// </summary>
        private void HandleRepositioning()
        {
            if (Controller.GetOneLeftClickDown())
            {
                Rectangle titleBar = _rectangle;

                titleBar.Height = TitlebarThickness;
                Vector2 mousePosition = Controller.GetMouseLocation();
                Point mousePoint;
                mousePoint.X = (int)mousePosition.X;
                mousePoint.Y = (int)mousePosition.Y;

                if (titleBar.Contains(mousePoint))
                {
                    _repositioning = true;
                    _grabPosition = mousePosition - Position;
                }
            }

            if (_repositioning)
            {
                Position = Controller.GetMouseLocation() - _grabPosition;

                if (Controller.GetOneLeftClickUp())
                    _repositioning = false;
            }
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// GI_Widow draw hadles window outline drawing and fading
        /// </summary>
        /// <returns>True if currently drawing</returns>
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            if (base.Draw(gameTime, spriteBatch, rectangle, fadeAmount))
            {
                // Declare colors we will use
                Color traceColor = TraceColor;
                Color backdropColor = BackdropColor;

                // If the window is opening or closing, we must apply fade in or out
                if (_opening || _closing)
                {
                    traceColor *= _openAnimationPercentage;
                    backdropColor *= _openAnimationPercentage;
                }

                // This rectangle will be used to draw the tracelines
                Rectangle drawRect = _rectangle;

                // ---------- Left and right sides ----------
                drawRect.Width = TraceThickness;
                drawRect.Height -= TraceThickness + TitlebarThickness;
                drawRect.Y += TitlebarThickness;
                for (int i = 0; i < 2; i++)
                {
                    drawRect.X += ((i % 2) * (Width - TraceThickness));
                    spriteBatch.Draw(_texture, drawRect, traceColor);
                }

                // ---------- Bottom side ----------
                drawRect.Width = Width;
                drawRect.Height = TraceThickness;
                drawRect.X = Left;
                drawRect.Y = Bottom - TraceThickness;
                spriteBatch.Draw(_texture, drawRect, traceColor);

                // ---------- Title bar ----------
                drawRect.Height = TitlebarThickness;
                drawRect.Y = Top;
                spriteBatch.Draw(_texture, drawRect, traceColor);

                // ---------- Title ----------
                Vector2 titlePosition;
                titlePosition.X = drawRect.X + 10;
                titlePosition.Y = drawRect.Bottom - (drawRect.Height / 2);
                Vector2 titleSize = InterfaceFont.MeasureString(_title);
                titlePosition.Y -= titleSize.Y / 2;
                spriteBatch.DrawString(InterfaceFont, _title, titlePosition, backdropColor);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }
        #endregion
    }


    /// <summary>
    /// GI_WindowCell acts as a container within a window that may hold content. A window by itself cannot hold content,
    /// only a window cell can.
    /// </summary>
    public class GI_WindowCell : GI_Container
    {
        #region Attributes
        private GI_Window _window;                  // Window containing this cell
        private Vector2 _viewPosition;              // Indicates our view position within the cell

        private Boolean _split;                     // Indicates whether or not this cell has been split
        private Boolean _vertSplit;                 // If split, indicates horizontal or vertical split
        private GI_WindowCell _childCell1;
        private GI_WindowCell _childCell2;
        #endregion

        #region Properties
        public GI_Window Window
        {
            get { return _window; }
        }
        public Vector2 ViewPosition
        {
            get { return _viewPosition; }
            set { _viewPosition = value; }
        }

        public Boolean IsSplit
        {
            get { return _split; }
        }
        public GI_WindowCell Child1
        {
            get { return _childCell1; }
        }
        public GI_WindowCell Child2
        {
            get { return _childCell2; }
        }
        #endregion

        #region Constructor Code
        public GI_WindowCell(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, GI_Window window)
            : base(gameInterface, texture, width, height, x, y, true)
        {
            _window = window;
            _viewPosition = Vector2.Zero;
            _split = false;

            // ---------- Hook up events ----------
            PositionChanged += delegate()
            {
                if (_split)
                {
                    _childCell1.Position = Position;
                    Vector2 relativePosition = _childCell2.Position - _previousPosition;
                    _childCell2.Position = Position + relativePosition;
                }
            };
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this window cell
        /// </summary>
        public override bool Update(GameTime gameTime)
        {
            if (base.Update(gameTime))
            {
                // If there are child cells, update them
                if (_split)
                {
                    _childCell1.Update(gameTime);
                    _childCell2.Update(gameTime);
                }

                // This updated, so return true
                return true;
            }

            // This did not update, so return false
            return false;
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this window cell
        /// If this cell contains child cells, then this cell will draw the border line between cdells
        /// </summary>
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            // This will only ever be called by a currently visible window,
            // so it does not need to check if it is currently drawing

            // Check if there are children. If there are, draw them instead
            if (_split)
            {
                // Draw child cells
                _childCell1.Draw(gameTime, spriteBatch, rectangle, fadeAmount);
                _childCell2.Draw(gameTime, spriteBatch, rectangle, fadeAmount);

                // Set border line
                Rectangle drawRect = _rectangle;
                if (_vertSplit)
                {
                    // Needs a vertical border line
                    drawRect.Width = TraceThickness;
                    drawRect.X = _childCell1.Right;
                }
                else
                {
                    // Needs a horizontal border line
                    drawRect.Height = TraceThickness;
                    drawRect.Y = _childCell1.Bottom;
                }

                // Draw border line
                spriteBatch.Draw(_texture, drawRect, TraceColor * fadeAmount);
            }
            // If there are no children, draw the contents of this cell
            else
            { 
                // Draw this cell
                DrawContainedObjs(gameTime, spriteBatch, rectangle, fadeAmount);
            }

            // This call will always return true
            return true;
        }
        #endregion

        #region Containment Code
        /// <summary>
        /// Add a GI_Obj to this cell and adjust the cell accordingly
        /// </summary>
        public void AddObject(GI_Obj obj)
        {
            _objs.Add(obj);
        }

        /// <summary>
        /// Split this cell into two child cells
        /// </summary>
        /// <param name="vertical">True for vertical, false for horizontal</param>
        /// <param name="splitRatio">The cross section where the split occurs</param>
        public void Split(bool vertical, float splitRatio = 0.5f)
        {
            // This cell should not contain anything, but we must guarantee that it doesn't
            _objs.Clear();

            // New cell sizes
            Vector2 size1 = new Vector2(Width, Height);
            Vector2 size2 = new Vector2(Width, Height);

            // New cell positions
            Vector2 position1 = new Vector2(Left, Top);
            Vector2 position2 = new Vector2(Left, Top);

            if (vertical)
            {
                // Split this cell vertically
                size1.X         = Width * splitRatio;
                size2.X         = Width - size1.X - TraceThickness;
                position2.X     = Left + size1.X + TraceThickness;

                _vertSplit      = true;
            }
            else
            { 
                // Splits this cell horizontally
                size1.Y         = Height * splitRatio;
                size2.Y         = Height - size1.Y - TraceThickness;
                position2.Y     = Top + size1.Y + TraceThickness;

                _vertSplit      = false;
            }

            // Create the new cells
            _childCell1 = new GI_WindowCell(_gameInterface, _texture, (int)size1.X, (int)size1.Y, (int)position1.X, (int)position1.Y, _window);
            _childCell2 = new GI_WindowCell(_gameInterface, _texture, (int)size2.X, (int)size2.Y, (int)position2.X, (int)position2.Y, _window);

            // Update split boolean to indicate there are children
            _split = true;
        }
        #endregion
    }


    /// <summary>
    /// Any GI_Obj contained within a GI_WindowCell
    /// </summary>
    public class GI_WindowCellObj : GI_Obj
    {
        public GI_WindowCellObj(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, GI_WindowCell cell)
            : base(gameInterface, texture, width, height, x, y)
        {
            cell.AddObject(this);
            cell.Window.Closed += Close;

            Closed += delegate()
            {
                // TODO: Add closing event code here
            };
        }
    }


    public class GI_Label : GI_WindowCellObj
    {
        #region Attributes
        private string _text;
        private GI_WindowCellObj _obj;
        private Vector2 _textPosition;
        #endregion

        #region Properties
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                AdjustTextPosition();
            }
        }
        #endregion

        #region Constructor Code
        public GI_Label(GameInterface gameInterface, Texture2D texture, GI_WindowCell cell, string text, GI_WindowCellObj obj)
            : base(gameInterface, texture, obj.Width, obj.Height, obj.Left, obj.Top, cell)
        {
            _obj            = obj;
            _textPosition   = Vector2.Zero;
            Text            = text;

            // ---------- Hook up events ----------
            _obj.PositionChanged += delegate()
            {
                AdjustTextPosition();
            };
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this label to the left of its object
        /// </summary>
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            if (_visible)
            {
                Color textColor = TraceColor;
                spriteBatch.DrawString(InterfaceFont, _text, _textPosition, textColor);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }
        #endregion

        public void AdjustTextPosition()
        {
            Vector2 textBoxSize = InterfaceFont.MeasureString(_text);
            _textPosition = _obj.Position;
            _textPosition.X -= textBoxSize.X + _gameInterface.LabelLeftSpacing;
            _textPosition.Y += (_obj.Height / 2) - (textBoxSize.Y / 2);
        }
    }


    /// <summary>
    /// Allows user to input text
    /// </summary>
    public class GI_TextField : GI_WindowCellObj
    {
        #region Attributes
        private string _text            = "";
        private int _textCount          = 0;
        private bool _editing           = false;
        private bool _active            = true;
        private Keys _lastKeyInput      = Keys.None;
        private int _lastPress          = 0;
        private int _cursorIndex        = 0;
        #endregion

        #region Properties
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _textCount = _text.Count();
            }
        }
        public bool Editing
        {
            get { return _editing; }
            set
            {
                _editing = value;
            }
        }
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
            }
        }
        #endregion

        #region Events
        public event EventHandler Apply;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Common constructor code
        /// All constructors should call this method
        /// </summary>
        private void Construct()
        {
            // ---------- Hook up events ----------
            Clicked += delegate()
            {
                Editing = true;
            };

            Closed += delegate()
            {
                Editing = false;
            };

            Apply += delegate()
            {
                Editing = false;
            };
        }

        /// <summary>
        /// Create a new text field
        /// </summary>
        public GI_TextField(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, GI_WindowCell cell)
            : base(gameInterface, texture, width, height, x, y, cell)
        {
            Construct();
        }

        /// <summary>
        /// Create a new text field with a label
        /// </summary>
        public GI_TextField(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, GI_WindowCell cell, string labelText)
            : base(gameInterface, texture, width, height, x, y, cell)
        {
            Construct();

            GI_Label label = new GI_Label(gameInterface, texture, cell, labelText, this);
            cell.AddObject(label);
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this text field
        /// </summary>
        public override bool Update(GameTime gameTime)
        {
            if (_active)
            {
                if (base.Update(gameTime))
                {
                    HandleInput();


                    // This updated, so return true;
                    return true;
                }
            }

            // This did not update, so return false
            return false;
        }

        /// <summary>
        /// Handles user input
        /// </summary>
        private void HandleInput()
        {
            if (_editing)
            { 
                // User is currently editing this text field
                Keys keyInput = Controller.GetPressedKey();
                _lastPress++;

                // If no key is pressed, reset last key input
                if (keyInput == Keys.None)
                {
                    _lastKeyInput = Keys.None;
                    return;
                }

                // Check if the current input is the same as previous input (key being held)
                if (_lastKeyInput == keyInput)
                {
                    // If so, verify we are beyond the cooldown limit before processing
                    if (_lastPress > _gameInterface.TextInputCooldown)
                    {
                        ProcessKey(keyInput);
                    }
                }
                // If we have different input, process the key
                else
                {
                    _lastPress = 0;
                    ProcessKey(keyInput);
                }
            }
        }

        /// <summary>
        /// Process key input
        /// </summary>
        /// <param name="key">Pressed key</param>
        private void ProcessKey(Keys key)
        { 
            // Update the last pressed key
            _lastKeyInput = key;

            // If this is backspace, remove the last character
            if (key == Keys.Back)
                if (_textCount > 0)
                {
                    Text = _text.Remove(_textCount - 1);
                    return;
                }

            // If this is space, insert an empty space
            if (key == Keys.Space)
                Text += ' ';

            // If this key is enter, fire the Apply event
            if (key == Keys.Enter)
                OnApply();

            // Verify validity of the key
            string keyAsString = key.ToString().ToLower();
            if (keyAsString.Count() > 1)
                return; // Invalid

            char letter = keyAsString[0];
            if (!_gameInterface.ValidChars.Contains(letter))
                return; // Invalid

            // At this point, the key press is valid text
            Text += letter;
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this text field
        /// </summary>
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            Color backdropColor = BackdropColor;
            Color textColor = TraceColor;

            if (!_active)
            {
                backdropColor           *= 0.25f;
                textColor               *= 5;
            }
            else if (_editing)
            {
                backdropColor           = TraceColor;
                textColor               = BackdropColor;
            }
            else if (_hovering)
            {
                backdropColor           = TraceColor;
                backdropColor           *= 0.5f;
                textColor               = BackdropColor;
                textColor               *= 0.5f;
            }

            if (base.Draw(gameTime, spriteBatch, rectangle, backdropColor, fadeAmount))
            {
                if (!_active)
                {
                    byte greyAmount     = 100;
                    textColor.R         -= greyAmount;
                    textColor.G         -= greyAmount;
                    textColor.B         -= greyAmount;
                }

                Vector2 stringSize      = InterfaceFont.MeasureString(_text);
                Vector2 textPosition;
                textPosition.X          = Left + 6;
                textPosition.Y          = Bottom - (Height / 2) - (stringSize.Y / 2);

                spriteBatch.DrawString(InterfaceFont, _text, textPosition, textColor);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }
        #endregion

        #region Interaction Code
        /// <summary>
        /// Fires the Apply event
        /// </summary>
        private void OnApply()
        {
            Apply();
        }
        #endregion
    }
}
