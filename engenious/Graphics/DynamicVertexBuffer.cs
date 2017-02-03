using System;

namespace engenious.Graphics
{
	public class DynamicVertexBuffer:VertexBuffer
	{
		public DynamicVertexBuffer (GraphicsDevice graphicsDevice, Type vertexType, int vertexCount)
			: base (graphicsDevice, vertexType, vertexCount, BufferUsageHint.DynamicDraw)
		{

		}

		public DynamicVertexBuffer (GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount)
			: base (graphicsDevice, vertexDeclaration, vertexCount, BufferUsageHint.DynamicDraw)
		{
		}
	}
}

