using System.Collections.Generic;

namespace engenious.Graphics
{
    internal class NodeContent
    {
        public NodeContent(NodeContent parent=null)
        {
            GlobalTransform = LocalTransform = Matrix.Identity;
            Parent = parent;
        }

        public string Name{ get; set; }

        public List<int> Meshes{ get; set; }

        public Matrix Transformation{ get; set; }

        public Matrix LocalTransform{ get; set; }

        public Matrix GlobalTransform{ get; set; }

        public List<NodeContent> Children{ get; set; }
        public NodeContent Parent{get;internal set;}
    }
}

