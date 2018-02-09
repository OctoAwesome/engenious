namespace engenious.Graphics
{
    public enum DepthFunction
    {
        Never = 512,
        Less,
        Equal,
        Lequal,
        Greater,
        Notequal,
        Gequal,
        Always
    }

    public enum StencilFunction
    {
        Never = 512,
        Less,
        Equal,
        Lequal,
        Greater,
        Notequal,
        Gequal,
        Always
    }

    public enum StencilOp
    {
        Zero,
        Invert = 5386,
        Keep = 7680,
        Replace,
        Incr,
        Decr,
        IncrWrap = 34055,
        DecrWrap
    }
}

