using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Threading;
using ContentTool.Items;
using engenious.Content.Pipeline;
using System.ComponentModel;

namespace ContentTool.Builder
{
    public class ContentBuilder
    {

        private const string DESTINATION_EXT = ".ego";

        public delegate void ItemProgressDelegate(object sender, ItemProgressEventArgs e);
        public delegate void BuildStatusChangedDelegate(object sender, BuildStep buildStep);

        public event engenious.Content.Pipeline.BuildMessageDel BuildMessage;
        public event ItemProgressDelegate ItemProgress;
        public event BuildStatusChangedDelegate BuildStatusChanged;

        private Dictionary<string,ContentFile> builtFiles;
        private BuildStep currentBuild = BuildStep.Finished;

        private System.Threading.Thread buildingThread;
        private BuildCache cache;

        private ContentProject project;

        public SynchronizationContext UiContext;
        public ContentProject Project {
            get{return project;}
            private set{
                project = value;
                if (project == null)
                    cache = null;
                else
                    cache = BuildCache.Load(GetCacheFile());
            }
        }

        public bool IsBuilding { get; private set; }

        public bool CanClean
        {
            get
            {
                return cache.CanClean(GetOutputDir());
            }
        }
        public bool CanBuild
        {
            get
            {
                return true;//TODO:
            }
        }

        public int FailedBuilds { get; private set; }

        public ContentBuilder(ContentProject project)
        {
            UiContext = SynchronizationContext.Current;
            Project = project;
            builtFiles = new Dictionary<string, ContentFile>();
        }

        #region File Helper

        internal static void CreateFolderIfNeeded(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;
            string folder = System.IO.Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(folder))
                return;
            if (!System.IO.Directory.Exists(folder))
            {
                CreateFolderIfNeeded(folder);
                System.IO.Directory.CreateDirectory(folder);
            }
        }

