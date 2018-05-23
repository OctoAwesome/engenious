using System;
using engenious;

namespace System.Drawing
{
	public static class ColorExt
	{
		public static Vector4 ToVector4(this Color color)
		{
			return new Vector4 (color.R/255f, color.G/255f, color.B/255f, color.A/255f);
		}
		public static Vector3 ToVector3(this Color color)
		{
			return new Vector3 (color.R/255f, color.G/255f, color.B/255f);
		}
	}
}

