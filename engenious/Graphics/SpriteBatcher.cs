using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace engenious.Graphics
{
    internal class SpriteBatcher: IDisposable
    {
        public class BatchItemPool
        {
            private const int BatchCount = 256;
            private readonly Stack<BatchItem> _batchItems = new Stack<BatchItem>(BatchCount);

            public BatchItemPool()
            {
                for (var i = 0; i < BatchCount; i++)
                {
                    _batchItems.Push(new BatchItem());
                }
            }

            public void ReleaseBatch(BatchItem batch)
            {
                _batchItems.Push(batch);
            }
            public BatchItem AquireBatch(Texture2D texture, Vector2 position, RectangleF? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode)
            {
                var item = _batchItems.Count == 0 ? new BatchItem() : _batchItems.Pop();
                item.InitBatchItem(texture,position,sourceRectangle,color,rotation,origin,size,effects,layerDepth,sortMode);
                return item;
            }
            public BatchItem AquireBatch(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode)
            {
                var item = _batchItems.Count == 0 ? new BatchItem() : _batchItems.Pop();
                item.InitBatchItem(texture,position,sourceRectangle,color,rotation,origin,size,effects,layerDepth,sortMode);
                return item;
            }
        }
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public class BatchItem
        {
            internal Vector2 TexTopLeft;
            internal Vector2 TexBottomRight;
            internal Vector3[] Positions;
            internal Color Color;
            internal Texture2D Texture;
            internal float SortingKey;

            public BatchItem()
            {
                Positions = new Vector3[4];
            }

            private void InitBatchItem(Vector2 position, Color color, float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode, Vector4 tempText)
            {

                if ((effects & SpriteBatch.SpriteEffects.FlipVertically) != 0)
                {
                    TexTopLeft.X = tempText.X + tempText.Z;
                    TexBottomRight.X = tempText.X;
                }
                else
                {
                    TexTopLeft.X = tempText.X;
                    TexBottomRight.X = tempText.X + tempText.Z;
                } 
                if ((effects & SpriteBatch.SpriteEffects.FlipHorizontally) != 0)
                {
                    TexTopLeft.Y = tempText.Y + tempText.W;
                    TexBottomRight.Y = tempText.Y;
                }
                else
                {
                    TexTopLeft.Y = tempText.Y;
                    TexBottomRight.Y = tempText.Y + tempText.W;
                }

                Positions[0] = new Vector3(-origin.X, -origin.Y, layerDepth);
                Positions[1] = new Vector3(-origin.X + size.X, -origin.Y, layerDepth);
                Positions[2] = new Vector3(-origin.X, -origin.Y + size.Y, layerDepth);
                Positions[3] = new Vector3(-origin.X + size.X, -origin.Y + size.Y, layerDepth);

                if (rotation != 0.0f || ((rotation = rotation % (float)(Math.PI * 2)) != 0.0f))
                {
                    var cosR = (float)Math.Cos(rotation);//TODO: correct rotation
                    var sinR = (float)Math.Sin(rotation);
                    for (var i = 0; i < Positions.Length; i++)
                    {
                        Positions[i] = new Vector3(Positions[i].X * cosR - Positions[i].Y * sinR, Positions[i].Y * cosR + Positions[i].X * sinR, Positions[i].Z);
                    }
                }
                for (var i = 0; i < Positions.Length; i++)
                {
                    Positions[i] += new Vector3(origin.X + position.X, origin.Y + position.Y);
                }
                Color = color;

                switch (sortMode)
                {
                    case SpriteBatch.SpriteSortMode.BackToFront:
                        SortingKey = -layerDepth;
                        break;
                    case SpriteBatch.SpriteSortMode.FrontToBack:
                        SortingKey = layerDepth;
                        break;
                    case SpriteBatch.SpriteSortMode.Texture:
                        SortingKey = Texture.GetHashCode();
                        break;
                }
            }

            public void InitBatchItem(Texture2D texture, Vector2 position, RectangleF? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode)
            {
                Texture = texture;
                Vector4 tempText;
                if (sourceRectangle.HasValue)
                    tempText = new Vector4(sourceRectangle.Value.X, sourceRectangle.Value.Y, sourceRectangle.Value.Width, sourceRectangle.Value.Height);
                else
                    tempText = new Vector4(0, 0, 1, 1);

                InitBatchItem(position, color, rotation, origin, new Vector2(size.X * tempText.Z, size.Y * tempText.W), effects, layerDepth, sortMode, tempText);
            }

            public void InitBatchItem(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode)
            {
                Texture = texture;
                Vector4 tempText;
                if (sourceRectangle.HasValue)
                    tempText = new Vector4((float)sourceRectangle.Value.X / texture.Width, (float)sourceRectangle.Value.Y / texture.Height, (float)sourceRectangle.Value.Width / texture.Width, (float)sourceRectangle.Value.Height / texture.Height);
                else
                    tempText = new Vector4(0, 0, 1, 1);
                InitBatchItem(position, color, rotation, origin, new Vector2(size.X, size.Y), effects, layerDepth, sortMode, tempText);
               
            }
        }

        private readonly DynamicVertexBuffer _vertexBuffer;
        private readonly VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MaxBatch * 4];
        private readonly DynamicIndexBuffer _indexBuffer;
        private readonly ushort[] _indexData = new ushort[MaxBatch * 6];
        private const int MaxBatch = 256;
        private SpriteBatch.SpriteSortMode _sortMode;
        private readonly GraphicsDevice _graphicsDevice;
        public static BatchItemPool BatchPool = new BatchItemPool();

        private readonly List<BatchItem> _batches;

        public SpriteBatcher(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _batches = new List<BatchItem>(MaxBatch);
            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4 * MaxBatch);
            _indexBuffer = new DynamicIndexBuffer(graphicsDevice, DrawElementsType.UnsignedShort, 6 * MaxBatch);
        }

        public void Begin(SpriteBatch.SpriteSortMode sortMode)
        {
            foreach (var t in _batches)
                BatchPool.ReleaseBatch(t);

            _batches.Clear();
            _sortMode = sortMode;
        }

        internal SamplerState SamplerState;

        public void Flush(Effect effect, Texture texture, int batch, int batchCount)
        {
            if (batchCount == 0)
                return;


            //graphicsDevice.DrawPrimitives(OpenTK.Graphics.OpenGL4.PrimitiveType.Qu
            _vertexBuffer.SetData(VertexPositionColorTexture.VertexDeclaration.VertexStride * batch * 4, _vertexData, 0, batchCount * 4, VertexPositionColorTexture.VertexDeclaration.VertexStride);
            _indexBuffer.SetData(sizeof(short) * 6 * batch, _indexData, 0, batchCount * 6);


            _graphicsDevice.VertexBuffer = _vertexBuffer;
            _graphicsDevice.IndexBuffer = _indexBuffer;
            _graphicsDevice.Textures[0] = texture;
            //graphicsDevice.SamplerStates[0] = samplerState;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, batchCount * 4, 0, batchCount * 2);
            }

            _graphicsDevice.IndexBuffer = null;
            _graphicsDevice.VertexBuffer = null;
        }

        public void End(Effect effect)
        {
            if (_batches.Count == 0)
                return;



            if (_sortMode != SpriteBatch.SpriteSortMode.Immediate && _sortMode != SpriteBatch.SpriteSortMode.Deffered)
            {
                _batches.Sort((x, y) =>
                    {
                        var first = y.SortingKey.CompareTo(x.SortingKey);
                        return first;
                    });
            }


            //Array.Sort (_batches, 0, _batches.Count);
            Texture currentTexture = _batches.First().Texture;
            int batchStart = 0, batchCount = 0;

            ushort bufferIndex = 0;
            ushort indicesIndex = 0;
            foreach (var item in _batches)
            {
                if (item.Texture != currentTexture || batchCount == MaxBatch)
                {
                    Flush(effect, currentTexture, batchStart, batchCount);
                    currentTexture = item.Texture;
                    batchStart = batchCount = 0;
                    indicesIndex = bufferIndex = 0;
                }

                _indexData[indicesIndex++] = (ushort)(bufferIndex + 0);
                _indexData[indicesIndex++] = (ushort)(bufferIndex + 1);
                _indexData[indicesIndex++] = (ushort)(bufferIndex + 2);

                _indexData[indicesIndex++] = (ushort)(bufferIndex + 1);
                _indexData[indicesIndex++] = (ushort)(bufferIndex + 3);
                _indexData[indicesIndex++] = (ushort)(bufferIndex + 2);

                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[0], item.Color, item.TexTopLeft);
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[1], item.Color, new Vector2(item.TexBottomRight.X, item.TexTopLeft.Y));
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[2], item.Color, new Vector2(item.TexTopLeft.X, item.TexBottomRight.Y));
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[3], item.Color, item.TexBottomRight);
                batchCount++;
            }
            if (batchCount > 0)
                Flush(effect, currentTexture, batchStart, batchCount);
        }

        public void AddBatch(BatchItem batch)
        {
            _batches.Add(batch);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}

