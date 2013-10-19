using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DataTypes;

namespace IsometricProject
{
    public class GameObject
    {
        #region Attributes
        private Rectangle _rectangle;
        private int _elevation;
        private Dictionary<Type, GameObjectComponent> _components;
        #endregion

        #region Properties
        public Vector2 Displacement
        {
            get
            {
                return new Vector2(_rectangle.X, _rectangle.Y);
            }
            set
            {
                _rectangle.X = (int)value.X;
                _rectangle.Y = (int)value.Y;
                OnPositionChange();
            }
        }
        public Vector2 Size
        {
            get
            {
                return new Vector2(_rectangle.Width, _rectangle.Height);
            }
            set
            {
                _rectangle.Width = (int)value.X;
                _rectangle.Height = (int)value.Y;
                OnSizeChange();
            }
        }

        public int X
        {
            get { return _rectangle.X; }
        }
        public int Y
        {
            get { return _rectangle.Y; }
        }
        public int Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;
            }
        }
        #endregion

        #region Events
        protected virtual void OnPositionChange()
        { 
            // TODO: Add position change logic here
        }
        protected virtual void OnSizeChange()
        { 
            // TODO: Add size change logic here
        }
        #endregion

        #region Constructor Code
        /// <summary>
        /// Create a GameObject, which is the main type for all game world objects
        /// Size: 0x0, Position: (0,0)
        /// </summary>
        public GameObject()
        {
            Construct();
        }

        /// <summary>
        /// Create a GameObject, which is the main type for all game world objects
        /// Size: specified, Position: (0,0)
        /// </summary>
        public GameObject(int width, int height)
        {
            Size = new Vector2(width, height);

            Construct();
        }

        /// <summary>
        /// Create a GameObject, which is the main type for all game world objects
        /// Size: specified, Position: specified
        /// </summary>
        public GameObject(int width, int height, int x, int y)
        {
            Size = new Vector2(width, height);
            Displacement = new Vector2(x, y);

            Construct();
        }

        /// <summary>
        /// Primary constructor code
        /// </summary>
        private void Construct()
        {
            _components = new Dictionary<Type, GameObjectComponent>();
        }
        #endregion

        #region Update Code
        /// <summary>
        /// Update this GameObject
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Update all components making up this GameObject
            foreach (KeyValuePair<Type, GameObjectComponent> entry in _components)
                entry.Value.Update(gameTime);
        }
        #endregion

        #region Component Code
        /// <summary>
        /// Gets a specified component from this GameObject
        /// </summary>
        /// <typeparam name="T">Type of GameObjectComponent</typeparam>
        /// <returns>Null if the component doesn't exist</returns>
        public T GetComponent<T>()
            where T : GameObjectComponent
        {
            if (_components.ContainsKey(typeof(T)))
                return (T)_components[typeof(T)];

            return null;
        }

        /// <summary>
        /// Add a GameObjectComponent to this GameObject
        /// </summary>
        protected void AddComponent<T>(Texture2D texture = null)
            where T : GameObjectComponent
        {
            Type componentType = typeof(T);

            // Add a drawable component
            if (componentType == typeof(GOCDrawable))
            {
                GOCDrawable drawableComponent = new GOCDrawable(this, texture);
                _components.Add(typeof(GOCDrawable), drawableComponent);
            }

            // Add a movable component
            else if (componentType == typeof(GOCMovable))
            {
                GOCMovable movableComponent = new GOCMovable(this);
                _components.Add(typeof(GOCMovable), movableComponent);
            }
           
        }
        #endregion
    }
}
