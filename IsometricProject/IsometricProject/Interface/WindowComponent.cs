using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IsometricProject.Interface
{
    public class GI_WindowComponent : GI_Obj
    {
        #region Attributes
        private GI_Window _window;
        #endregion

        #region Properties
        #endregion

        #region Constructor Code
        public GI_WindowComponent(GameInterface gameInterface, Texture2D texture, int width, int height, int x, int y, GI_Window window)
            : base(gameInterface, texture, width, height, x, y)
        {
            _window = window;
        }
        #endregion
    }
}
