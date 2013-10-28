using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
        #endregion

    }
}
