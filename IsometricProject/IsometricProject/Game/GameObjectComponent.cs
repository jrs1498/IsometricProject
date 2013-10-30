using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsometricProject.Game
{
    public abstract class GameObjectComponent
    {
        #region Attributes
        protected GameObject _parent;
        #endregion

        #region Constructor Code
        /// <summary>
        /// Base stucture for a GameObjectComponent
        /// </summary>
        public GameObjectComponent(GameObject parent)
        {
            _parent = parent;
        }
        #endregion

        #region Update Code
        public virtual void Update(GameTime gameTime)
        { 
            // TODO: Add general update code here
        }
        #endregion

        #region Draw Code
        /// <summary>
        /// Draw this component
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        { 
            // TODO: Add general draw code here
        }
        #endregion
    }
}
