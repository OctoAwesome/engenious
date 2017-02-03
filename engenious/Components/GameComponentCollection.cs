using System.Collections;
using System.Collections.Generic;

namespace engenious
{
    public sealed class GameComponentCollection : ICollection<GameComponent>
    {
        private readonly UpdateComparer _updateComparer;
        private readonly DrawComparer _drawComparer;
        private readonly List<IDrawable> _drawables;
        private readonly List<IUpdateable> _updateables;
        private readonly List<GameComponent> _components;

        public GameComponentCollection()
        {
            _drawables = new List<IDrawable>();
            _updateables = new List<IUpdateable>();
            _components = new List<GameComponent>();
            _updateComparer = new UpdateComparer();
            _drawComparer = new DrawComparer();
        }

        internal List<IUpdateable> Updatables => _updateables;

        internal List<IDrawable> Drawables => _drawables;


        internal void Sort()
        {
            _updateables.Sort(_updateComparer);
            _drawables.Sort(_drawComparer);
        }

        private class UpdateComparer : IComparer<IUpdateable>
        {
            public int Compare(IUpdateable x,IUpdateable y)
            {
                if (x.Enabled && y.Enabled)
                    return x.UpdateOrder.CompareTo(y.UpdateOrder);
                return -x.Enabled.CompareTo(y.Enabled);
            }
        }
        private class DrawComparer : IComparer<IDrawable>
        {
            public int Compare(IDrawable x,IDrawable y)
            {
                if (x.Visible && y.Visible)
                    return x.DrawOrder.CompareTo(y.DrawOrder);
                return -x.Visible.CompareTo(y.Visible);
            }
        }

        #region ICollection implementation

        public void Add(GameComponent item)
        {
            IDrawable drawable = item as IDrawable;
            if (drawable != null)
                _drawables.Add(drawable);
            IUpdateable updateable = item;
            if (updateable != null)
                _updateables.Add(updateable);
            _components.Add(item);
        }

        public void Clear()
        {
            _drawables.Clear();
            _updateables.Clear();
            _components.Clear();
        }

        public bool Contains(GameComponent item)
        {
            return _components.Contains(item);
        }

        public void CopyTo(GameComponent[] array, int arrayIndex)
        {
            _components.CopyTo(array, arrayIndex);
        }

        public bool Remove(GameComponent item)
        {

            IDrawable drawable = item as IDrawable;
            if (drawable != null)
                _drawables.Remove(drawable);
            IUpdateable updateable = item;
            if (updateable != null)
                _updateables.Remove(updateable);
            
            return _components.Remove(item);
        }

        public int Count
        {
            get
            {
                return _components.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<GameComponent> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        #endregion
    }
}

