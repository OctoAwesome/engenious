namespace engenious.Graphics
{
	public sealed class EffectTechnique
	{
		internal EffectTechnique (string name)
		{
			Name = name;
			Passes = new EffectPassCollection ();
		}

		public string Name { get; private set;}
		public EffectPassCollection Passes{ get; private set; }
	}
}

