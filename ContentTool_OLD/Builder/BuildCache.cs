using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ContentTool.old.Builder
{
    [Serializable()]
    public class BuildCache
    {
        public string OutputPath{ get; set; }

        public BuildCache()
        {
            Files = new Dictionary<string,BuildInfo>();
        }

        public Dictionary<string,BuildInfo> Files{ get; private set; }

        public void AddBuildInfo(BuildInfo info)
        {
            Files[info.InputFile] = info;
        }

        public void AddDependencies(string importDir, IEnumerable<string> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (Files.ContainsKey(dependency))
                    Files[dependency].Refresh(importDir);
                else
                    Files.Add(dependency, new BuildInfo(importDir, dependency));
                
            }
        }

        public bool NeedsRebuild(string importPath, string outputPath, string filename)
        {
            BuildInfo val;
            if (Files.TryGetValue(filename, out val))
            {
                if (val.NeedsRebuild(importPath, outputPath))
                    return true;

                foreach (var dependency in val.Dependencies)
                {
                    if (NeedsRebuild(importPath, outputPath, dependency))
                        return true;
                }

                return false;
            }
            return true;
        }

        public bool CanClean(string outputPath)
        {
            foreach (var file in Files)
            {
                if (file.Value.IsBuilt(outputPath))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            Files.Clear();
        }

        public void Save(string file)
        {
            ContentBuilder.CreateFolderIfNeeded(file);
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, this);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static BuildCache Load(string file)
        {
            if (!File.Exists(file))
                return new BuildCache();
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (BuildCache)formatter.Deserialize(fs);
                }
            }
            catch
            {
                // ignored
            }
            return new BuildCache();
        }
    }
}

