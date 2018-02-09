using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ContentTool.Models.History
{
    public class HistoryPropertyChange : IHistoryItem
    {
        private readonly object _oldValue,_newValue;

        private readonly Action<object> _setter;
        
        public HistoryPropertyChange(object reference,string propertyName,object oldValue,object newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;

            var prop = reference.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var mi = prop?.GetSetMethod();
            if (mi == null)
                return;

            var p = Expression.Parameter(typeof(object));
            var call = Expression.Call(Expression.Constant(reference), mi,Expression.Convert(p,prop.PropertyType));
            _setter = Expression.Lambda<Action<object>>(call,p).Compile();
        }
        
        public void Undo()
        {
            _setter(_oldValue);
        }

        public void Redo()
        {
            _setter(_newValue);
        }
    }
}