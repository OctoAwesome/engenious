using System;
using System.ComponentModel;
using System.Globalization;

namespace engenious
{
    public class Vector2Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s == null)
                return base.ConvertFrom(context, culture, value);

            string val = s.Trim();
            if (val.StartsWith("[") && val.EndsWith("]"))
                val = val.Substring(1,val.Length-2);
            string[] splt = val.Split(new[]{','},2,StringSplitOptions.RemoveEmptyEntries);
            float x,y;
            if (float.TryParse(splt[0].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out x) && float.TryParse(splt[1].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out y))
            {
                return new Vector2(x,y);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    public class Vector3Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s == null)
                return base.ConvertFrom(context, culture, value);
            string val = s.Trim();
            if (val.StartsWith("[") && val.EndsWith("]"))
                val = val.Substring(1,val.Length-2);
            string[] splt = val.Split(new[]{','},3,StringSplitOptions.RemoveEmptyEntries);
            float x,y,z;
            if (float.TryParse(splt[0].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out x) && float.TryParse(splt[1].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out y) && float.TryParse(splt[2].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out z))
            {
                return new Vector3(x,y,z);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    public class Vector4Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s == null)
                return base.ConvertFrom(context, culture, value);
            string val = s.Trim();
            if (val.StartsWith("[") && val.EndsWith("]"))
                val = val.Substring(1,val.Length-2);
            string[] splt = val.Split(new[]{','},4,StringSplitOptions.RemoveEmptyEntries);
            float x,y,z,w;
            if (float.TryParse(splt[0].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out x) && float.TryParse(splt[1].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out y) && float.TryParse(splt[2].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out z) && float.TryParse(splt[3].Trim(),NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out w))
            {
                return new Vector4(x,y,z,w);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    public class MatrixConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
    }
}

