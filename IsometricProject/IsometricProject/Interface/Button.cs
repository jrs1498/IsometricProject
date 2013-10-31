using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsometricProject.Interface
{
    public class GI_Button : GI_Obj
    {
        #region Attributes
        protected string _title;
        #endregion

        #region Properties
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        #endregion

        #region Constructor Code
        public GI_Button(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, string title)
            : base(gameInterface, texture, width, height, x, y)
        {
            _title = title;
        }
        #endregion

        #region Draw Code
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle? rectangle, float fadeAmount = 1.0f)
        {
            // Only draw this button if it's currently visible
            if (_visible)
            {
                // ---------- Rectangle for drawing ----------
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
                    textColor = TraceColor;
                    backdropColor = BackdropColor;
                }
                else
                {
                    textColor = BackdropColor;
                    backdropColor = TraceColor;
                }
                textColor *= fadeAmount;
                backdropColor *= fadeAmount;

                // ---------- Draw backdrop ----------
                if (_hovering)
                {
                    spriteBatch.Draw(_texture, drawRect, backdropColor);
                }

                // ---------- Draw title ----------
                Vector2 textSize = InterfaceFont.MeasureString(_title);
                Point center = drawRect.Center;
                Vector2 textPosition;
                textPosition.X = center.X - (textSize.X / 2);
                textPosition.Y = center.Y - (textSize.Y / 2);
                spriteBatch.DrawString(InterfaceFont, _title, textPosition, textColor);

                // This drew, so return true
                return true;
            }

            // This did not draw, so return false
            return false;
        }
        #endregion

    }


    public class GI_ContentButton : GI_Button
    {
        #region Attributes
        private CL_ObjType _objType;
        #endregion

        #region Properties
        #endregion

        public GI_ContentButton(GameInterface gameInterface, int width, int height, int x, int y, CL_ObjType objType)
            : base(gameInterface, objType.Texture, width, height, x, y, objType.Texture.Name)
        { 
        }
    }
}
