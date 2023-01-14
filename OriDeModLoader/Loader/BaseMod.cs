namespace OriDeModLoader
{
    public interface IMod
    {
        void Init();
        void Unload();

        string Name { get; }

        string ModID { get; }
        string Version { get; }
    }
}
