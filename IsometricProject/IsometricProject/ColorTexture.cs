using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject
{
    public class ColorTexture
    {
        public static Texture2D Create(GraphicsDevice graphicsDevice, Color color)
        {
            return Create(graphicsDevice, 1, 1, color);
        }

        public static Texture2D Create(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height, true, SurfaceFormat.Color);

            Color[] colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
                colors[i] = new Color(color.ToVector3());

            texture.SetData(colors);

            return texture;
        }
    }
}
