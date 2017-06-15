using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Viewer
{
    public class ViewerManager
    {

        private Dictionary<string, Type> ViewerTypes = new Dictionary<string, Type>();
        private Dictionary<string, IViewer> Viewers = new Dictionary<string, IViewer>();

        public ViewerManager()
        {
            LoadViewers();
        }

        private void LoadViewers()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(p => typeof(IViewer).IsAssignableFrom(p)).Where(c => c.GetCustomAttributes(typeof(ViewerInfo), true).Length > 0);//TODO load from project references as well

            foreach(var type in types)
            {
                var attr = type.GetCustomAttributes(typeof(ViewerInfo), true).FirstOrDefault();
                if (attr != null)
                {
                    ViewerTypes.Add(((ViewerInfo)attr).Extension, type);
                }
            }
        }

        public Control GetViewer(ContentFile item)
        {
            if (Viewers.TryGetValue(Path.GetExtension(item.FilePath), out IViewer viewer))
                return viewer.GetViewer(item);
            else if(ViewerTypes.TryGetValue(Path.GetExtension(item.FilePath), out Type type))
            {
                var view = (IViewer)Activator.CreateInstance(type);
                Viewers.Add(Path.GetExtension(item.FilePath), view);
                return view.GetViewer(item);
            }
            return null;
        }


    }
}
