using System.Collections.Generic;
using BaseModLib;
using BFModLoader.ModLoader;

namespace OriDeModLoader.UIExtensions
{
    public class SettingsListOptionScreen : CustomOptionsScreen
    {
        private List<SettingBase> settings;

        public SettingsListOptionScreen()
        {
            this.settings = new List<SettingBase>();
        }

        public void Init(List<SettingBase> settings)
        {
            this.settings = settings;
        }

        public override void InitScreen()
        {
            foreach (var setting in settings)
            {
                switch (setting)
                {
                    case BoolSetting bs:
                        AddToggle(bs);
                        break;
                    case FloatSetting fs:
                        AddSlider(fs);
                        break;
                    case KeybindSetting ks:
                        AddKeybind(ks);
                        break;
                }
            }
        }
    }
}
