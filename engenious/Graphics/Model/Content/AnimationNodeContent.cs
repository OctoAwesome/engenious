using System.Collections.Generic;
using System.Linq;

namespace engenious.Graphics
{
    internal class AnimationNodeContent
    {
        public NodeContent Node{ get; set; }

        public List<AnimationFrame> Frames{ get; set; }

        public void Sort()
        {
            Frames = Frames.OrderBy(f => f.Frame).ToList();
        }
    }
}

