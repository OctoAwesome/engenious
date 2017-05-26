using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContentTool.old.Items
{
    public class ContentItemCollection : INotifyCollectionChanged, IList<ContentItem>, INotifyPropertyChanged
    {
        readonly ObservableList<ContentItem> _contents = new ObservableList<ContentItem>();

        public ContentItemCollection()
        {
            _contents.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(sender, args);
            _contents.CollectionChanged += (sender, args) => CollectionChanged?.Invoke(sender, args);
        }

        public ContentItem this[int index]
        {
            get { return _contents[index]; }

            set { _contents[index] = value; }
        }

        public int Count => _contents.Count;

        public bool IsReadOnly => _contents.IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Add(ContentItem item)
        {
            if (!_contents.Contains(item))
                _contents.Add(item);
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public bool Contains(ContentItem item)
        {
            return _contents.Contains(item);
        }

        public void CopyTo(ContentItem[] array, int arrayIndex)
        {
            _contents.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ContentItem> GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        public int IndexOf(ContentItem item)
        {
            return _contents.IndexOf(item);
        }

        public void Insert(int index, ContentItem item)
        {
            if (!_contents.Contains(item))
                _contents.Insert(index,item);
        }

        public bool Remove(ContentItem item)
        {
            return _contents.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _contents.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
