using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    public class GameObjectData
    {
        public Vector2 position;
    }

    public class GameLayerData
    {
        public Type layerType;
        public GameObjectData[] gameObjects;

        public GameLayerData()
        { 
            
        }

        public GameLayerData(Type gameLayerType, int numObjects)
        {
            layerType = gameLayerType;
            gameObjects = new GameObjectData[numObjects];
        }
    }

    public class GameLevelData
    {
        public GameLayerData[] layers;

        public GameLevelData()
        { 
            
        }

        public GameLevelData(int numLayers)
        {
            layers = new GameLayerData[numLayers];
        }
    }
}
