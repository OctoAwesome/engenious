using System;
using System.Collections.Generic;
using engenious.Helper;

namespace engenious.Graphics
{
    public class PrimitiveModels
    {
        #region IcoSphere
        private static int AddVertex(Vector3 p,List<Vector3> vertices,ref int index)
        {
            var length = (float)Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            vertices.Add(new Vector3(p.X/length, p.Y/length, p.Z/length));
            return index++;
        }
        private static int GetMiddlePoint(int p1, int p2,List<Vector3> vertices, Dictionary<long, int> middlePointIndexCache,ref int index)
        {
            // first check if we have it already
            var firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            var key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            var point1 = vertices[p1];
            var point2 = vertices[p2];
            var middle = new Vector3(
                (point1.X + point2.X) / 2.0f, 
                (point1.Y + point2.Y) / 2.0f, 
                (point1.Z + point2.Z) / 2.0f);

            // add vertex makes sure point is on unit sphere
            var i = AddVertex(middle,vertices,ref index);

            // store it, return index
            middlePointIndexCache.Add(key, i);
            return i;
        }
        public static void CreateIcoSphere(int recursionLevel,ref List<Vector3> vertices,ref List<int> indices)
        {
            
            var middlePointIndexCache = new Dictionary<long, int>();
            var index = 0;

            // create 12 vertices of a icosahedron
            var t = (float)((1.0 + Math.Sqrt(5.0)) / 2.0);

            AddVertex(new Vector3(-1,  t),vertices,ref index);
            AddVertex(new Vector3( 1,  t),vertices,ref index);
            AddVertex(new Vector3(-1, -t),vertices,ref index);
            AddVertex(new Vector3( 1, -t),vertices,ref index);

            AddVertex(new Vector3( 0, -1,  t),vertices,ref index);
            AddVertex(new Vector3( 0,  1,  t),vertices,ref index);
            AddVertex(new Vector3( 0, -1, -t),vertices,ref index);
            AddVertex(new Vector3( 0,  1, -t),vertices,ref index);

            AddVertex(new Vector3( t,  0, -1),vertices,ref index);
            AddVertex(new Vector3( t,  0,  1),vertices,ref index);
            AddVertex(new Vector3(-t,  0, -1),vertices,ref index);
            AddVertex(new Vector3(-t,  0,  1),vertices,ref index);


            // create 20 triangles of the icosahedron
            var faces = new List<int>();

            // 5 faces around point 0
            faces.Add(0);faces.Add( 11);faces.Add( 5);
            faces.Add(0);faces.Add( 5);faces.Add( 1);
            faces.Add(0);faces.Add( 1);faces.Add( 7);
            faces.Add(0);faces.Add( 7);faces.Add(10);
            faces.Add(0);faces.Add( 10);faces.Add( 11);

            // 5 adjacent faces 
            faces.Add(1);faces.Add( 5);faces.Add( 9);
            faces.Add(5);faces.Add( 11);faces.Add( 4);
            faces.Add(11);faces.Add( 10);faces.Add( 2);
            faces.Add(10);faces.Add( 7);faces.Add( 6);
            faces.Add(7);faces.Add( 1);faces.Add( 8);

            // 5 faces around point 3
            faces.Add(3);faces.Add( 9);faces.Add( 4);
            faces.Add(3);faces.Add( 4);faces.Add( 2);
            faces.Add(3);faces.Add( 2);faces.Add( 6);
            faces.Add(3);faces.Add( 6);faces.Add( 8);
            faces.Add(3);faces.Add( 8);faces.Add( 9);

            // 5 adjacent faces 
            faces.Add(4);faces.Add( 9);faces.Add( 5);
            faces.Add(2);faces.Add( 4);faces.Add( 11);
            faces.Add(6);faces.Add( 2);faces.Add( 10);
            faces.Add(8);faces.Add( 6);faces.Add( 7);
            faces.Add(9);faces.Add( 8);faces.Add( 1);


            // refine triangles
            for (var i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<int>();
                for(var j=0;j<faces.Count;j+=3)
                {
                    // replace triangle by 4 triangles
                    var a = GetMiddlePoint(faces[j+0], faces[j+1],vertices,middlePointIndexCache,ref index);
                    var b = GetMiddlePoint(faces[j+1], faces[j+2],vertices,middlePointIndexCache,ref index);
                    var c = GetMiddlePoint(faces[j+2], faces[j+0],vertices,middlePointIndexCache,ref index);

                    faces2.Add(faces[j+0]);faces2.Add( a);faces2.Add( c);
                    faces2.Add(faces[j+1]);faces2.Add( b);faces2.Add( a);
                    faces2.Add(faces[j+2]);faces2.Add( c);faces2.Add(b);
                    faces2.Add(a);faces2.Add( b);faces2.Add( c);
                }
                faces = faces2;
            }

            indices.AddRange(faces);   
        }
        #endregion
        #region UVSphere
        // ReSharper disable once InconsistentNaming
        public static void CreateUVSphere(out Vector3[] vertices,out int[] indices,int nbLong=24,int nbLat=16)
        {
            var radius = 1f;

            vertices = new Vector3[(nbLong+1) * nbLat + 2];
            var _pi = (float)Math.PI;
            var _2pi = MathHelper.TwoPi;

            vertices[0] = Vector3.UnitZ * radius;
            for( var lat = 0; lat < nbLat; lat++ )
            {
                var a1 = _pi * (lat+1) / (nbLat+1);
                var sin1 = (float)Math.Sin(a1);
                var cos1 = (float)Math.Cos(a1);

                for( var lon = 0; lon <= nbLong; lon++ )
                {
                    var a2 = _2pi * (lon == nbLong ? 0 : lon) / nbLong;
                    var sin2 = (float)Math.Sin(a2);
                    var cos2 = (float)Math.Cos(a2);

                    vertices[ lon + lat * (nbLong + 1) + 1] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * radius;
                }
            }
            vertices[vertices.Length-1] = Vector3.UnitZ * -radius;

            var nbFaces = vertices.Length;
            var nbTriangles = nbFaces * 2;
            var nbIndexes = nbTriangles * 3;
            indices = new int[ nbIndexes ];

            //Top Cap
            var i = 0;
            for( var lon = 0; lon < nbLong; lon++ )
            {
                indices[i++] = lon+2;
                indices[i++] = lon+1;
                indices[i++] = 0;
            }

            //Middle
            for( var lat = 0; lat < nbLat - 1; lat++ )
            {
                for( var lon = 0; lon < nbLong; lon++ )
                {
                    var current = lon + lat * (nbLong + 1) + 1;
                    var next = current + nbLong + 1;

                    indices[i++] = current;
                    indices[i++] = current + 1;
                    indices[i++] = next + 1;

                    indices[i++] = current;
                    indices[i++] = next + 1;
                    indices[i++] = next;
                }
            }

            //Bottom Cap
            for( var lon = 0; lon < nbLong; lon++ )
            {
                indices[i++] = vertices.Length - 1;
                indices[i++] = vertices.Length - (lon+2) - 1;
                indices[i++] = vertices.Length - (lon+1) - 1;
            }
        }
        #endregion
    }
}

