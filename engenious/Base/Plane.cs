namespace engenious
{
    public struct Plane
    {
        public float D;
        public Vector3 Normal;

        public Plane(float a, float b, float c, float d)
        {
            Normal = new Vector3(a, b, c);
            var len = Normal.Length;
            D = (d / len);
            Normal /= len;
        }

        public float DistanceToPoint(Vector3 pt)
        {
            return Normal.Dot(pt) + D;
        }
    }
}

