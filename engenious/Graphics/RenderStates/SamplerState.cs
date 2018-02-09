namespace engenious.Graphics
{
    public class SamplerState:GraphicsResource
    {
        public static readonly SamplerState LinearClamp;
        public static readonly SamplerState LinearWrap;

        static SamplerState()
        {
            LinearClamp = new SamplerState();
            LinearWrap = new SamplerState();
            LinearWrap.AddressU = LinearWrap.AddressV = LinearWrap.AddressW = TextureWrapMode.Repeat;
        }

        public SamplerState()
        {
            //TODO: implement completly	
            AddressU = AddressV = AddressW = TextureWrapMode.ClampToEdge;

        }

        public TextureWrapMode AddressU{ get; set; }

        public TextureWrapMode AddressV{ get; set; }

        public TextureWrapMode AddressW{ get; set; }

        public TextureFilter TextureFilter{get;set;}
    }
}

