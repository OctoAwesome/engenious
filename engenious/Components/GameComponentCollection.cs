using System.Collections;
using System.Collections.Generic;

namespace engenious
{
    public sealed class GameComponentCollection : ICollection<GameComponent>
    {
        private readonly UpdateComparer _updateComparer;
        private readonly DrawComparer _drawComparer;
        private readonly List<GameComponent> _components;

        public GameComponentCollection()
        {
            Drawables = new List<IDrawable>();
            Updatables = new List<IUpdateable>();
            _components = new List<GameComponent>();
            _updateComparer = new UpdateComparer();
            _drawComparer = new DrawComparer();
        }

        internal List<IUpdateable> Updatables { get; }

        internal List<IDrawable> Drawables { get; }


        internal void Sort()
        {
            Updatables.Sort(_updateComparer);
            Drawables.Sort(_drawComparer);
        }

        private class UpdateComparer : IComparer<IUpdateable>
        {
            public int Compare(IUpdateable x,IUpdateable y)
            {
                if (x == null || y == null)
                    return 0;
                if (x.Enabled && y.Enabled)
                    return x.UpdateOrder.CompareTo(y.UpdateOrder);
                return -x.Enabled.CompareTo(y.Enabled);
            }
        }
        private class DrawComparer : IComparer<IDrawable>
        {
            public int Compare(IDrawable x,IDrawable y)
            {
                if (x == null || y == null)
                    return 0;
                if (x.Visible && y.Visible)
                    return x.DrawOrder.CompareTo(y.DrawOrder);
                return -x.Visible.CompareTo(y.Visible);
            }
        }

        #region ICollection implementation

        public void Add(GameComponent item)
        {
            var drawable = item as IDrawable;
            if (drawable != null)
                Drawables.Add(drawable);
            IUpdateable updateable = item;
            if (updateable != null)
                Updatables.Add(updateable);
            _components.Add(item);
        }

        public void Clear()
        {
            Drawables.Clear();
            Updatables.Clear();
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

            var drawable = item as IDrawable;
            if (drawable != null)
                Drawables.Remove(drawable);
            IUpdateable updateable = item;
            if (updateable != null)
                Updatables.Remove(updateable);
            
            return _components.Remove(item);
        }

        public int Count => _components.Count;

        public bool IsReadOnly => false;

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

