namespace engenious.Graphics
{
    public interface IEffect
    {
        EffectParameterCollection Parameters {
            get;
        }

        EffectTechniqueCollection Techniques {
            get;
        }

        EffectTechnique CurrentTechnique {
            get;
            set;
        }
    }
}

