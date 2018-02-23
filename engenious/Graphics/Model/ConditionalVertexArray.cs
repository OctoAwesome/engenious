using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    public class ConditionalVertexArray
    {
        private Array _shit;
        private Type _type;

        public ConditionalVertexArray(int size, bool hasPositions, bool hasColors, bool hasNormals,
            bool hasTextureCoordinates)
        {
            if (!hasPositions)
                throw new ArgumentException("vertices without positions not known");
            if (hasColors)
            {
                if (hasNormals)
                {
                    if (hasTextureCoordinates)
                    {
                        SetVertices(new VertexPositionNormalColorTexture[size]);
                    }
                    else
                    {
                        SetVertices(new VertexPositionNormalColor[size]);
                    }
                }
                else
                {
                    if (hasTextureCoordinates)
                    {
                        SetVertices(new VertexPositionColorTexture[size]);
                    }
                    else
                    {
                        SetVertices(new VertexPositionColor[size]);
                    }
                }
            }
            else
            {
                if (hasNormals)
                {
                    if (hasTextureCoordinates)
                    {
                        SetVertices(new VertexPositionNormalTexture[size]);
                    }
                    else
                    {
                        SetVertices(new VertexPositionNormal[size]);
                    }
                }
                else
                {
                    if (hasTextureCoordinates)
                    {
                        SetVertices(new VertexPositionTexture[size]);
                    }
                    else
                    {
                        SetVertices(new VertexPosition[size]);
                    }
                }
            }
        }

        public T[] GetVertices<T>() where T : struct, IVertexType
        {
            return (T[]) _shit;
        }

        public Array GetVertices()
        {
            return _shit;
        }

        public void SetVertices<T>(T[] vertices) where T : struct, IVertexType
        {
            SetVertices(vertices,vertices.Length > 0 ? vertices[0].VertexDeclaration : (VertexDeclaration)typeof(T).GetField("VertexDeclaration", BindingFlags.Public | BindingFlags.Static)?.GetValue(null));
        }

        public void SetVertices<T>(T[] vertices, VertexDeclaration vertexDeclaration) where T : struct, IVertexType
        {
            _shit = vertices;

            var type = typeof(T);
            _type = type;

            VertexDeclaration = vertexDeclaration;
            
            
            HasPositions = typeof(IPositionVertex).IsAssignableFrom(type);
            HasColors = typeof(IColorVertex).IsAssignableFrom(type);
            HasNormals = typeof(INormalVertex).IsAssignableFrom(type);
            HasTextureCoordinates = typeof(ITextureCoordinatesVertex).IsAssignableFrom(type);
            
            
            AsPosition = HasPositions ? new GenericIndexer<T,IPositionVertex,Vector3>(vertices) : null;
            AsColor = HasColors ? new GenericIndexer<T,IColorVertex,Color>(vertices) : null;
            AsNormal = HasNormals ? new GenericIndexer<T,INormalVertex,Vector3>(vertices) : null;
            AsTextureCoordinate = HasTextureCoordinates ? new GenericIndexer<T,ITextureCoordinatesVertex,Vector2>(vertices) : null;
        }
        public VertexDeclaration VertexDeclaration { get; private set; }
        public int Length => _shit.Length;

        public bool HasPositions { get; private set; }
        public bool HasColors { get; private set; }
        public bool HasNormals { get; private set; }
        public bool HasTextureCoordinates { get; private set; }

        public IGenericIndexer<Vector3> AsPosition { get; private set; }
        public IGenericIndexer<Color> AsColor { get; private set; }
        public IGenericIndexer<Vector3> AsNormal { get; private set; }
        public IGenericIndexer<Vector2> AsTextureCoordinate { get; private set; }
    }

    public interface IGenericIndexer<T>
    {
        T this[int index] { get; set; }
    }
    public class GenericIndexer<T,TU,TUu> : IGenericIndexer<TUu>
    {
        private readonly T[] _array;

        public GenericIndexer(T[] array)
        {
            _array = array;
        }

        static GenericIndexer()
        {
            if (!typeof(TU).IsAssignableFrom(typeof(T)))
                throw new ArgumentException("types not compatible");

            PropertyInfo propInfo = null;
            var retType = typeof(TUu);
            var props = typeof(TU).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (props.Length > 2)
                throw new ArgumentException("type not supported");
            foreach (var prop in props)
            {
                if (prop.Name != "VertexDeclaration" && retType == prop.PropertyType)
                    propInfo = prop;
            }
            if (propInfo == null)
                throw new ArgumentException("type not supported");
            
            
            
            
            var arrayParam = Expression.Parameter(typeof(T[]), "array");
            var indexParam = Expression.Parameter(typeof(int), "index");
            var valueParam = Expression.Parameter(retType, "value");
            var arrayAccess = Expression.ArrayAccess(arrayParam, indexParam);
            

            GetValue = Expression.Lambda<Func<T[],int,TUu>>(Expression.Property(Expression.Convert(arrayAccess, typeof(TU)),propInfo),arrayParam,indexParam).Compile();
            SetValue = Expression.Lambda<Action<T[], int, TUu>>(Expression.Assign(Expression.Property(arrayAccess,propInfo), valueParam), arrayParam,
                indexParam, valueParam).Compile();
        }

        private static readonly Func<T[],int,TUu> GetValue;
        private static readonly Action<T[],int,TUu> SetValue;

        public TUu this[int index]
        {
            get => GetValue(_array,index);
            set => SetValue(_array, index, value);
        }
    }
}