using System;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    [Serializable]
    public struct VertexElement
    {
        public VertexElement(
            int offset,
            VertexElementFormat elementFormat,
            VertexElementUsage elementUsage,
            int usageIndex
        )
        {
            Offset = offset;
            VertexElementFormat = elementFormat;
            VertexElementUsage = elementUsage;
            UsageIndex = usageIndex;
        }

        public int Offset { get; set; }

        public VertexElementFormat VertexElementFormat { get; set; }

        public int UsageIndex { get; set; }

        public VertexElementUsage VertexElementUsage { get; set; }

        internal bool IsNormalized
        {
            get
            {
                switch (VertexElementFormat)
                {
                    case VertexElementFormat.NormalizedShort2:
                    case VertexElementFormat.NormalizedShort4:
                    case VertexElementFormat.Normalized101010:
                        return true;
                    default:
                        return false;
                }
            }
        }

        internal VertexAttribPointerType GetGlVertexDataType()
        {
            switch (VertexElementFormat)
            {
                case VertexElementFormat.Single:
                case VertexElementFormat.Vector2:
                case VertexElementFormat.Vector3:
                case VertexElementFormat.Vector4:
                case VertexElementFormat.Color:
                case VertexElementFormat.Rgba32:
                case VertexElementFormat.Rg32:
                    return VertexAttribPointerType.Float;
                case VertexElementFormat.HalfVector2:
                case VertexElementFormat.HalfVector4:
                    return VertexAttribPointerType.HalfFloat;
                case VertexElementFormat.Rgba64:
                    return VertexAttribPointerType.Double;
                case VertexElementFormat.NormalizedShort2:
                case VertexElementFormat.NormalizedShort4:
                case VertexElementFormat.Short2:
                case VertexElementFormat.Short4:
                    return VertexAttribPointerType.Short;
                case VertexElementFormat.Byte4:
                    return VertexAttribPointerType.Byte;
                case VertexElementFormat.Normalized101010:
                    return VertexAttribPointerType.Int;
                case VertexElementFormat.UInt101010:
                    return VertexAttribPointerType.UnsignedInt;
                default:
                    throw new NotImplementedException(); //TODO: dunno
            }

        }

        internal int GetGlVertexDataTypeSize()
        {
            switch (GetGlVertexDataType())
            {
                case VertexAttribPointerType.Byte:
                case VertexAttribPointerType.UnsignedByte:
                    return 1;
                case VertexAttribPointerType.Short:
                case VertexAttribPointerType.UnsignedShort:
                case VertexAttribPointerType.HalfFloat:
                    return 2;
                case VertexAttribPointerType.Int:
                case VertexAttribPointerType.UnsignedInt:
                case VertexAttribPointerType.Float:
                    return 4;
                case VertexAttribPointerType.Double:
                    return 8;
            }
            throw new NotImplementedException(); //TODO: dunno
        }

        public int ByteCount
        {
            get
            {
                switch (VertexElementFormat)
                {
                    case VertexElementFormat.Single:
                        return 4;
                    case VertexElementFormat.Vector2:
                        return 8;
                    case VertexElementFormat.Vector3:
                        return 12;
                    case VertexElementFormat.Vector4:
                        return 16;
                    case VertexElementFormat.HalfVector2:
                        return 4;
                    case VertexElementFormat.HalfVector4:
                        return 8;
                    case VertexElementFormat.Rgba64:
                        return 32;
                    case VertexElementFormat.Color:
                        return 16;
                    case VertexElementFormat.Rgba32:
                        return 16;
                    case VertexElementFormat.Rg32:
                        return 8;
                    case VertexElementFormat.NormalizedShort2:
                        return 4;
                    case VertexElementFormat.NormalizedShort4:
                        return 8;
                    case VertexElementFormat.Normalized101010:
                        return 4;
                    case VertexElementFormat.Short2:
                        return 4;
                    case VertexElementFormat.Short4:
                        return 8;
                    case VertexElementFormat.Byte4:
                        return 4;
                    case VertexElementFormat.UInt101010:
                        return 4;
                }
                throw new NotImplementedException(); //TODO: dunno
            }
        }
    }
}