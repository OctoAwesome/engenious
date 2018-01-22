namespace engenious.Graphics
{
	public class Effect:GraphicsResource,IEffect
	{
		public Effect (GraphicsDevice graphicsDevice)
			: base (graphicsDevice)
		{
			Techniques = new EffectTechniqueCollection ();

		}

		protected internal virtual void Initialize ()
		{
			Parameters = new EffectParameterCollection (Techniques);
			foreach (var technique in Techniques)
			{
				technique.Initialize();
			}
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
			foreach (var pass in CurrentTechnique.Passes) {
				pass.Apply ();
			}
		}
	}
}

