namespace engenious.Graphics
{
	public class FontCharacter
	{
		public FontCharacter (char character, Rectangle textureSize, Rectangle textureRegionPx, Vector2 offset, float advance)
		{
			Character = character;
			TextureRegion = new RectangleF ((float)textureRegionPx.X / textureSize.Width, (float)textureRegionPx.Y / textureSize.Height, (float)textureRegionPx.Width / textureSize.Width, (float)textureRegionPx.Height / textureSize.Height);
			Offset = offset;
			Advance = advance;
		}

		public FontCharacter (char character, RectangleF textureRegion, Vector2 offset, float advance)
		{
			Character = character;
			TextureRegion = textureRegion;
			Offset = offset;
			Advance = advance;
		}

		public char Character{ get; private set; }

		public RectangleF TextureRegion{ get; private set; }

		public Vector2 Offset{ get; private set; }

		public float Advance{ get; private set; }
	}
}

