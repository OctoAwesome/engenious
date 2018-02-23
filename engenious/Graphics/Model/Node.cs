using System.Collections.Generic;

namespace engenious.Graphics
{
    public class Node
    {
        public Node()
        {
            GlobalTransform = LocalTransform = Matrix.Identity;
        }

        public string Name{ get; set; }

        public List<IMesh> Meshes{ get; set; }

        public Matrix Transformation{ get; set; }

        public Matrix LocalTransform{ get; set; }

        public Matrix GlobalTransform{ get; set; }

        public List<Node> Children{ get; set; }
    }
}

