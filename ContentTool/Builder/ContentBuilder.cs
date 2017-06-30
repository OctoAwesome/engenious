using ContentTool.Models;
using engenious.Content.Pipeline;
using engenious.Content.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContentTool.Builder
{
    public class ContentBuilder
    {
        public const string FileExtension = ".ego";

        public ContentProject Project {
            get => project;
            set
            {
                project = value;
            }
        }
        private ContentProject project;

        public int FailedBuilds { get; private set; }

        public bool IsBuilding { get; private set; }

        private SynchronizationContext SyncContext;

        private BuildCache cache;

        private Thread buildThread;

        public ContentBuilder(ContentProject project)
        {
            Project = project;
            SyncContext = SynchronizationContext.Current;

            cache = BuildCache.Load(Path.Combine(Path.GetDirectoryName(project.ContentProjectPath), "obj", project.Name + ".dat"));
        }

        public void Build(ContentItem item)
        {
            buildThread = new Thread(() => {
                IsBuilding = true;
                BuildThread(item);
                IsBuilding = false;
            });
            buildThread.SetApartmentState(ApartmentState.STA);
            buildThread.Start();
        }

        public void Clean()
        {
            buildThread = new Thread(() =>
            {
                IsBuilding = true;
                CleanThread();
                IsBuilding = false;
            });
            buildThread.SetApartmentState(ApartmentState.STA);
            buildThread.Start();
        }

        public void Rebuild()
        {
            buildThread = new Thread(() => {
                IsBuilding = true;
                CleanThread();
                BuildThread(Project);
                IsBuilding = false;
            });
            buildThread.SetApartmentState(ApartmentState.STA);
            buildThread.Start();
        }

        public void Abort()
        {
            if (IsBuilding)
                buildThread.Suspend();
        }
        public void Join()
        {
            buildThread.Join();
        }

        protected void BuildThread(ContentItem item)
        {
            FailedBuilds = 0;
            PipelineHelper.PreBuilt(Project);

            string outputDestination = Path.Combine(Path.GetDirectoryName(Project.ContentProjectPath), string.Format(Project.OutputDirectory.Replace("{Configuration}", "{0}"), Project.Configuration));

            using (ContentImporterContext iContext = new ContentImporterContext())
            using (ContentProcessorContext pContext = new ContentProcessorContext(SyncContext))
            {
                iContext.BuildMessage += RaiseBuildMessage;
                pContext.BuildMessage += RaiseBuildMessage;
                InternalBuildItem(item, outputDestination, iContext, pContext);
            }
            cache.Save();
        }

        protected void CleanThread()
        {
            foreach(var item in cache.Files)
            {
                if (File.Exists(item.Value.OutputFilePath))
                    File.Delete(item.Value.OutputFilePath);
            }
        }

        protected void InternalBuildItem(ContentItem item, string outputDestination, ContentImporterContext importerContext, ContentProcessorContext processorContext)
        {
            outputDestination = Path.Combine(outputDestination, item.Name);

            if(item is ContentFolder)
            {
                foreach (var child in ((ContentFolder)item).Content)
                    InternalBuildItem(child, outputDestination, importerContext, processorContext);
            }
            else
            {
                if (cache.NeedsRebuild(item.FilePath))
                {
                    InternalBuildFile(item as ContentFile, Path.Combine(Path.GetDirectoryName(outputDestination), Path.GetFileNameWithoutExtension(item.Name) + FileExtension), importerContext, processorContext);
                    RaiseBuildMessage(this, new BuildMessageEventArgs(item.RelativePath, item.RelativePath + " built", BuildMessageEventArgs.BuildMessageType.Information));
                }
                else
                {
                    RaiseBuildMessage(this, new BuildMessageEventArgs(item.RelativePath, item.RelativePath + " skipped", BuildMessageEventArgs.BuildMessageType.Information));
                }
                importerContext.Dependencies.Clear();
                processorContext.Dependencies.Clear();
            }


        }

        protected void RaiseBuildMessage(object sender, BuildMessageEventArgs e) => BuildMessage?.Invoke(e);

        public object InternalBuildFile(ContentFile item, string destination, ContentImporterContext importerContext, ContentProcessorContext processorContext)
        {
            return BuildFile(item, destination, importerContext, processorContext, cache);
        }
        /// <summary>
        /// Builds a ContentFile and writes it to the destinatin
        /// </summary>
        /// <param name="item">ContentFile to build</param>
        /// <param name="destination">Location to save to</param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        /// <returns></returns>
        public static object BuildFile(ContentFile item, string destination, ContentImporterContext importerContext, ContentProcessorContext processorContext,BuildCache cache = null)
        {
            if (!File.Exists(item.FilePath)) return null;

            var dirName = Path.GetDirectoryName(destination);
            Directory.CreateDirectory(dirName);

            BuildFile buildFile = new BuildFile(item.FilePath, destination);

            var importer = item.Importer;
            if (importer == null) return null;

            var importedFile = importer.Import(item.FilePath, importerContext);
            if (importedFile == null) return null;

            buildFile.Dependencies.AddRange(importerContext.Dependencies);
            cache?.AddDependencies(Path.GetDirectoryName(item.FilePath), importerContext.Dependencies);

            var processor = item.Processor;
            if (processor == null) return null;

            var processedFile = processor.Process(importedFile, item.FilePath, processorContext);
            if (processedFile == null) return null;

            buildFile.Dependencies.AddRange(processorContext.Dependencies);
            cache?.AddDependencies(Path.GetDirectoryName(item.FilePath), processorContext.Dependencies);

            IContentTypeWriter typeWriter = SerializationManager.Instance.GetWriter(processedFile.GetType());
            var outputContentFileWriter = new engenious.Content.ContentFile(typeWriter.RuntimeReaderName);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(destination, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(fs, outputContentFileWriter);
                ContentWriter writer = new ContentWriter(fs);
                writer.WriteObject(processedFile, typeWriter);
            }

            cache?.AddFile(item.FilePath, buildFile);
            buildFile.RefreshModifiedTime();

            return processedFile;
        }

        public event BuildMessageHandler BuildMessage;
        public event BuildStatusChangedHandler BuildStatusChanged;

        public delegate void BuildMessageHandler(BuildMessageEventArgs args);
        public delegate void BuildStatusChangedHandler(BuildStatus status);

        [Flags]
        public enum BuildStatus
        {
            Clean = 1,
            Build = 2,
            Abort = 4,
            Finished = 8,
            Built = 16
        }

    }
}
