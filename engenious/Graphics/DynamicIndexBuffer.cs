using System;

namespace engenious.Graphics
{
	public class DynamicIndexBuffer : IndexBuffer
	{
		public DynamicIndexBuffer (GraphicsDevice graphicsDevice, Type indexType, int indexCount)
			: base (graphicsDevice, indexType, indexCount, BufferUsageHint.DynamicDraw)
		{

		}

		public DynamicIndexBuffer (GraphicsDevice graphicsDevice, DrawElementsType indexElementSize, int indexCount)
			: base (graphicsDevice, indexElementSize, indexCount, BufferUsageHint.DynamicDraw)
		{
		}
	}
}

