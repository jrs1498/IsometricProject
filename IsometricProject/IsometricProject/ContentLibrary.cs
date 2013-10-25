using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DataTypes;

namespace IsometricProject
{
    /// <summary>
    /// This class handles game content
    /// </summary>
    public class ContentLibrary
    {
        #region Attributes
        private ContentManager _contentManager;
        private Dictionary<string, Dictionary<short, CL_ObjType>> _loadedFiles;
        #endregion

        #region Constructor Code
        public ContentLibrary(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _loadedFiles = new Dictionary<string, Dictionary<short, CL_ObjType>>();
        }
        #endregion

        #region Loading & Saving Code
        /// <summary>
        /// Load a file containing data for multiple CL_ObjTypes
        /// and then store the file in this ContentLibrary
        /// </summary>
        /// <typeparam name="DataType">The DataType contained within the file</typeparam>
        /// <typeparam name="ObjType">The CL_ObjType corresponding to the specified DataType</typeparam>
        /// <param name="path">Path to the directory containing the file, including slashes</param>
        /// <param name="filename">Filename, which is also used as the has key for this loaded file</param>
        /// <returns>Indicates success / failure</returns>
        public bool LoadTypesFromFile<DataType, ObjType>(string path, string filename)
            where DataType  : CL_ObjTypeData
            where ObjType   : CL_ObjType
        {
            Dictionary<short, CL_ObjType> loadedTypes = new Dictionary<short, CL_ObjType>();
            DataType[] loadedData = _contentManager.Load<DataType[]>(path + filename);

            foreach (DataType data in loadedData)
            {
                Texture2D texture = _contentManager.Load<Texture2D>("Textures\\" + data.TextureName);
                ObjType loadedType = (ObjType)Activator.CreateInstance(typeof(ObjType), new object[] { data, texture });
                loadedTypes.Add(loadedType.ReferenceID, loadedType);
            }

            _loadedFiles.Add(filename, loadedTypes);

            return true;
        }
        #endregion

        #region Content Retrieval Code
        /// <summary>
        /// Returns the specified dictionary which was previously loaded from a file
        /// </summary>
        /// <param name="filename">The name of the file used to load the dictionary</param>
        /// <returns>Dictionary previously generates by loading a file</returns>
        public Dictionary<short, CL_ObjType> GetLoadedFile(string filename)
        {
            if (_loadedFiles.ContainsKey(filename))
                return _loadedFiles[filename];

            return null;
        }

        /// <summary>
        /// Returns the specified object type which was previous loaded from a file
        /// </summary>
        /// <param name="filename">The name of the file which contained this object when loading</param>
        /// <param name="referenceID">The referenceID of the object type requested</param>
        /// <returns>CL_ObjType previously loaded from a file</returns>
        public CL_ObjType GetLoadedObjType(string filename, short referenceID)
        {
            if (_loadedFiles.ContainsKey(filename))
                if (_loadedFiles[filename].ContainsKey(referenceID))
                    return _loadedFiles[filename][referenceID];

            return null;
        }
        #endregion
    }


    public class CL_Type
    {
        protected short _referenceID;
        public short ReferenceID
        {
            get { return _referenceID; }
        }

        public CL_Type(short referenceID)
        {
            _referenceID = referenceID;
        }
    }


    public class CL_ObjType : CL_Type
    {
        protected Texture2D _texture;
        public Texture2D Texture
        {
            get { return _texture; }
        }

        public CL_ObjType(short referenceID, Texture2D texture)
            : base(referenceID)
        {
            _texture = texture;
        }
    }
}
