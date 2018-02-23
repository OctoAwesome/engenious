using System.Collections.Generic;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReader(typeof(ModelContent))]
    public class ModelContentTypeReader:ContentTypeReader<ModelContent>
    {
        private NodeContent ReadTree(ModelContent model, ContentReader reader)
        {
            var index = reader.ReadInt32();
            var node = model.Nodes[index];
            var childCount = reader.ReadInt32();
            node.Children = new List<NodeContent>();
            for (var i = 0; i < childCount; i++)
                node.Children.Add(ReadTree(model, reader));

            return node;
        }

        public override ModelContent Read(ContentManager manager, ContentReader reader)
        {
            var model = new ModelContent();
            var meshCount = reader.ReadInt32();
            model.Meshes = new MeshContent[meshCount];
            for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
            {
                var m = new MeshContent();
                m.PrimitiveCount = reader.ReadInt32();
                bool hasPositions = reader.ReadBoolean();
                bool hasColors = reader.ReadBoolean();
                bool hasNormals = reader.ReadBoolean();
                bool hasTextureCoordinates = reader.ReadBoolean();
                var vertexCount = reader.ReadInt32();
                m.Vertices = new ConditionalVertexArray(vertexCount,hasPositions,hasColors,hasNormals,hasTextureCoordinates);


                Vector3 minVertex=new Vector3(float.MaxValue),maxVertex=new Vector3(float.MinValue);
                for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                {
                    if (hasPositions)
                    {
                        var position = reader.ReadVector3();
                        m.Vertices.AsPosition[vertexIndex] = position;
                        if (position.X < minVertex.X)
                            minVertex.X = position.X;
                        else if(position.X > maxVertex.X)
                            maxVertex.X = position.X;

                        if (position.Y < minVertex.Y)
                            minVertex.Y = position.Y;
                        else if(position.Y > maxVertex.Y)
                            maxVertex.Y = position.Y;

                        if (position.Z < minVertex.Z)
                            minVertex.Z = position.Z;
                        else if(position.Z > maxVertex.Z)
                            maxVertex.Z = position.Z;
                    }

                    if (hasColors)
                    {
                        m.Vertices.AsColor[vertexIndex] = reader.ReadColor();
                    }

                    if (hasNormals)
                    {
                        m.Vertices.AsNormal[vertexIndex] = reader.ReadVector3();
                    }

                    if (hasTextureCoordinates)
                    {
                        m.Vertices.AsTextureCoordinate[vertexIndex] = reader.ReadVector2();
                    }
                }
                //m.BoundingBox = new BoundingBox(minVertex,maxVertex);
                model.Meshes[meshIndex] = m;
            }
            var nodeCount = reader.ReadInt32();
            model.Nodes = new List<NodeContent>();
            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var node = new NodeContent();
                node.Name = reader.ReadString();
                node.Transformation = reader.ReadMatrix();
                var nodeMeshCount = reader.ReadInt32();
                node.Meshes = new List<int>();
                for (var meshIndex = 0; meshIndex < nodeMeshCount; meshIndex++)
                {
                    node.Meshes.Add(reader.ReadInt32());
                }
                model.Nodes.Add(node);
            }

            model.RootNode = ReadTree(model, reader);
            var animationCount = reader.ReadInt32();
            for (var animationIndex=0;animationIndex<animationCount;animationIndex++)
            {
                var anim = new AnimationContent();
                anim.MaxTime = reader.ReadSingle();
                var channelCount = reader.ReadInt32();
                anim.Channels = new List<AnimationNodeContent>();
                for (var channel = 0; channel < channelCount; channel++)
                {
                    var node = new AnimationNodeContent();
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

            //model.UpdateAnimation(null, model.RootNode);
            return model;
        }
    }
}

