namespace engenious
{
    public interface IUpdateable
    {
        int UpdateOrder { get; }

        bool Enabled { get; }

        void Update(GameTime gameTime);
    }
}

