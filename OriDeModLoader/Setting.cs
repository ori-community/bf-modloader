﻿using System;

namespace BaseModLib
{
    public abstract class SettingBase
    {
        /// <param name="id">If null, the setting won't be saved to or read from file</param>
        public SettingBase(string id)
        {
            ID = id;
        }

        public abstract void Parse(string value);

        public new abstract string ToString();

        public abstract void Reset();

        public string ID;
    }

    public abstract class Setting<T> : SettingBase
    {
        public Setting(string id, T defaultValue) : base(id)
        {
            if (!string.IsNullOrEmpty(id) && id.Contains(":"))
                throw new ArgumentException("Setting id cannot contain the following characters: \":\"", nameof(id));

            Default = defaultValue;
            Value = Default;

            if (!string.IsNullOrEmpty(id))
            {
                string savedValue = SettingsFile.GetValue(id);
                if (savedValue != null)
                    Parse(savedValue); // TODO handle error
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override void Reset()
        {
            Value = Default;
        }

        public event Action<T> OnValueChanged;

        public T Default { get; }

        protected T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public class BoolSetting : Setting<bool>
    {
        public BoolSetting(string id, bool defaultValue) : base(id, defaultValue) { }

        public override void Parse(string value)
        {
            _value = bool.Parse(value);
        }
    }

    public class FloatSetting : Setting<float>
    {
        public FloatSetting(string id, float defaultValue) : base(id, defaultValue) { }

        public override void Parse(string value)
        {
            _value = float.Parse(value);
        }
    }

    public class IntSetting : Setting<int>
    {
        public IntSetting(string id, int defaultValue) : base(id, defaultValue) { }

        public override void Parse(string value)
        {
            _value = int.Parse(value);
        }
    }
}
