using System;
using System.IO;
using System.Reflection;
using Assimp;
using Assimp.Unmanaged;
using engenious.Content.Pipeline;

namespace engenious.Pipeline
{
    [ContentImporter(".fbx",".dae", DisplayName = "Model Importer", DefaultProcessor = "ModelProcessor")]
    public class FbxImporter : ContentImporter<Scene>
    {

        private static readonly Exception DllLoadExc;

        static FbxImporter()
        {
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (dir == null)
                    throw new Exception("executing path not found");
                string ext = ".dll";
                switch (PlatformHelper.RunningPlatform())
                {
                    case Platform.Linux:
                        ext = ".so";
                        break;
                    case Platform.Mac:
                        ext = ".dylib";
                        break;
                }
                if (Environment.Is64BitProcess)
                    dir = Path.Combine(dir, "Assimp64" + ext);
                else
                    dir = Path.Combine(dir, "Assimp64" + ext);
                AssimpLibrary.Instance.LoadLibrary(dir);
            }
            catch (Exception ex)
            {
                DllLoadExc = ex;
            }
        }

        public override Scene Import(string filename, ContentImporterContext context)
        {
            if (DllLoadExc != null)
                context.RaiseBuildMessage("FBXIMPORT" , DllLoadExc.Message, BuildMessageEventArgs.BuildMessageType.Error);
            try
            {
                AssimpContext c = new AssimpContext();
                return c.ImportFile(filename,PostProcessSteps.Triangulate | PostProcessSteps.OptimizeMeshes | PostProcessSteps.OptimizeGraph);
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename , ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }
    }
}

