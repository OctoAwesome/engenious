using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using ContentTool.old.Items;
using engenious.Content.Pipeline;
using engenious.Content.Serialization;

namespace ContentTool.old.Builder
{
    public class ContentBuilder
    {

        private const string DestinationExt = ".ego";

        public delegate void ItemProgressDelegate(object sender, ItemProgressEventArgs e);
        public delegate void BuildStatusChangedDelegate(object sender, BuildStep buildStep);

        public event BuildMessageDel BuildMessage;
        public event ItemProgressDelegate ItemProgress;
        public event BuildStatusChangedDelegate BuildStatusChanged;

        private readonly Dictionary<string,ContentFile> _builtFiles;
        private BuildStep _currentBuild = BuildStep.Finished;

        private Thread _buildingThread;
        private BuildCache _cache;

        private ContentProject _project;

        public SynchronizationContext UiContext;
        public ContentProject Project {
            get{return _project;}
            private set{
                _project = value;
                if (_project == null)
                    _cache = null;
                else
                    _cache = BuildCache.Load(GetCacheFile());
            }
        }

        public bool IsBuilding { get; private set; }

        public bool CanClean
        {
            get
            {
                return _cache.CanClean(GetOutputDir());
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
            _builtFiles = new Dictionary<string, ContentFile>();
        }

        #region File Helper

        internal static void CreateFolderIfNeeded(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;
            string folder = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(folder))
                return;
            if (!Directory.Exists(folder))
            {
                CreateFolderIfNeeded(folder);
                Directory.CreateDirectory(folder);
            }
        }

        private string GetDestinationFile(ContentFile contentFile)
        {
            return Path.Combine(Path.GetDirectoryName(contentFile.GetPath()), Path.GetFileNameWithoutExtension(contentFile.Name) + DestinationExt);
        }

        private string GetDestinationFileAbsolute(ContentFile contentFile)
        {
            //string importFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Project.File), contentFile.getPath());
            return Path.Combine(GetOutputDir(),GetDestinationFile(contentFile));
        }

        private string GetOutputDir()
        {
            string relOut = string.Format(Project.OutputDir.Replace("{Configuration}", "{0}"), Project.Configuration);
            return Path.Combine(Path.GetDirectoryName(Project.File), relOut);
        }

        private string GetObjectDir()
        {
            return Path.Combine(Path.GetDirectoryName(Project.File), "obj");
        }

        private string GetCacheFile()
        {
            return Path.Combine(GetObjectDir(),"obj",Path.GetFileNameWithoutExtension(Project.File)+".dat");
        }

        #endregion

        private void RaiseBuildMessage(object sender,BuildMessageEventArgs e)
        {
            if (e.MessageType == BuildMessageEventArgs.BuildMessageType.Error)
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
            string importDir = Path.GetDirectoryName(Project.File);
            string importFile = Path.Combine(importDir, contentFile.GetPath());
            string destFile = GetDestinationFileAbsolute(contentFile);
            string outputPath = GetOutputDir();

            if (!File.Exists(importFile))
            {
                RaiseBuildMessage(this,new BuildMessageEventArgs(contentFile.Name,": File not found!",BuildMessageEventArgs.BuildMessageType.Warning));
                return null;
            }

            CreateFolderIfNeeded(destFile);

            if (!_cache.NeedsRebuild(importDir,outputPath,contentFile.GetPath())){
                RaiseBuildMessage(this, new BuildMessageEventArgs(contentFile.Name, "skipped!", BuildMessageEventArgs.BuildMessageType.Information));
                return null;
            }
            BuildInfo cacheInfo = new BuildInfo(importDir,contentFile.GetPath(),GetDestinationFile(contentFile));
            var importer = contentFile.Importer;
            if (importer == null)
                return null;

            object importerOutput = importer.Import(importFile, importerContext);
            if (importerOutput == null)
                return null;

            cacheInfo.Dependencies.AddRange(importerContext.Dependencies);
            _cache.AddDependencies(importDir,importerContext.Dependencies);

            IContentProcessor processor = contentFile.Processor;
            if (processor == null)
                return new Tuple<object,object>(importerOutput,null);

            object processedData = processor.Process(importerOutput,importFile, processorContext);

            if (processedData == null)
                return new Tuple<object, object>(importerOutput, null);
            cacheInfo.Dependencies.AddRange(processorContext.Dependencies);
            _cache.AddDependencies(importDir,processorContext.Dependencies);

            IContentTypeWriter typeWriter = SerializationManager.Instance.GetWriter(processedData.GetType());
            engenious.Content.ContentFile outputFileWriter = new engenious.Content.ContentFile(typeWriter.RuntimeReaderName);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(fs, outputFileWriter);
                ContentWriter writer = new ContentWriter(fs);
                writer.WriteObject(processedData, typeWriter);
            }
            cacheInfo.BuildDone(outputPath);
            _cache.AddBuildInfo(cacheInfo);
            _builtFiles[contentFile.GetPath()]=contentFile;
            ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Build, contentFile), null, null);

            return new Tuple<object, object>(importerOutput,processedData);
        }

        /// <summary>
        /// Builds a Directory based on the given folder, importerContext and processorContext
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        private void BuildDir(ContentFolder folder,ContentImporterContext importerContext,ContentProcessorContext processorContext)
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
            _currentBuild = BuildStep.Build;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build, null, null);

            if(item == null)
                _buildingThread = new Thread(BuildThread);
            else
                _buildingThread = new Thread(()=>BuildThread(item));

            _buildingThread.SetApartmentState(ApartmentState.STA);
            _buildingThread.Start();
        }

        public void Rebuild()
        {
            if (Project == null)
                return;
            var thread = new Thread(new ThreadStart(delegate
            {
                Clean();
                _buildingThread.Join();
                Build();
            }));
            thread.Start();
        }
        public void Clean()
        {
            if (Project == null)
                return;

            _currentBuild = BuildStep.Clean;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Clean, null, null);

            _buildingThread = new Thread(CleanThread);
            _buildingThread.Start();

        }
        public void Abort()
        {
            if (!IsBuilding)
                return;
            _buildingThread.Abort();//TODO: better solution?
            IsBuilding = false;
            _currentBuild = BuildStep.Abort;
            BuildStatusChanged?.Invoke(this, _currentBuild | BuildStep.Abort);
        }

        public void Join()
        {
            _buildingThread.Join();
        }

        #region Thread Methods
        private void CleanThread()
        {
            IsBuilding = true;
            foreach (var cachedItem in _cache.Files)
            {
                var item = Project.GetElement(cachedItem.Value.InputFile) as ContentFile;
                if (item != null)
                {
                    ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Clean, item), null, null);
                    if (File.Exists(GetDestinationFileAbsolute(item)))
                        File.Delete(GetDestinationFileAbsolute(item));
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
            using (ContentImporterContext importerContext = new ContentImporterContext())
            using (ContentProcessorContext processorContext = new ContentProcessorContext(UiContext))
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
            _cache.Save(GetCacheFile());
            IsBuilding = false;

            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build | BuildStep.Finished, null, null);
        }
        #endregion
    }
}

