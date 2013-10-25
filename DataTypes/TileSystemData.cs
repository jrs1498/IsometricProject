using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class TileSystemData
    {
        public TileRefData[] Tiles;
        public int NumRows;
        public int NumCols;

        public TileSystemData() { }
        public TileSystemData(int numRows, int numCols)
        {
            Tiles = new TileRefData[numRows * numCols];
            NumRows = numRows;
            NumCols = numCols;
        }
    }

    public class TileTypeData : CL_ObjTypeData
    {
        public byte TileFlags;

        public TileTypeData() { }
        public TileTypeData(short referenceID = 0, string textureName = "", byte tileFlags = 0)
            : base(referenceID, textureName)
        {
            TileFlags = tileFlags;
        }
    }

    public class TileRefData : CL_Data
    {
        public byte Elevation;

        public TileRefData() { }
        public TileRefData(short referenceID = 0, byte elevation = 0)
            : base(referenceID)
        {
            Elevation = elevation;
        }
    }
}
