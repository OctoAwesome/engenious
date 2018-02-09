namespace engenious.Graphics
{
    public struct Viewport
    {
        public Viewport(int x, int y, int width, int height, float minDepth = 0.0f, float maxDepth = 1.0f)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        internal Viewport(System.Drawing.Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
			
        }

        public Viewport(Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {

        }

        public float AspectRatio => (float)Width / Height;

        public int Width{ get; internal set; }

        public int Height{ get; internal set; }

        public int X{ get; internal set; }

        public int Y{ get; internal set; }

        public float MaxDepth{ get; internal set; }

        public float MinDepth{ get; internal set; }

        public Rectangle Bounds
        {
            get{ return new Rectangle(X, Y, Width, Height); }
            internal set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Vector3 Unproject(Vector3 source, Matrix matrix)
        {
            matrix = Matrix.Invert(matrix);
            source.X = (((source.X - X) / Width) * 2f) - 1f;
            source.Y = -((((source.Y - Y) / Height) * 2f) - 1f);
            source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
            var vector = Vector3.Transform(source, matrix);
            var a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            //float a = (((source.X * matrix.M41) + (source.Y * matrix.M42)) + (source.Z * matrix.M43)) + matrix.M44;
            vector.X = vector.X / a;
            vector.Y = vector.Y / a;
            vector.Z = vector.Z / a;

            return vector;
        }

        public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            return Unproject(source, projection * view * world);
        }

        public Vector3 Project(Vector3 source, Matrix matrix)
        {

            var vector = Vector3.Transform(source, matrix);
            var a = (((source.X * matrix.M41) + (source.Y * matrix.M42)) + (source.Z * matrix.M43)) + matrix.M44;

            vector.X = vector.X / a;
            vector.Y = vector.Y / a;
            vector.Z = vector.Z / a;

            vector.X = (((vector.X + 1f) * 0.5f) * Width) + X;
            vector.Y = (((-vector.Y + 1f) * 0.5f) * Height) + Y;
            vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;
            return vector;
        }

        public Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            return Project(source, world * view * projection);

        }

        public override string ToString()
        {
            return string.Format("[Viewport: X={0}, Y={1}, Width={2}, Height={3}]", X, Y, Width, Height);
        }

        /*public override bool Equals (object obj)
		{
			if (!(obj is Viewport))
				return false;
			Viewport v2 = (Viewport)obj;
			return this == v2;
		}

		public static bool operator==(Viewport v1,Viewport v2){
			return v1.X == v2.X && v1.Y == v2.Y && v1.Width == v2.Width && v1.Height == v2.Height;
		}
		public static bool operator!=(Viewport v1,Viewport v2){
			return v1.X != v2.X || v1.Y != v2.Y || v1.Width != v2.Width ||  v1.Height != v2.Height;
		}*/

    }
			
}