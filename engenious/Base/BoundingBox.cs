using System;
using System.Collections.Generic;

namespace engenious
{
    public struct BoundingBox
    {
        public Vector3 Max;
        public Vector3 Min;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public BoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
        }

        public bool Contains(Vector3 vec)
        {
            return vec.X >= Min.X && vec.X < Max.X &&
            vec.Y >= Min.Y && vec.Y < Max.Y &&
            vec.Z >= Min.Z && vec.Z < Max.Z;
        }

        public bool Contains(BoundingBox box)
        {
            return Contains(box.Min) && Contains(box.Max);
        }

        public bool Intersects(BoundingBox box)
        {
            throw new NotImplementedException();//TODO: return (this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)
        }
        public float? Intersects(Ray ray)
        {
            const float epsilon = 1e-6f;

            float? tMin = null, tMax = null;

            if (Math.Abs(ray.Direction.X) < epsilon)
            {
                if (ray.Position.X < Min.X || ray.Position.X > Max.X)
                    return null;
            }
            else
            {
                tMin = (Min.X - ray.Position.X) / ray.Direction.X;
                tMax = (Max.X - ray.Position.X) / ray.Direction.X;

                if (tMin > tMax)
                {
                    var temp = tMin;
                    tMin = tMax;
                    tMax = temp;
                }
            }

            if (Math.Abs(ray.Direction.Y) < epsilon)
            {
                if (ray.Position.Y < Min.Y || ray.Position.Y > Max.Y)
                    return null;
            }
            else
            {
                var tMinY = (Min.Y - ray.Position.Y) / ray.Direction.Y;
                var tMaxY = (Max.Y - ray.Position.Y) / ray.Direction.Y;

                if (tMinY > tMaxY)
                {
                    var temp = tMinY;
                    tMinY = tMaxY;
                    tMaxY = temp;
                }

                if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
                    return null;

                if (!tMin.HasValue || tMinY > tMin)
                    tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax)
                    tMax = tMaxY;
            }

            if (Math.Abs(ray.Direction.Z) < epsilon)
            {
                if (ray.Position.Z < Min.Z || ray.Position.Z > Max.Z)
                    return null;
            }
            else
            {
                var tMinZ = (Min.Z - ray.Position.Z) / ray.Direction.Z;
                var tMaxZ = (Max.Z - ray.Position.Z) / ray.Direction.Z;

                if (tMinZ > tMaxZ)
                {
                    var temp = tMinZ;
                    tMinZ = tMaxZ;
                    tMaxZ = temp;
                }

                if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
                    return null;

                if (!tMin.HasValue || tMinZ > tMin)
                    tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax)
                    tMax = tMaxZ;
            }

            // having a positive tMin and a negative tMax means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if ((tMin.HasValue && tMin < 0) && tMax > 0)
                return 0;

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (tMin < 0)
                return null;

            return tMin;
            /*float txmin = (Min.X - ray.Position.X) / ray.Direction.X;
            float txmax = (Max.X - ray.Position.X) / ray.Direction.X;

            if (txmin > txmax)
                swap(ref txmin, ref txmax);

            float tymin = (Min.Y - ray.Position.Y) / ray.Direction.Y;
            float tymax = (Max.Y - ray.Position.Y) / ray.Direction.Y;

            if (tymin > tymax)
                swap(ref tymin, ref tymax);

            if ((txmin > tymax) || (tymin > txmax))
                return null;

            if (tymin > txmin)
                txmin = tymin;

            if (tymax < txmax)
                txmax = tymax;

            float tzmin = (Min.Z - ray.Position.Z) / ray.Direction.Z;
            float tzmax = (Max.Z - ray.Position.Z) / ray.Direction.Z;

            if (tzmin > tzmax)
                swap(ref tzmin, ref tzmax);

            if ((txmin > tzmax) || (tzmin > txmax))
                return null;

            if (tzmin > txmin)
                txmin = tzmin;

            if (tzmax < txmax)
                txmax = tzmax;

            return (float)Math.Sqrt(txmin * txmin + tymin * tymin + tzmin * tzmin);*/ //TODO: verify?
        }

        public Vector3[] GetCorners()
        {
            return new[]
            { new Vector3(Min.X, Max.Y, Max.Z), Max, new Vector3(Max.X, Min.Y, Max.Z), new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z), Max, new Vector3(Max.X, Min.Y, Min.Z), new Vector3(Min.X, Min.Y, Min.Z),
            };//TODO: verify?
        }

        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                                           
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }
            return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public static void CreateMerged(ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
        {
            result = new BoundingBox(Math.Min(original.Min.X, additional.Min.X),
                Math.Min(original.Min.Y, additional.Min.Y),
                Math.Min(original.Min.Z, additional.Min.Z),
                Math.Max(original.Max.X, additional.Max.X),
                Math.Max(original.Max.Y, additional.Max.Y),
                Math.Max(original.Max.Z, additional.Max.Z));
        }

        public static BoundingBox CreateMerged(params BoundingBox[] boundingBoxes)
        {
            return CreateMerged((IEnumerable<BoundingBox>)boundingBoxes);
        }

        public static BoundingBox CreateMerged(IEnumerable<BoundingBox> boundingBoxes)
        {
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (var box in boundingBoxes)
            {
                minX = Math.Min(box.Min.X, minX);
                minY = Math.Min(box.Min.Y, minY);
                minZ = Math.Min(box.Min.Z, minZ);
                                    
                maxX = Math.Max(box.Max.X, maxX);
                maxY = Math.Max(box.Max.Y, maxY);
                maxZ = Math.Max(box.Max.Z, maxZ);
            }
            return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
        }
    }
}

