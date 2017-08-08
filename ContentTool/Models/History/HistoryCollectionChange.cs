using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms.VisualStyles;

namespace ContentTool.Models.History
{
    public class HistoryCollectionChange<T> : IHistoryItem
    {
        private readonly NotifyCollectionChangedAction _action;
        private readonly System.Collections.IList _oldList,_newList;
        private readonly IList<T> _reference;
        private readonly int _oldStartingIndex, _newStartingIndex;

        private static Dictionary<Type, Func<object, NotifyCollectionChangedAction, System.Collections.IList, System.Collections.IList,int, int, IHistoryItem>> _cachedCreators = new Dictionary<Type, Func<object, NotifyCollectionChangedAction, System.Collections.IList, System.Collections.IList,int, int, IHistoryItem>>();

        public static IHistoryItem CreateInstance(object reference, NotifyCollectionChangedEventArgs args)
        {
            var type = reference.GetType();
            if (_cachedCreators.TryGetValue(type, out var lambda))
            {
                return lambda(reference, args.Action, args.OldItems, args.NewItems, args.OldStartingIndex, args.NewStartingIndex);
            }
            type = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));
        
            if (type == null)
                return null;
           /* while (() == null)
            {
                type = type.BaseType;
                if (type == null)
                    return null;
            }while (type != null)*/
            var itemType = type.GetGenericArguments().First();
            var collectionType = typeof(IList<>).MakeGenericType(itemType);
            
            var historyType = typeof(HistoryCollectionChange<>).MakeGenericType(itemType);
            var ci = historyType.GetConstructor(new Type[]{collectionType,typeof(NotifyCollectionChangedAction),typeof(System.Collections.IList),typeof(System.Collections.IList),typeof(int),typeof(int)});

            var collectionParamObj1 = Expression.Parameter(typeof(object));
            var collectionParamObj2 = Expression.Parameter(typeof(System.Collections.IList));
            var collectionParamObj3 = Expression.Parameter(typeof(System.Collections.IList));
            var collectionParam1 = Expression.Convert(collectionParamObj1,collectionType);
            //var collectionParam2 = Expression.Convert(collectionParamObj2,collectionType);
            //var collectionParam3 = Expression.Convert(collectionParamObj3,collectionType);
            
            var actionParam = Expression.Parameter(typeof(NotifyCollectionChangedAction));
            var oldIndexParam = Expression.Parameter(typeof(int));
            var newIndexParam = Expression.Parameter(typeof(int));
            
            var call = Expression.New(ci, collectionParam1,actionParam,
                collectionParamObj2, collectionParamObj3,oldIndexParam,newIndexParam);

            lambda = Expression
                .Lambda<Func<object, NotifyCollectionChangedAction, System.Collections.IList, System.Collections.IList,int,int, IHistoryItem>>(call,
                        collectionParamObj1,
                        actionParam,
                        collectionParamObj2,
                        collectionParamObj3,
                        oldIndexParam,
                        newIndexParam
                ).Compile();

            _cachedCreators[reference.GetType()] = lambda;

            return lambda(reference, args.Action, args.OldItems, args.NewItems,args.OldStartingIndex,args.NewStartingIndex);
        }
        
        public HistoryCollectionChange(IList<T> reference,NotifyCollectionChangedAction action,System.Collections.IList oldList,System.Collections.IList newList,int oldListStartIndex=-1,int newListStartIndex=-1)
        {
            if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove)
                throw  new NotImplementedException(); //TODO: implement
            
            
            
            
            _action = action;
            _reference = reference;
            _oldList = oldList;
            _newList = newList;
            _oldStartingIndex = oldListStartIndex;
            _newStartingIndex = newListStartIndex;
        }

        public void UndoAdd()
        {
            int index = _newStartingIndex;
            if (index != -1)
                for (int i = index;i<index+_newList.Count;i++)
                    _reference.RemoveAt(i);//TODO: perhaps delete references directly? or check reference
                /*foreach(var e in _newList)
                    if (_reference[index] == e)
                        _reference.RemoveAt(index++);
                    else
                        throw new NotSupportedException("dunno");*/
            else
                foreach (var e in _newList.OfType<T>())
                    _reference.Remove(e);
        }

        public void RedoAdd()
        {
            int index = _newStartingIndex;
            if (index != -1)
                
                foreach(var e in _newList.OfType<T>())
                    _reference.Insert(index++,e);
            else
                foreach (var e in _newList.OfType<T>())
                    _reference.Add(e);
        }

        public void UndoRemove()
        {
            int index = _oldStartingIndex;
            if (index != -1)
                foreach (var e in _oldList.OfType<T>())
                    _reference.Insert(index++,e);
            else
                foreach (var e in _oldList.OfType<T>())
                    _reference.Add(e);
        }

        public void RedoRemove()
        {
            int index = _oldStartingIndex;
            if (index != -1)
                foreach (var e in _oldList.OfType<T>())
                    _reference.RemoveAt(index++);//TODO reference check?
            else
                foreach (var e in _oldList.OfType<T>())
                    _reference.Remove(e);

        }

        public void UndoReplace()
        {
            int index = _newStartingIndex;
            if (index != -1)
                foreach (var e in _oldList.OfType<T>())
                    _reference[index++] = e;//TODO reference check
            else
                for (int i = 0; i < _oldList.Count; i++)
                {
                    int eI = _reference.IndexOf((T)(object)_newList[i]);//TODO wtf?
                    _reference[eI] = (T)(object)_oldList[i];
                }
        }

        public void RedoReplace()
        {
            int index = _newStartingIndex;
            if (index != -1)
                foreach (var e in _newList.OfType<T>())
                    _reference[index++] = e;//TODO reference check
            else
                for (int i = 0; i < _oldList.Count; i++)
                {
                    int eI = _reference.IndexOf((T)(object)_oldList[i]);
                    _reference[eI] = (T)(object)_newList[i];
                }
        }

        public void UndoMove()
        {
            UndoAdd();
            UndoRemove();
        }

        public void RedoMove()
        {
            RedoRemove();
            RedoAdd();
        }
        
        public void Undo()
        {
            switch (_action)
            {
                case NotifyCollectionChangedAction.Add:
                    UndoAdd();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    UndoRemove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UndoReplace();
                    break;
                case NotifyCollectionChangedAction.Move:
                    UndoMove();
                    break;
            }
        }

        public void Redo()
        {
            switch (_action)
            {
                case NotifyCollectionChangedAction.Add:
                    RedoAdd();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RedoRemove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RedoReplace();
                    break;
                case NotifyCollectionChangedAction.Move:
                    RedoMove();
                    break;
            }
        }
    }
}