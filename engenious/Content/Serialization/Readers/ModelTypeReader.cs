using System.Collections.Generic;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReader(typeof(Model))]
    public class ModelTypeReader:ContentTypeReader<Model>
    {
        private Node ReadTree(Model model, ContentReader reader)
        {
            var index = reader.ReadInt32();
            var node = model.Nodes[index];
            var childCount = reader.ReadInt32();
            node.Children = new List<Node>();
            for (var i = 0; i < childCount; i++)
                node.Children.Add(ReadTree(model, reader));

            return node;
        }

        public override Model Read(ContentManager manager, ContentReader reader)
        {
            var model = new Model(manager.GraphicsDevice);
            var meshCount = reader.ReadInt32();
            model.Meshes = new Mesh[meshCount];
            for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
            {
                var m = new Mesh(model.GraphicsDevice);
                m.PrimitiveCount = reader.ReadInt32();
                var vertexCount = reader.ReadInt32();
                var vertices = new VertexPositionNormalTexture[vertexCount];
                Vector3 minVertex=new Vector3(float.MaxValue),maxVertex=new Vector3(float.MinValue);
                for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                {
                    var vertex = reader.ReadVertexPositionNormalTexture();
                    if (vertex.Position.X < minVertex.X)
                        minVertex.X = vertex.Position.X;
                    else if(vertex.Position.X > maxVertex.X)
                        maxVertex.X = vertex.Position.X;

                    if (vertex.Position.Y < minVertex.Y)
                        minVertex.Y = vertex.Position.Y;
                    else if(vertex.Position.Y > maxVertex.Y)
                        maxVertex.Y = vertex.Position.Y;

                    if (vertex.Position.Z < minVertex.Z)
                        minVertex.Z = vertex.Position.Z;
                    else if(vertex.Position.Z > maxVertex.Z)
                        maxVertex.Z = vertex.Position.Z;

                    vertices[vertexIndex] = vertex;
                }
                m.VB = new VertexBuffer(m.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount);
                m.VB.SetData(vertices);
                m.BoundingBox = new BoundingBox(minVertex,maxVertex);
                model.Meshes[meshIndex] = m;
            }
            var nodeCount = reader.ReadInt32();
            model.Nodes = new List<Node>();
            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var node = new Node();
                node.Name = reader.ReadString();
                node.Transformation = reader.ReadMatrix();
                var nodeMeshCount = reader.ReadInt32();
                node.Meshes = new List<Mesh>();
                for (var meshIndex = 0; meshIndex < nodeMeshCount; meshIndex++)
                {
                    node.Meshes.Add(model.Meshes[reader.ReadInt32()]);
                }
                model.Nodes.Add(node);
            }

            model.RootNode = ReadTree(model, reader);
            var animationCount = reader.ReadInt32();
            for (var animationIndex=0;animationIndex<animationCount;animationIndex++)
            {
                var anim = new Animation();
                anim.MaxTime = reader.ReadSingle();
                var channelCount = reader.ReadInt32();
                anim.Channels = new List<AnimationNode>();
                for (var channel = 0; channel < channelCount; channel++)
                {
                    var node = new AnimationNode();
                    node.Node = model.Nodes[reader.ReadInt32()];
                    node.Frames = new List<AnimationFrame>();
                    var frameCount = reader.ReadInt32();
                    for (var i = 0; i < frameCount; i++)
                    {
                        var f = new AnimationFrame();
                        f.Frame = reader.ReadSingle();
                        f.Transform = new AnimationTransform(node.Node.Name,reader.ReadVector3(),reader.ReadVector3(),reader.ReadQuaternion());
                        node.Frames.Add(f);
                    }
                    anim.Channels.Add(node);
                }
                model.Animations.Add(anim);
            }
            foreach(var anim in model.Animations)
            {
                foreach(var ch in anim.Channels)
                {
                    if (!ch.Node.Name.Contains("$"))
                        continue;
                    //var firstFrame = ch.Frames.FirstOrDefault();
                    for (var j=1;j<ch.Frames.Count;j++)
                    {
                        //var firstFrame = ch.Frames[j-1];
                        var f = ch.Frames[j];
                        f.Transform = new AnimationTransform(string.Empty,f.Transform.Location,f.Transform.Scale,f.Transform.Rotation);
                    }
                }
            }
            model.UpdateAnimation(null, model.RootNode);
            return model;
        }
    }
}

