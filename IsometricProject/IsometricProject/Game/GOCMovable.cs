using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IsometricProject.Game
{
    public class GOCMovable : GameObjectComponent
    {
        #region Attributes
        private Vector3 _acceleration;
        private Vector3 _velocity;
        #endregion

        #region Properties
        private Vector3 Acceleration
        {
            get { return _acceleration; }
            set
            {
                _acceleration = value;
            }
        }
        public Vector3 Velocity
        {
            get { return _velocity; }
            set
            {
                _velocity = value;
            }
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Provides movability to a GameObject
        /// </summary>
        public GOCMovable(GameObject parent)
            : base(parent)
        {
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Causes the parent GameObject to move according to this components velocity
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            _velocity += _acceleration;

            if (_velocity != Vector3.Zero)
                _parent.Displacement += _velocity;
        }
        #endregion

        #region Accessibility
        public void Move(Vector3 amount)
        {
            _parent.Displacement += amount;
        }
        #endregion
    }
}
