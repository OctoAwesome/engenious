namespace engenious.Graphics
{
	public class EffectTechnique
	{
		protected internal EffectTechnique (string name)
		{
			Name = name;
			Passes = new EffectPassCollection ();
		}

		public string Name { get; private set;}
		public EffectPassCollection Passes{ get; private set; }

		protected internal virtual void Initialize()
		{
			
		}
	}
}

