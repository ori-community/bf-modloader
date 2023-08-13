using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using Input = UnityEngine.Input;

namespace BaseModLib
{
    public struct SettingsScreenConfig
    {
        public string name;
        public List<SettingBase> settings;
    }

    public abstract class SettingBase
    {
        //if we want translations, I'd just turn those two into an identifier
        public SettingBase(string id, string label, string tooltip)
        {
            Id = id;
            Label = label;
            Tooltip = tooltip;
        }

        public abstract void Parse(string value);

        public new abstract string ToString();

        public abstract void Reset();

        public string Id;
        public string Label;
        public string Tooltip;
        public string ModName;
        public string ScreenName;
    }

    public abstract class Setting<T> : SettingBase
    {
        public Setting(string id, string label, string tooltip, T defaultValue) : base(id, label, tooltip)
        {
            if (id.Contains(":"))
                throw new ArgumentException("Setting id cannot contain the following characters: \":\"", nameof(id));

            Default = defaultValue;
            Value = Default;
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
        public BoolSetting(string id, string label, string tooltip, bool defaultValue) : base(id, label, tooltip, defaultValue) { }

        public override void Parse(string value)
        {
            _value = bool.Parse(value);
        }
    }

    public class FloatSetting : Setting<float>
    {
        public float Min;
        public float Max;
        public float Step;

        public FloatSetting(string id, float min, float max, float step, string label, string tooltip,
            float defaultValue) : base(id, label, tooltip, defaultValue)
        {
            Min = min;
            Max = max;
            Step = step;
        }

        public override void Parse(string value)
        {
            _value = float.Parse(value);
        }
    }

    public class KeybindSetting : Setting<IEnumerable<IEnumerable<KeyCode>>>
    {
        public KeybindSetting(string id, string label, string tooltip, KeyCode defaultValue) :
            this(id, label, tooltip, new[]{new[] {defaultValue}.AsEnumerable()})
        {
        }     public KeybindSetting(string id, string label, string tooltip, IEnumerable<KeyCode> defaultValue) :
            this(id, label, tooltip, new[]{defaultValue})
        {
        }

        public KeybindSetting(string id, string label, string tooltip, IEnumerable<IEnumerable<KeyCode>> defaultValue) : base(id, label, tooltip, defaultValue)
        {
        }

        public override void Parse(string value)
        {
            string[] groups = value.Split(new[]
            {
                ", "
            }, StringSplitOptions.None);
            IEnumerable<IEnumerable<KeyCode>> binds = groups.Select(a => a.Split(new[]
            {
                "+"
            }, StringSplitOptions.None).Select(code => (KeyCode)(Enum.Parse(typeof(KeyCode), code))));
            _value = binds;
        }
        public override string ToString()
        {
            return String.Join(",",
                Value.Select(group 
                    => String.Join("+", group.Select(key => key.ToString()).ToArray())).ToArray());
        }

        public bool IsPressed()
        {
            return Value.Any(bind => bind.All(Input.GetKey));
        }

        public bool IsJustPressed()
        {
            return Value.Any(bind => bind.All(Input.GetKey) && bind.Any(Input.GetKeyDown));
        }
    }
}
