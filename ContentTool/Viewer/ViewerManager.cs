using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ContentTool.Models;

namespace ContentTool.Viewer
{
    public class ViewerManager
    {

        private readonly Dictionary<string, Type> _viewerTypes = new Dictionary<string, Type>();
        private readonly Dictionary<string, IViewer> _viewers = new Dictionary<string, IViewer>();

        public ViewerManager()
        {
            LoadViewers();
        }

        private void LoadViewers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(p => typeof(IViewer).IsAssignableFrom(p)).Where(c => c.GetCustomAttributes(typeof(ViewerInfo), true).Length > 0);//TODO load from project references as well

            foreach(var type in types)
            {
                foreach(var attr in type.GetCustomAttributes(typeof(ViewerInfo), true).Where(x=>x != null))
                    _viewerTypes.Add(((ViewerInfo)attr).Extension, type);
            }
        }

        public Control GetViewer(ContentFile item)
        {
            if (_viewers.TryGetValue(Path.GetExtension(item.FilePath), out IViewer viewer))
                return viewer.GetViewer(item);
            if (!_viewerTypes.TryGetValue(Path.GetExtension(item.FilePath), out Type type))
                return null;
            var view = (IViewer)Activator.CreateInstance(type);
            _viewers.Add(Path.GetExtension(item.FilePath), view);
            return view.GetViewer(item);
        }


    }
}
