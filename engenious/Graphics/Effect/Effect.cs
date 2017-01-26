using System;

namespace engenious.Graphics
{
	public class Effect:GraphicsResource,IEffect
	{
		public Effect (GraphicsDevice graphicsDevice)
			: base (graphicsDevice)
		{
			Techniques = new EffectTechniqueCollection ();

		}

		internal void Initialize ()
		{
			Parameters = new EffectParameterCollection (Techniques);
		}

		public EffectParameterCollection Parameters {
			get;
			private set;
		}

		public EffectTechniqueCollection Techniques {
			get;
			private set;
		}

		public EffectTechnique CurrentTechnique {
			get;
			set;
		}

		protected internal virtual void OnApply ()
		{
			foreach (EffectPass pass in CurrentTechnique.Passes.PassesList) {
				pass.Apply ();
			}
		}
	}
}

