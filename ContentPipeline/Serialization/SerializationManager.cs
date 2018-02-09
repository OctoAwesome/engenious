using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace engenious.Content.Serialization
{
    public class SerializationManager
    {
        private static SerializationManager _instance;

        public static SerializationManager Instance => _instance ?? (_instance = new SerializationManager());


        //private Dictionary<string ,IContentTypeReader> typeReaders;
        private readonly Dictionary<string ,IContentTypeWriter> _typeWriters;

        protected SerializationManager()
        {
            //typeReaders = new Dictionary<string, IContentTypeReader> ();
            _typeWriters = new Dictionary<string, IContentTypeWriter>();
            AddAssembly(Assembly.GetExecutingAssembly());
        }

        public void AddAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                /*if (t.GetInterfaces ().Contains (typeof(IContentTypeReader)) && t.GetCustomAttribute<ContentTypeReaderAttribute> () != null) {
					IContentTypeReader reader = Activator.CreateInstance (t) as IContentTypeReader;
					typeReaders.Add (t.Namespace + "." + t.Name, reader);
				} else*/
                if (t.GetInterfaces().Contains(typeof(IContentTypeWriter)) && t.GetCustomAttributes(typeof(ContentTypeWriterAttribute), true).FirstOrDefault() != null)
                {
                    IContentTypeWriter writer = Activator.CreateInstance(t) as IContentTypeWriter;
                    if (writer != null)
                        _typeWriters.Add(writer.RuntimeType.Namespace + "." + writer.RuntimeType.Name, writer);
                }
            }
        }


        /*public IContentTypeReader GetReader (string reader)
		{
			IContentTypeReader res;
			if (!typeReaders.TryGetValue (reader, out res))
				return null;
			return res;
		}*/

        public IContentTypeWriter GetWriter(Type writerType)
        {
            IContentTypeWriter res;
            if (!_typeWriters.TryGetValue(writerType.FullName, out res))
                return null;
            return res;
		
        }
    }
}