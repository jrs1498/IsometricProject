using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IsometricProject
{
    public class GOCMovable : GameObjectComponent
    {
        #region Attributes
        private Vector2 _acceleration;
        private Vector2 _velocity;
        #endregion

        #region Properties
        private Vector2 Acceleration
        {
            get { return _acceleration; }
            set
            {
                _acceleration = value;
            }
        }
        public Vector2 Velocity
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

            if (_velocity != Vector2.Zero)
                _parent.Displacement += _velocity;
        }
        #endregion

        #region Accessibility
        public void Move(Vector2 amount)
        {
            _parent.Displacement += amount;
        }
        #endregion
    }
}
