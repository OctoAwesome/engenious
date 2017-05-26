using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContentTool.old
{
    
    public class ObservableList<T> :  INotifyCollectionChanged, IList<T>, INotifyPropertyChanged
    {
        private readonly List<T> _list;

        public ObservableList()
        {
            _list = new List<T>();
        }

        public ObservableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }



        private void RemoveChangedEvents(T item)
        {
            RemoveCollectionChanged(item as INotifyCollectionChanged);
            RemovePropertyChanged(item as INotifyPropertyChanged);
        }

        private void AddChangedEvents(T item)
        {
            AddCollectionChanged(item as INotifyCollectionChanged);
            AddPropertyChanged(item as INotifyPropertyChanged);
        }

        private void RemovePropertyChanged(INotifyPropertyChanged item)
        {
            if (item == null)
                return;

            item.PropertyChanged -= Item_PropertyChanged;
        }

        private void AddPropertyChanged(INotifyPropertyChanged item)
        {
            if (item == null)
                return;

            item.PropertyChanged += Item_PropertyChanged;
        }

        private void AddCollectionChanged(INotifyCollectionChanged item)
        {
            if (item == null)
                return;

            item.CollectionChanged += Item_CollectionChanged;
        }

        private void RemoveCollectionChanged(INotifyCollectionChanged item)
        {
            if (item == null)
                return;

            item.CollectionChanged -= Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        #region IList implementation

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }



        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            AddChangedEvents(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void RemoveAt(int index)
        {
            T old = _list[index];
            _list.RemoveAt(index);
            AddChangedEvents(old);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old));
        }

        public T this [int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                T old = _list[index];
                _list[index] = value;
                RemoveChangedEvents(old);
                AddChangedEvents(value);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old));
            }
        }

        #endregion

        #region ICollection implementation

        public void Add(T item)
        {
            _list.Add(item);
            AddChangedEvents(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _list.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool removed = _list.Remove(item);
            AddChangedEvents(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return removed;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion



        #region INotifyCollectionChanged implementation

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

