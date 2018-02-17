using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assimp;
using engenious.Content.Pipeline;
using engenious.Graphics;
using Node = Assimp.Node;

namespace engenious.Pipeline
{
    [ContentProcessor(DisplayName = "Model Processor")]
    public class ModelProcessor : ContentProcessor<Scene, ModelContent,ModelProcessorSettings>
    {
        private NodeContent ParseNode(ModelContent model, Node node,NodeContent parent=null)
        {
            NodeContent n = new NodeContent(parent);
            model.Nodes.Add(n);
            if (settings.TransformMesh){
                Matrix matrix = ConvertMatrix(node.Transform);
                matrix.M41 *= settings.Scale.X;
                matrix.M42 *= settings.Scale.Y;
                matrix.M43 *= settings.Scale.Z;
                n.Transformation = matrix;
            }else
                n.Transformation = Matrix.Identity;

            n.Name = node.Name;
            n.Meshes = new List<int>();
            foreach (var meshIndex in node.MeshIndices)
                n.Meshes.Add(meshIndex);
            n.Children = new List<NodeContent>();
            foreach (var child in node.Children)
                n.Children.Add(ParseNode(model, child,n));
            return n;
        }

        private Matrix ConvertMatrix(Matrix4x4 m)
        {
            return new Matrix(m.A1, m.A2, m.A3, m.A4,
                m.B1, m.B2, m.B3, m.B4,
                m.C1, m.C2, m.C3, m.C4,
                m.D1, m.D2, m.D3, m.D4);
        }
        private bool CombineWithParentNode(ModelContent content,NodeContent node)
        {
            if (node.Parent == null)
                return false;
            foreach(var c in node.Children)
            {
                c.Parent = node.Parent;

                node.Parent.Children.Add(c);
                
            }
            node.Children.Clear();
            content.Nodes.Remove(node);

            var changed = node.Parent.Children.Remove(node);
            if (changed)
            {
                foreach(var anim in content.Animations)
                {
                    AnimationNodeContent c1=null,c2=null;
                    foreach(var c in anim.Channels)
                    {
                        if (c.Node == node.Parent)
                            c1 = c;
                        else if(c.Node == node)
                            c2 = c;

                        if (c1 != null && c2 != null)
                            break;
                    }
                    CombineAnimationContent(c1,c2);
                    for(int i=anim.Channels.Count-1;i>= 0;i--)
                    {
                        if (anim.Channels[i].Node == node)
                            anim.Channels.RemoveAt(i);
                    }
                }
            }
            if (!node.Name.Contains("PreRotation"))
                node.Parent.Transformation = node.Transformation*node.Parent.Transformation;

            return changed;
        }
        private void PostProcess(ModelContent content,NodeContent node)
        {
            if (node.Name.Contains("$") && node.Name.Contains("Translation"))
                node.Name = node.Name.Replace("Translation","Transform");
            if (node.Name.Contains("$") && !node.Name.Contains("Transform"))
            {
                if(CombineWithParentNode(content,node))
                    PostProcess(content,node.Parent);
            }
            for (int i=node.Children.Count-1;i>=0;i--){
                var child = node.Children[i];
                if (child.Name.Contains("$") && child.Name.Contains("PreRotation"))
                {
                    foreach(var anim in content.Animations)
                    {
                        AnimationNodeContent c=anim.Channels.FirstOrDefault(x=>x.Node == node);
                        if (c == null)
                            continue;
                        foreach(var f in c.Frames)
                        {
                            var newVec = Vector3.Transform(f.Transform.Location,child.Transformation);
                            newVec = new Vector3(newVec.X,newVec.Y,-newVec.Z);//TODO: hardcoded?
                            f.Transform = new AnimationTransform(string.Empty,newVec,f.Transform.Scale,f.Transform.Rotation);
                        }
                    }
                }
                PostProcess(content,child);
            }

        }
        private void CombineAnimationContent(AnimationNodeContent c1,AnimationNodeContent c2)
        {
            if (c1 == null || c2 == null)
                return;
            AnimationFrame empty = new AnimationFrame();
            empty.Transform = new AnimationTransform(string.Empty,new Vector3(),new Vector3(1),new Quaternion(0,0,0,1));
            for (int i=0;i<Math.Max(c1.Frames.Count,c2.Frames.Count);i++)
            {
                AnimationFrame f1 = i<c1.Frames.Count?c1.Frames[i]:empty;
                AnimationFrame f2 = i<c2.Frames.Count?c2.Frames[i]:empty;
                if (f1== empty){
                    c1.Frames.Add(f2);
                }else{
                    AnimationTransform t1 = f1.Transform;
                    AnimationTransform t2 = f2.Transform;
                    f1.Transform = t1 + t2;
                }
            }
        }
        public override ModelContent Process(Scene scene, string filename, ContentProcessorContext context)
        {
            try
            {
                //AssimpContext c = new AssimpContext();
                //ExportFormatDescription des = c.GetSupportedExportFormats()[0];
                //c.ExportFile(scene,Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"test.dae"),des.FormatId);
                ModelContent content = new ModelContent();
                content.Meshes = new MeshContent[scene.MeshCount];
                for (int meshIndex = 0; meshIndex < scene.MeshCount; meshIndex++)
                {
                    var sceneMesh = scene.Meshes[meshIndex];
                    var meshContent = new MeshContent();
                    meshContent.PrimitiveCount = sceneMesh.FaceCount;

                    meshContent.Vertices = new VertexPositionNormalTexture[meshContent.PrimitiveCount * 3];
                    int vertex=0;
                    //TODO: indexing
                    foreach(var f in sceneMesh.Faces)
                    {
                        foreach(var i in f.Indices)
                        {
                            var pos = sceneMesh.Vertices[i];
                            var norm = sceneMesh.Normals[i];
                            Vector3D tex = new Vector3D();
                            if (sceneMesh.TextureCoordinateChannels.Length > 0 && sceneMesh.TextureCoordinateChannels[0].Count >i)
                                tex = sceneMesh.TextureCoordinateChannels[0][i];
                            var translated = new Vector3(pos.X, pos.Y, pos.Z)+settings.Translate;
                            meshContent.Vertices[vertex++] = new VertexPositionNormalTexture(
                                new Vector3(translated.X*settings.Scale.X,translated.Y*settings.Scale.Y,translated.Z*settings.Scale.Z),
                                new Vector3(norm.X, norm.Y, norm.Z),
                                new Vector2(tex.X, -tex.Y));
                        }
                    }
                    /*for (int i = 0; i < sceneMesh.VertexCount; i++)
                    {

                    }*/

                    content.Meshes[meshIndex] = meshContent;
                }
                content.Nodes = new List<NodeContent>();
                content.RootNode = ParseNode(content, scene.RootNode);
                foreach(var animation in scene.Animations){
                    var anim = new AnimationContent();
                    anim.Channels = new List<AnimationNodeContent>();
                    foreach (var channel in animation.NodeAnimationChannels)
                    {
                        AnimationNodeContent node = new AnimationNodeContent();
                        var curNode = content.Nodes.First(n => n.Name == channel.NodeName);
                        node.Node = curNode;
                        node.Frames = new List<AnimationFrame>();
                        int frameCount = Math.Max(Math.Max(channel.PositionKeyCount, channel.RotationKeyCount), channel.ScalingKeyCount);
                        float diff=0.0f,maxTime = 0;
                        for (int i = 0; i < frameCount; i++)
                        {

                            AnimationFrame frame = new AnimationFrame();

                            if (i < channel.PositionKeyCount)
                                frame.Frame = (float)channel.PositionKeys[i].Time;
                            else if (i < channel.RotationKeyCount)
                                frame.Frame = (float)channel.RotationKeys[i].Time;
                            else if (i < channel.ScalingKeyCount)
                                frame.Frame = (float)channel.ScalingKeys[i].Time;
                            if (i==0)
                                diff = frame.Frame;
                            frame.Frame -= diff;
                            frame.Frame = (float)(frame.Frame / animation.TicksPerSecond);
                            maxTime = Math.Max(frame.Frame, maxTime);
                            //TODO: interpolation
                            var rot = channel.RotationKeyCount == 0 ? new Assimp.Quaternion(1, 0, 0, 0) : i >= channel.RotationKeyCount ? channel.RotationKeys.Last().Value : channel.RotationKeys[i].Value;
                            var loc = channel.PositionKeyCount == 0 ? new Vector3D() : i >= channel.PositionKeyCount ? channel.PositionKeys.Last().Value : channel.PositionKeys[i].Value;
                            var sca = channel.ScalingKeyCount == 0 ? new Vector3D(1, 1, 1) : i >= channel.ScalingKeyCount ? channel.ScalingKeys.Last().Value : channel.ScalingKeys[i].Value;
                            rot.Normalize();

                            frame.Transform = new AnimationTransform(node.Node.Name,new Vector3((loc.X+settings.Translate.X), (loc.Y+settings.Translate.Y), (loc.Z+settings.Translate.Z)),
                                new Vector3(sca.X*settings.Scale.X, sca.Y*settings.Scale.Y, sca.Z*settings.Scale.Z), new Quaternion(rot.X, rot.Y, rot.Z, rot.W));
                            node.Frames.Add(frame);
                        }
                        anim.MaxTime = maxTime;
                        anim.Channels.Add(node);
                    }
                    content.Animations.Add(anim);
                }
                PostProcess(content,content.RootNode);



                return content;
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename, ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }

    }
    [Serializable]
    public class ModelProcessorSettings : ProcessorSettings
    {
        [Category("Settings")]
        [DefaultValue("[1, 1, 1]")]
        public Vector3 Scale{get;set;}=new Vector3(1);
        [Category("Settings")]
        [DefaultValue("[0, 0, 0]")]
        public Vector3 Translate{get;set;} = new Vector3();
        [Category("Settings")]
        [DefaultValue("[0, 0, 0]")]
        public Vector3 Rotation{get;set;} = new Vector3();
        [DefaultValue(true)]
        public bool TransformMesh{get;set;}=true;
    }
}

