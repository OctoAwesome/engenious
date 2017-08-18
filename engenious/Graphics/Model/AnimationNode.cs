using System;
using System.Linq;
using System.Collections.Generic;

namespace engenious.Graphics
{
    public class AnimationNode
    {
        private bool _sorted = false;

        public Node Node{ get; set; }

        public List<AnimationFrame> Frames{ get; set; }

        public void Sort()
        {
            if (_sorted)
                return;
            _sorted = true;
            Frames = Frames.OrderBy(f => f.Frame).ToList();
        }
        public bool Repeat{get;set;}=true;

        public void ApplyAnimation(float time, float maxTime)
        {
            Sort();
            var frameIndex = Math.Max(Frames.FindIndex(f => f.Frame >= time)-1,0);
            var frame = Frames[frameIndex];
            AnimationFrame nextFrame=null;
            if (Repeat){
                nextFrame = Frames[(frameIndex + 1) % Frames.Count];
            }
            else if(frameIndex < Frames.Count-1)
                nextFrame = Frames[frameIndex + 1];
            else
                return;
            
            var diff = time-frame.Frame;
            var frameTime = nextFrame.Frame - frame.Frame;

            /*if (diff == 0)
            {
                Node.LocalTransform = frame.Transform.ToMatrix();
            }
            else if (diff > 0)*/
            var percent=diff / frameTime;
            if (Node.Name.Contains("$"))
            {
                Console.WriteLine(frameIndex + " - " + percent);
            }
            {
                //Matrix m1 = frame.Transform.ToMatrix();
                //Matrix m2 = nextFrame.Transform.ToMatrix();
                //Node.LocalTransform = Matrix.Lerp(m1,m2,percent);
                Node.LocalTransform = AnimationTransform.Lerp(frame.Transform, nextFrame.Transform, percent).ToMatrix();
            }
        }
    }
}

