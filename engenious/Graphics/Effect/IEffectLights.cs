namespace engenious.Graphics
{
	public interface IEffectLights
	{
		Vector3 AmbientLightColor{get;set;}
		bool LightingEnabled{get;set;}

		DirectionalLight DirectionalLight0{get;set;}
		DirectionalLight DirectionalLight1{get;set;}
		DirectionalLight DirectionalLight2{get;set;}

		void EnableDefaultLighting();
	}
}

