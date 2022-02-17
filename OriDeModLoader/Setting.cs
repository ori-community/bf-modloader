namespace BaseModLib
{
    public abstract class SettingBase
    {
        public SettingBase(string name, bool nag = true)
        {
            this.Name = name;
            //RandomizerSettings.All[name] = this;

            this.Nag = nag;
        }

        public abstract void Parse(string value);

        public abstract new string ToString();

        public abstract void Reset();

        public string Name;

        public bool Nag;
    }

    public abstract class Setting<T> : SettingBase
    {
        public Setting(string name, T defaultValue, bool nag = true) : base(name, nag)
        {
            this.Default = defaultValue;
            this.Value = this.Default;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override void Reset()
        {
            this.Value = this.Default;
        }

        public static implicit operator T(Setting<T> setting) => setting.Value;

        public T Default;

        public T Value;
    }

    public class BoolSetting : Setting<bool>
    {
        public BoolSetting(string name, bool defaultValue, bool nag = true) : base(name, defaultValue, nag) { }

        public override void Parse(string value)
        {
            this.Value = bool.Parse(value);
        }
    }

    public class FloatSetting : Setting<float>
    {
        public FloatSetting(string name, float defaultValue, bool nag = true) : base(name, defaultValue, nag) { }

        public override void Parse(string value)
        {
            this.Value = float.Parse(value);
        }
    }
}
