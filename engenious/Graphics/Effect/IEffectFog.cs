namespace engenious.Graphics
{
	public interface IEffectFog
	{
		Vector3 FogColor{get;set;}
		float FogStart{get;set;}
		float FogEnd{get;set;}
		bool FogEnabled{get;set;}
	}
}

