namespace engenious.Graphics
{
    public enum BlendingFactorSrc
    {
        Zero,
        SrcColor = 768,
        OneMinusSrcColor,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        DstColor,
        OneMinusDstColor,
        SrcAlphaSaturate,
        ConstantColor = 32769,
        ConstantColorExt = 32769,
        OneMinusConstantColor,
        OneMinusConstantColorExt = 32770,
        ConstantAlpha,
        ConstantAlphaExt = 32771,
        OneMinusConstantAlpha,
        OneMinusConstantAlphaExt = 32772,
        Src1Alpha = 34185,
        Src1Color = 35065,
        OneMinusSrc1Color,
        OneMinusSrc1Alpha,
        One = 1
    }

    public enum BlendingFactorDest
    {
        Zero,
        SrcColor = 768,
        OneMinusSrcColor,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        DstColor,
        OneMinusDstColor,
        SrcAlphaSaturate,
        ConstantColor = 32769,
        ConstantColorExt = 32769,
        OneMinusConstantColor,
        OneMinusConstantColorExt = 32770,
        ConstantAlpha,
        ConstantAlphaExt = 32771,
        OneMinusConstantAlpha,
        OneMinusConstantAlphaExt = 32772,
        Src1Alpha = 34185,
        Src1Color = 35065,
        OneMinusSrc1Color,
        OneMinusSrc1Alpha,
        One = 1
    }

    public enum BlendEquationMode
    {
        FuncAdd = 32774,
        Min,
        Max,
        FuncSubtract = 32778,
        FuncReverseSubtract
    }
}

