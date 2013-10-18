using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class Tile : GameObject
    {
        public Tile(Texture2D texture)
        {
            AddComponent<GOCDrawable>(texture);
        }
    }
}
