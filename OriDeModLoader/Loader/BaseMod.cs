namespace OriDeModLoader
{
    public interface IMod
    {
        void Init();
        void Unload();

        string Name { get; }
    }
}
