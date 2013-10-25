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
    /// <summary>
    /// Packages GameLevel data
    /// </summary>
    public class GameLevelData
    {
        #region Members
        public GameLayerData[] GameLayerData;
        public GameLayerTiledData MainLayerData;
        #endregion

        #region Constructor Code
        public GameLevelData() { }
        public GameLevelData(int gameLayersCount)
        {
            GameLayerData = new GameLayerData[gameLayersCount];
        }
        #endregion
    }

    public class GameLayerData
    { 
        
    }

    public class GameLayerTiledData : GameLayerData
    {
        public TileSystemData TileSystemData;
    }

    public class CL_Data
    {
        public short ReferenceID;

        public CL_Data(short referenceID = 0)
        {
            ReferenceID = referenceID;
        }
    }

    public class CL_ObjTypeData : CL_Data
    {
        public string TextureName;

        public CL_ObjTypeData(short referenceID = 0, string textureName = "")
            : base(referenceID)
        {
            TextureName = textureName;
        }
    }
}
