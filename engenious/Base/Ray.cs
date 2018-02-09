using OpenTK.Graphics.ES20;

namespace engenious
{
    public struct Ray
    {
        public Vector3 Direction;
        public Vector3 Position;

        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        public float? Intersects(BoundingBox box)
        {
            return box.Intersects(this);   
        }

        public static bool operator ==(Ray ray1, Ray ray2)
        {
            
            bool posEqual = ray1.Position == ray2.Position;
                if (!posEqual)
                return false;
            //ray1.Direction.Normalized() == ray2.Direction.Normalized();
            // test if direction is the same
            var div = ray1.Direction * (ray2.Direction.X * ray2.Direction.Y * ray2.Direction.Z) / ray2.Direction;
            return div.X > 0 && (int)div.X == (int)div.Y && (int)div.Y == (int)div.Z;
        }

        public static bool operator !=(Ray ray1, Ray ray2)
        {
            return ray1.Position != ray2.Position || ray1.Direction.Normalized() != ray2.Direction.Normalized();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Direction.GetHashCode()*397) ^ Position.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Ray)
            {
                return (Ray)obj == this;
            }
            return false;
        }

        public override string ToString()
        {
            return $"[Position: {Position.ToString()},Direction: {Direction.ToString()}]";
        }
    }
}

