using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class TileSystemData
    {
        public TileReferencerData[] TileReferencers;
        public int NumRows;
        public int NumCols;

        public TileSystemData() { }
        public TileSystemData(int numRows, int numCols)
        {
            TileReferencers = new TileReferencerData[numRows * numCols];
            NumRows = numRows;
            NumCols = numCols;
        }
    }

    public class TileTypeData
    {
        public byte TileReferenceCode;
        public string TextureFileName;
        public byte TileFlags;

        public TileTypeData() { }
        public TileTypeData(byte tileReferenceCode, string textureFileName, byte tileFlags)
        {
            TileReferenceCode = tileReferenceCode;
            TextureFileName = textureFileName;
            TileFlags = tileFlags;
        }
    }

    public class TileReferencerData
    {
        public byte TileReferenceCode;
        public byte Elevation;

        public TileReferencerData() { }
        public TileReferencerData(byte tileReferenceCode, byte elevation)
        {
            TileReferenceCode = tileReferenceCode;
            Elevation = elevation;
        }
    }
}
