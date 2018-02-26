namespace engenious
{
    public enum TextureWrapMode
    {
        Repeat = 10497,
        ClampToBorder = 33069,
        ClampToBorderArb = 33069,
        ClampToBorderNv = 33069,
        ClampToBorderSgis = 33069,
        ClampToEdge = 33071,
        ClampToEdgeSgis = 33071,
        MirroredRepeat = 33648
    }
    public enum MagFilter
    {
        Nearest = 9728, // 0x00002600
        Linear = 9729, // 0x00002601
        LinearDetailSgis = 32919, // 0x00008097
        LinearDetailAlphaSgis = 32920, // 0x00008098
        LinearDetailColorSgis = 32921, // 0x00008099
        LinearSharpenSgis = 32941, // 0x000080AD
        LinearSharpenAlphaSgis = 32942, // 0x000080AE
        LinearSharpenColorSgis = 32943, // 0x000080AF
        Filter4Sgis = 33094, // 0x00008146
        PixelTexGenQCeilingSgix = 33156, // 0x00008184
        PixelTexGenQRoundSgix = 33157, // 0x00008185
        PixelTexGenQFloorSgix = 33158, // 0x00008186
    }

    public enum MinFilter
    {
        Nearest = 9728, // 0x00002600
        Linear = 9729, // 0x00002601
        NearestMipmapNearest = 9984, // 0x00002700
        LinearMipmapNearest = 9985, // 0x00002701
        NearestMipmapLinear = 9986, // 0x00002702
        LinearMipmapLinear = 9987, // 0x00002703
        Filter4Sgis = 33094, // 0x00008146
        LinearClipmapLinearSgix = 33136, // 0x00008170
        PixelTexGenQCeilingSgix = 33156, // 0x00008184
        PixelTexGenQRoundSgix = 33157, // 0x00008185
        PixelTexGenQFloorSgix = 33158, // 0x00008186
        NearestClipmapNearestSgix = 33869, // 0x0000844D
        NearestClipmapLinearSgix = 33870, // 0x0000844E
        LinearClipmapNearestSgix = 33871, // 0x0000844F
    }
}

