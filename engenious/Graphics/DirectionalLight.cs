namespace engenious.Graphics
{
	public class DirectionalLight
	{
		internal EffectParameter DirectionParameter,DiffuseColorParameter,SpecularColorParameter;
		public DirectionalLight (DirectionalLight clone)
			:this(clone.DirectionParameter,clone.DiffuseColorParameter,clone.SpecularColorParameter)
		{
			DiffuseColor = clone.DiffuseColor;
			Direction = clone.Direction;
			SpecularColor = clone.SpecularColor;
			Enabled = clone.Enabled;
		}
		public DirectionalLight (EffectParameter directionParameter,EffectParameter diffuseColorParameter,EffectParameter specularColorParameter)
		{
			DirectionParameter = directionParameter;
			DirectionParameter = diffuseColorParameter;
			SpecularColorParameter = specularColorParameter;
		}
		public Vector3 DiffuseColor{get; }
	    public Vector3 Direction{get; }
	    public Vector3 SpecularColor{get; }
	    public bool Enabled{get; }

	}
}

