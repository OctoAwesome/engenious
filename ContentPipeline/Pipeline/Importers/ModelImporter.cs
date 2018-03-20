using System;
using System.IO;
using System.Reflection;
using Assimp;
using Assimp.Unmanaged;
using engenious.Content.Pipeline;
using engenious.Helper;

namespace engenious.Pipeline
{
    [ContentImporter(".3D",".3DS",".3MF",".AC",".AC3D",".ACC",".AMJ",".ASE",".ASK",".B3D",".BLEND",".BVH",".COB",".CMS",".DAE",".DXF",".ENFF",".FBX",".glTF",".glTF",".HMB",".IFC-STEP",".IRR",".LWO",".LWS",".LXO",".MD2",".MD3",".MD5",".MDC",".MDL",".MESH",".MOT",".MS3D",".NDO",".NFF",".OBJ",".OFF",".OGEX",".PLY",".PMX",".PRJ",".Q3O",".Q3S",".RAW",".SCN",".SIB",".SMD",".STL",".STP",".TER",".UC",".VTA",".X",".X3D",".XGL",".ZGL", DisplayName = "Model Importer", DefaultProcessor = "ModelProcessor")]
    public class ModelImporter : ContentImporter<Scene>
    {

        private static readonly Exception DllLoadExc;

        static ModelImporter()
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
                return c.ImportFile(filename,PostProcessSteps.Triangulate);//,PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.FindInstances | PostProcessSteps.Triangulate | PostProcessSteps.OptimizeMeshes | PostProcessSteps.OptimizeGraph
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename , ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }
    }
}