        private string GetDestinationFile(ContentFile contentFile)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(contentFile.getPath()), System.IO.Path.GetFileNameWithoutExtension(contentFile.Name) + DESTINATION_EXT);
        }

        private string GetDestinationFileAbsolute(ContentFile contentFile)
        {
            //string importFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Project.File), contentFile.getPath());
            return System.IO.Path.Combine(GetOutputDir(),GetDestinationFile(contentFile));
        }

        private string GetOutputDir()
        {
            string relOut = string.Format(Project.OutputDir.Replace("{Configuration}", "{0}"), Project.Configuration.ToString());
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Project.File), relOut);
        }

        private string GetObjectDir()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Project.File), "obj");
        }

        private string GetCacheFile()
        {
            return Path.Combine(GetObjectDir(),"obj",Path.GetFileNameWithoutExtension(Project.File)+".dat");
        }

        #endregion

        private void RaiseBuildMessage(object sender,engenious.Content.Pipeline.BuildMessageEventArgs e)
        {
            if (e.MessageType == engenious.Content.Pipeline.BuildMessageEventArgs.BuildMessageType.Error)
                FailedBuilds++;
            BuildMessage?.Invoke(sender, e);
        }


        /// <summary>
        /// Builds a File based on the given File, ImporterContext and Processor Context
        /// </summary>
        /// <param name="contentFile"></param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        public Tuple<object,object> BuildFile(ContentFile contentFile,ContentImporterContext importerContext,ContentProcessorContext processorContext)
        {
            string importDir = System.IO.Path.GetDirectoryName(Project.File);
            string importFile = System.IO.Path.Combine(importDir, contentFile.getPath());
            string destFile = GetDestinationFileAbsolute(contentFile);
            string outputPath = GetOutputDir();

            CreateFolderIfNeeded(destFile);

            if (!cache.NeedsRebuild(importDir,outputPath,contentFile.getPath())){
                RaiseBuildMessage(this, new BuildMessageEventArgs(contentFile.Name, "skipped!", BuildMessageEventArgs.BuildMessageType.Information));
                return null;
            }
            BuildInfo cacheInfo = new BuildInfo(importDir,contentFile.getPath(),GetDestinationFile(contentFile));
            var importer = contentFile.Importer;
            if (importer == null)
                return null;
            
            object importerOutput = importer.Import(importFile, importerContext);
            if (importerOutput == null)
                return null;
            
            cacheInfo.Dependencies.AddRange(importerContext.Dependencies);
            cache.AddDependencies(importDir,importerContext.Dependencies);

            engenious.Content.Pipeline.IContentProcessor processor = contentFile.Processor;
            if (processor == null)
                return new Tuple<object,object>(importerOutput,null);

            object processedData = processor.Process(importerOutput,importFile, processorContext);

            if (processedData == null)
                return new Tuple<object, object>(importerOutput, null);
            cacheInfo.Dependencies.AddRange(processorContext.Dependencies);
            cache.AddDependencies(importDir,processorContext.Dependencies);

            engenious.Content.Serialization.IContentTypeWriter typeWriter = engenious.Content.Serialization.SerializationManager.Instance.GetWriter(processedData.GetType());
            engenious.Content.ContentFile outputFileWriter = new engenious.Content.ContentFile(typeWriter.RuntimeReaderName);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(fs, outputFileWriter);
                engenious.Content.Serialization.ContentWriter writer = new engenious.Content.Serialization.ContentWriter(fs);
                writer.WriteObject(processedData, typeWriter);
            }
            cacheInfo.BuildDone(outputPath);
            cache.AddBuildInfo(cacheInfo);
            builtFiles[contentFile.getPath()]=contentFile;
            ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Build, contentFile), null, null);

            return new Tuple<object, object>(importerOutput,processedData);
        }

        /// <summary>
        /// Builds a Directory based on the given folder, importerContext and processorContext
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        private void BuildDir(ContentFolder folder,engenious.Content.Pipeline.ContentImporterContext importerContext,engenious.Content.Pipeline.ContentProcessorContext processorContext)
        {
            foreach (var item in folder.Contents)
            {
                if (item is ContentFile)
                {
                    BuildFile(item as ContentFile,importerContext,processorContext);
                }
                else if (item is ContentFolder)
                {
                    BuildDir(item as ContentFolder,importerContext,processorContext);
                }
            }
        }

        public void Build(ContentItem item = null)
        {
            if (Project == null)
                return;

            IsBuilding = true;
            currentBuild = BuildStep.Build;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build, null, null);

            if(item == null)
                buildingThread = new System.Threading.Thread(new System.Threading.ThreadStart(BuildThread));
            else
                buildingThread = new System.Threading.Thread(new ThreadStart(()=>BuildThread(item)));
            
            buildingThread.SetApartmentState(ApartmentState.STA);
            buildingThread.Start();
        }

        public void Rebuild()
        {
            if (Project == null)
                return;
            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate ()
            {
                Clean();
                buildingThread.Join();
                Build();
            }));
            thread.Start();
        }
        public void Clean()
        {
            if (Project == null)
                return;
            
            currentBuild = BuildStep.Clean;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Clean, null, null);

            buildingThread = new System.Threading.Thread(new System.Threading.ThreadStart(CleanThread));
            buildingThread.Start();

        }
        public void Abort()
        {
            if (!IsBuilding)
                return;
            buildingThread.Abort();//TODO: better solution?
            IsBuilding = false;
            currentBuild = BuildStep.Abort;
            BuildStatusChanged?.Invoke(this, currentBuild | BuildStep.Abort);
        }

        public void Join()
        {
            buildingThread.Join();
        }

        #region Thread Methods
        private void CleanThread()
        {
            IsBuilding = true;
            foreach (var cachedItem in cache.Files)
            {
                var item = Project.GetElement(cachedItem.Value.InputFile) as ContentFile;
                if (item != null)
                {
                    ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Clean, item), null, null);
                    if (System.IO.File.Exists(GetDestinationFileAbsolute(item)))
                        System.IO.File.Delete(GetDestinationFileAbsolute(item));
                }
            }
            IsBuilding = false;
            BuildStatusChanged?.Invoke(this, BuildStep.Clean | BuildStep.Finished);
        }

        private void BuildThread()
        {
            BuildThread(Project);
        }

        private void BuildThread(ContentItem item)
        {
            FailedBuilds = 0;

            IsBuilding = true;
            string outputDir = GetOutputDir();
            CreateFolderIfNeeded(outputDir);
            PipelineHelper.PreBuilt(Project);
            using (engenious.Content.Pipeline.ContentImporterContext importerContext = new engenious.Content.Pipeline.ContentImporterContext())
            using (engenious.Content.Pipeline.ContentProcessorContext processorContext = new engenious.Content.Pipeline.ContentProcessorContext(UiContext))
            {
                importerContext.BuildMessage += RaiseBuildMessage;
                processorContext.BuildMessage += RaiseBuildMessage;

                if(item == null)
                    BuildDir(Project, importerContext, processorContext);
                else if(item is ContentFolder)
                    BuildDir((ContentFolder)item,importerContext,processorContext);
                else if (item is ContentFile)
                    BuildFile((ContentFile) item, importerContext, processorContext);
            }
            //System.Threading.Thread.Sleep(8000);
            cache.Save(GetCacheFile());
            IsBuilding = false;

            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build | BuildStep.Finished, null, null);
        }
        #endregion
    }
}

