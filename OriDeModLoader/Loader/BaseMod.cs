using System.Collections.Generic;
using System.Linq;
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
        
        List<SettingsScreenConfig> GetSettings();
    }

    public abstract class BaseMod: IMod
    {
        public virtual void Init() { }
        public virtual void Unload() { }

        public abstract string Name { get; }
        public abstract string ModID { get; }
        public abstract string Version { get; }
        
        public virtual void FixedUpdate() { }
        public virtual List<SettingsScreenConfig> GetSettings() => new List<SettingsScreenConfig>();

        protected SettingsScreenConfig DefaultSettingsScreen(params SettingBase[] settings)
        {
            return this.SettingsScreen(this.Name, settings);
        }
        
        protected SettingsScreenConfig SettingsScreen(string name, params SettingBase[] settings)
        {
            return new SettingsScreenConfig
            {
                name = name,
                settings = settings.ToList()
            };
        }
        
        
        
    }
}
