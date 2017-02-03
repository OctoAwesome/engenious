namespace engenious.Graphics
{
    public class RasterizerState : GraphicsResource
    {
        public static readonly RasterizerState CullNone;
        public static readonly RasterizerState CullClockwise;
        public static readonly RasterizerState CullCounterClockwise;

        static RasterizerState()
        {
            CullNone = new RasterizerState();
            CullNone.CullMode = CullMode.None;
            CullNone.FillMode = PolygonMode.Fill;

            CullClockwise = new RasterizerState();
            CullClockwise.CullMode = CullMode.Clockwise;
            CullClockwise.FillMode = PolygonMode.Fill;

            CullCounterClockwise = new RasterizerState();
            CullCounterClockwise.CullMode = CullMode.CounterClockwise;
            CullCounterClockwise.FillMode = PolygonMode.Fill;


        }

        public RasterizerState()
        {
            CullMode = CullMode.None;
            FillMode = PolygonMode.Fill;
        }

        public CullMode CullMode{ get; set; }

        public PolygonMode FillMode{ get; set; }

        public bool MultiSampleAntiAlias{ get; set; }

        public bool ScissorTestEnable{ get; set; }
    }
}

