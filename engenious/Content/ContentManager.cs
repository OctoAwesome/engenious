using System;
using System.Collections.Generic;
using System.IO;
using engenious.Graphics;
using System.Reflection;
using engenious.Content.Serialization;
using System.Linq;

namespace engenious.Content
{
    public class ContentManager
    {
        private Dictionary<string ,IContentTypeReader> typeReaders;
        private Dictionary<string ,IContentTypeReader> typeReadersOutput;
        private Dictionary<string,object> assets;
        private System.Runtime.Serialization.IFormatter formatter;
        internal GraphicsDevice graphicsDevice;

        public ContentManager(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Content"))
        {
			
        }

        public ContentManager(GraphicsDevice graphicsDevice, string rootDirectory)
        {
            this.RootDirectory = rootDirectory;
            this.graphicsDevice = graphicsDevice;
            typeReaders = new Dictionary<string, IContentTypeReader>();
            typeReadersOutput = new Dictionary<string, IContentTypeReader>();
            assets = new Dictionary<string, object>();
            formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            AddAssembly(Assembly.GetExecutingAssembly());
        }

        internal void AddAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {

                if (t.GetInterfaces().Contains(typeof(IContentTypeReader)) && !(t.IsInterface || t.IsAbstract))
                {
                    var attr = (ContentTypeReaderAttribute)t.GetCustomAttributes(typeof(ContentTypeReaderAttribute), true).FirstOrDefault();
                    if (attr != null){
                        IContentTypeReader reader = Activator.CreateInstance(t) as IContentTypeReader;
                        typeReaders.Add(t.FullName, reader);
                        if (attr.OutputType != null)
                            typeReadersOutput.Add(attr.OutputType.FullName,reader);
                    }
                }
            }
        }
        internal IContentTypeReader GetReaderByOutput(string outputType)
        {
            IContentTypeReader res;
            if (!typeReadersOutput.TryGetValue(outputType, out res))
                return null;
            return res;
        }
        internal IContentTypeReader GetReader(string reader)
        {
            IContentTypeReader res;
            if (!typeReaders.TryGetValue(reader, out res))
                return null;
            return res;
        }

        public string RootDirectory{ get; set; }

        public void Unload(string assetName)
        {
            object asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                var disp = asset as IDisposable;
                disp?.Dispose();
            }
        }
        public void Unload<T>(string assetName) where T : IDisposable
        {
            object asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                if (asset is T)
                {
                    var disp = (T) asset;
                    disp.Dispose();
                }
            }
        }
        public T Load<T>(string assetName) where T : IDisposable
        {
            object asset;
            bool containsName =false;
            if (assets.TryGetValue(assetName, out asset))
            {
                containsName = true;
                if (asset is T)
                {
                    return (T)asset;
                }
            }
            T tmp = ReadAsset<T>(assetName);
            if (tmp != null)
            {
                if (!containsName)
                    assets.Add(assetName, tmp);
            }
            return tmp;
        }
        public IEnumerable<string> ListContent(string path,bool recursive=false)
        {
            path = System.IO.Path.Combine(RootDirectory, path);
            return System.IO.Directory.GetFiles(path,"*.ego",recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
        public IEnumerable<string> ListContent<T>(string path,bool recursive=false)//rather slow(needs to load part of files)
        {
            var tp = GetReaderByOutput(typeof(T).FullName);
            foreach(var file in ListContent(path,false)){
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    ContentFile res = formatter.Deserialize(fs) as ContentFile;
                    if (res == null)
                        continue;
                    Console.WriteLine(res.FileType);
                    if (res.FileType == tp.GetType().FullName)//TODO inheritance
                        yield return file;
                }
            }
            yield break;
        }
        protected T ReadAsset<T>(string assetName)
        {
            try
            {
                using (FileStream fs = new FileStream(System.IO.Path.Combine(RootDirectory, assetName + ".ego"), FileMode.Open, FileAccess.Read))
                {
                    ContentFile res = formatter.Deserialize(fs) as ContentFile;
                    if (res == null)
                        throw new Exception("Could not load non content file");
                    return (T)res.Load(this, fs,typeof(T));
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
            //return default(T);
        }
    }
}

