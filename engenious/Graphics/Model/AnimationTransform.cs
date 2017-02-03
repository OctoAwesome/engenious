namespace engenious
{
    public class AnimationTransform
    {
        public AnimationTransform(string name,Vector3 location, Vector3 scale, Quaternion quaternion)
        {
            //dubi += name + ": " + quaternion.ToString() + "\n";
            Location = location;
            if (name.Contains("$")){
                Location = new Vector3(location.X,location.Z,location.Y);
            }
            Scale = scale;
            Rotation = quaternion;
        }
        public Quaternion Rotation{get;private set;}
        public Vector3 Location{get;private set;}
        public Vector3 Scale{get;private set;}

        public Matrix ToMatrix()
        {
            Matrix res = Matrix.CreateFromQuaternion(Rotation.X, Rotation.Y, Rotation.Z, Rotation.W);
            res.M11 *= Scale.X;
            res.M12 *= Scale.X;
            res.M13 *= Scale.X;
            res.M21 *= Scale.Y;
            res.M22 *= Scale.Y;
            res.M23 *= Scale.Y;
            res.M31 *= Scale.Z;
            res.M32 *= Scale.Z;
            res.M33 *= Scale.Z;
            res.M41 = Location.X;
            res.M42 = Location.Y;
            res.M43 = Location.Z;
            return res;
        }

        public static AnimationTransform Lerp(AnimationTransform transform1,AnimationTransform transform2,float amount)
        {
            return new AnimationTransform("",Vector3.Lerp(transform1.Location,transform2.Location,amount),
                                            Vector3.Lerp(transform1.Scale,transform2.Scale,amount),
                                            Quaternion.Lerp(transform1.Rotation,transform2.Rotation,amount));
        }
        public static AnimationTransform operator +(AnimationTransform t1,AnimationTransform t2)
        {
            return new AnimationTransform("",t1.Location+t2.Location,t1.Scale*t2.Scale,t1.Rotation*t2.Rotation);
        }
        public static AnimationTransform Transform(AnimationTransform t,Matrix transformation)
        {
            return new AnimationTransform("",Vector3.Transform(t.Location,transformation),
                t.Scale,
                t.Rotation);
        }
    }
}

