using System;
using engenious.Content.Pipeline;
using engenious.Graphics;

namespace engenious.Pipeline
{
    [ContentProcessor(DisplayName = "Ego Model Processor")]
    public class EgoModelProcessor : ContentProcessor<ModelContent, ModelContent>
    {
        private void RemoveFrame(AnimationContent anim,int index)
        {
            foreach(var c in anim.Channels)
                c.Frames.RemoveAt(index);//letzter frame entfernen...
        }
        #region implemented abstract members of ContentProcessor
        public override ModelContent Process(ModelContent input, string filename, ContentProcessorContext context)
        {

            RemoveFrame(input.Animations[0],input.Animations[0].Channels[0].Frames.Count-1);//letzte frame entfernen...
            RemoveFrame(input.Animations[1],input.Animations[1].Channels[0].Frames.Count-1);//letzte frame entfernen...
            RemoveFrame(input.Animations[2],0);//letzte frame entfernen...
            RemoveFrame(input.Animations[3],input.Animations[3].Channels[0].Frames.Count-1);//letzte frame entfernen...
            RemoveFrame(input.Animations[4],0);//letzte frame entfernen...

            RemoveFrame(input.Animations[5],input.Animations[5].Channels[0].Frames.Count-1);//letzte frame entfernen...
            RemoveFrame(input.Animations[6],input.Animations[6].Channels[0].Frames.Count-1);//letzte frame entfernen...

            foreach(var a in input.Animations)
            {
                if (a.Channels.Count > 0){
                    float diff = a.Channels[0].Frames[0].Frame;
                    float max=0.0f;
                    foreach(var c in a.Channels)
                    {
                        foreach(var f in c.Frames)
                        {
                            f.Frame -= diff;
                            max = Math.Max(f.Frame,max);
                        }

                    }
                    a.MaxTime = max;
                }
            }



            return input;
        }
        #endregion
    }

}

