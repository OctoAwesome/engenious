namespace engenious
{
    public interface IDrawable
    {
        bool Visible{ get; }

        int DrawOrder{ get; }

        void Draw(GameTime gameTime);
    }
}

