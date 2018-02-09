using System;

namespace engenious
{
    public abstract class GameComponent : IGameComponent,IUpdateable,IDisposable
    {
        protected GameComponent(Game game)
        {
            Game = game;
            Enabled = true;
        }

        #region IUpdateable implementation

        public virtual void Update(GameTime gameTime)
        {
        }

        public int UpdateOrder
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public Game Game { get; private set; }

        #endregion


        #region IGameComponent implementation

        protected virtual void LoadContent()
        {
        }

        protected virtual void UnloadContent()
        {
        }

        internal void Load()
        {
            LoadContent();
        }

        internal void Unload()
        {
            UnloadContent();
        }

        public virtual void Initialize()
        {
        }

        #endregion

        #region IDisposable implementation

        public virtual void Dispose()
        {
        }

        #endregion
    }
}

