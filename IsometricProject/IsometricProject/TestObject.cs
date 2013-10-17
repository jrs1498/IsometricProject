using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class TestObject : GameObject
    {
        public TestObject(Texture2D texture)
            : base(texture.Width, texture.Height)
        {
            AddComponent<GOCDrawable>(texture);
            AddComponent<GOCMovable>();
        }
    }
}
