using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace engenious.Content.Pipeline
{
    [Serializable]
    [TypeConverter(typeof(CustomExpandableObjectConverter))]
    public class ProcessorSettings
    {
        public override string ToString()
        {
            return string.Empty;
        }

        #region Serialization
        private static void SetPrimitive(PropertyDescriptor property,object obj,string val)
        {
            var code = Type.GetTypeCode(property.PropertyType);
            if (code != TypeCode.String && val == null)
                return;
            switch(code)
            {
                case TypeCode.String:
                    property.SetValue(obj,val);
                    break;
                case TypeCode.Char:
                    property.SetValue(obj,string.IsNullOrEmpty(val) ? '\0' : val[0]);
                    break;
                case TypeCode.SByte:
                    {
                        sbyte num;
                        if (sbyte.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Int16:
                    {
                        short num;
                        if (short.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Int32:
                    {
                        int num;
                        if (int.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Int64:
                    {
                        long num;
                        if (long.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Byte:
                    {
                        byte num;
                        if (byte.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.UInt16:
                    {
                        ushort num;
                        if (ushort.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.UInt32:
                    {
                        uint num;
                        if (uint.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.UInt64:
                    {
                        ulong num;
                        if (ulong.TryParse(val,NumberStyles.Integer,CultureInfo.InvariantCulture,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Boolean:
                    {
                        bool tmp;
                        if (bool.TryParse(val,out tmp))
                            property.SetValue(obj,tmp);
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        DateTime dt;
                        if (DateTime.TryParse(val,out dt))
                            property.SetValue(obj,dt);
                    }
                    break;
                case TypeCode.Single:
                    {
                        float num;
                        if (float.TryParse(val,NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Double:
                    {
                        double num;
                        if (double.TryParse(val,NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
                case TypeCode.Decimal:
                    {
                        decimal num;
                        if (decimal.TryParse(val,NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat,out num))
                        {
                            property.SetValue(obj,num);
                        }
                    }
                    break;
            }
        }
        private static void ReadObject(XElement nodes,object obj)
        {

            var props = TypeDescriptor.GetProperties(obj).OfType<PropertyDescriptor>().ToDictionary(x => x.Name, x => x);
            foreach (var setting in nodes.Elements())
            {
                PropertyDescriptor property;
                if (props.TryGetValue(setting.Name.LocalName, out property))
                {
                    
                    var val = setting.Nodes().OfType<XText>().FirstOrDefault()?.Value;
                    if (val == null)
                        continue;
                    if (property.PropertyType.IsPrimitive)
                    {
                        SetPrimitive(property,obj,val);
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        try{
                            property.SetValue(obj,Enum.Parse(property.PropertyType,val));
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    else
                    {
                        var tmp =Activator.CreateInstance(property.PropertyType);
                        ReadObject(setting,tmp);
                        property.SetValue(obj,tmp);
                    }
                }
            }
        }

        public virtual void Read(XElement nodes)
        {
            ReadObject(nodes,this);
        }
        private static string PrimitiveToString(object obj)
        {
            var code = Type.GetTypeCode(obj.GetType());
            switch(code)
            {
                case TypeCode.String:
                    return (string)obj;
                case TypeCode.Boolean:
                case TypeCode.Char:
                    return obj.ToString();
                case TypeCode.DateTime:
                    return ((DateTime)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return ((decimal)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return ((double)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return ((float)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Int16:
                    return ((short)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Int32:
                    return ((int)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Int64:
                    return ((long)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.UInt16:
                    return ((ushort)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.UInt32:
                    return ((uint)obj).ToString(CultureInfo.InvariantCulture);
                case TypeCode.UInt64:
                    return ((ulong)obj).ToString(CultureInfo.InvariantCulture);
            }
            return obj.ToString();
        }

        private static void WriteObject(XElement writer, object obj)
        {
            foreach (var prop in TypeDescriptor.GetProperties(obj).OfType<PropertyDescriptor>())
            {
                var type = prop.PropertyType;
                if (prop.IsReadOnly)
                    continue;
                if (type.IsPrimitive || type.IsEnum)
                {
                    writer.Add(new XElement(prop.Name, PrimitiveToString(prop.GetValue(obj))));
                }
                else
                {
                    var el = new XElement(prop.Name);
                    WriteObject(el, prop.GetValue(obj));
                    writer.Add(el);
                }
            }
        }

        public virtual void Write(XElement writer)
        {
            WriteObject(writer, this);
        }

        #endregion
    }
}

