using System.Collections.Generic;
using BaseModLib;

namespace OriDeModLoader
{
    public interface IMod
    {
        void Init();
        void Unload();

        void FixedUpdate();

        string Name { get; }
        string ModID { get; }
        string Version { get; }
        
        List<SettingBase> GetSettings();
    }

    public abstract class BaseMod: IMod
    {
        public virtual void Init() { }
        public virtual void Unload() { }

        public abstract string Name { get; }
        public abstract string ModID { get; }
        public abstract string Version { get; }
        
        public virtual void FixedUpdate() { }
        public virtual List<SettingBase> GetSettings() => new List<SettingBase>();
    }
}
