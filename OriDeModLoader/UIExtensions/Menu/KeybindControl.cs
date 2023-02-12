using System;
using System.Collections.Generic;
using System.Linq;
using BaseModLib;
using UnityEngine;

namespace OriDeModLoader.UIExtensions
{
    public class KeybindControl : MonoBehaviour
    {
        private void Awake()
        {
            messageBox = base.transform.Find("text/stateText").GetComponent<MessageBox>();
        }

        public void BeginEditing()
        {
            currentKeys.Clear();
            currentKeys.AddRange(GetKeys().Select(it => it.ToList()));
            SuspensionManager.SuspendAll();
            editing = true;
            exit = 0;
            tooltipProvider.SetMessage("Backspace: remove bind\n<icon>D</>: finish editing");
            owner.tooltipController.UpdateTooltip();
        }

        public void Update()
        {
            if (!editing)
            {
                return;
            }

            if (exit < 2)
            {
                exit++;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (in_bind)
                {
                    in_bind = false;
                    currentKeys.Add(currentMultibind);
                    currentMultibind = new List<KeyCode>();
                }else if (currentKeys.Count > 0){
                    editing = false;
                    in_bind = false;
                    SuspensionManager.ResumeAll();
                    SetKeys(currentKeys.Select(bind => bind.ToArray()).ToArray());
                    PlayerInputRebinding.WriteKeyRebindSettings();
                    PlayerInput.Instance.RefreshControlScheme();
                    tooltipProvider.SetMessage("<icon>D</>: add or remove binds");
                    owner.tooltipController.UpdateTooltip();
                }
            } else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (currentKeys.Count > 0)
                {
                    currentKeys.RemoveAt(currentKeys.Count - 1);
                    UpdateMessageBox();
                    return;
                }
            }
            else if (Input.anyKeyDown)
            {
                foreach (object obj in Enum.GetValues(typeof(KeyCode)))
                {
                    KeyCode keyCode = (KeyCode)obj;
                    if (Input.GetKeyDown(keyCode) && !currentMultibind.Contains(keyCode))
                    {
                        currentMultibind.Add(keyCode);
                        UpdateMessageBox();
                    }
                }

                in_bind = true;
            }
            else if (!Input.anyKey && in_bind)
            {
                in_bind = false;
                currentKeys.Add(currentMultibind);
                currentMultibind = new List<KeyCode>();
                UpdateMessageBox();
            }
        }

        private void UpdateMessageBox()
        {
            var text = KeyBindingToString(currentKeys.Select(bind => bind.ToArray()).ToArray());
            if (currentMultibind.Count > 0)
            {
                if (text.Length > 0)
                    text += ",";

                text += KeyBindingToString(new[] {currentMultibind.ToArray()});
            }
            messageBox.SetMessage(new MessageDescriptor(text));
        }

        public static string KeyBindingToString(KeyCode[][] codes)
        {
            return String.Join(",", codes.Select(bind => String.Join("+", bind.Select(kc => kc.ToString()).ToArray())).ToArray());
        }

        public void Reset()
        {
            messageBox.SetMessage(new MessageDescriptor(KeyBindingToString(GetKeys())));
            editing = false;
        }

        public void Init(Func<KeyCode[][]> getKeys, Action<KeyCode[][]> setKeys, CustomOptionsScreen owner)
        {
            this.owner = owner;
            GetKeys = getKeys;
            SetKeys = setKeys;
            messageBox.SetMessage(new MessageDescriptor(KeyBindingToString(GetKeys())));
            CleverMenuItemTooltip component = base.GetComponent<CleverMenuItemTooltip>();
            tooltipProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            tooltipProvider.SetMessage("<icon>D</>: add or remove binds");
            component.Tooltip = tooltipProvider;
            owner.tooltipController.UpdateTooltip();
        }

        private Func<KeyCode[][]> GetKeys;

        private Action<KeyCode[][]> SetKeys;

        private bool in_bind;
        
        private bool editing;

        private MessageBox messageBox;

        private readonly List<List<KeyCode>> currentKeys = new List<List<KeyCode>>();
        private List<KeyCode> currentMultibind = new List<KeyCode>();

        private int exit;

        private CustomOptionsScreen owner;

        private BasicMessageProvider tooltipProvider;
    }

    public static class Ext
    {
        public static string KeyCodeToButtonIcon(this KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.A: return "<icon>n</>";
                case KeyCode.B: return "<icon>o</>";
                case KeyCode.C: return "<icon>p</>";
                case KeyCode.D: return "<icon>q</>";
                case KeyCode.DownArrow: return "<icon>r</>";
                case KeyCode.LeftArrow: return "<icon>s</>";
                case KeyCode.RightArrow: return "<icon>t</>";
                case KeyCode.S: return "<icon>u</>";
                case KeyCode.UpArrow: return "<icon>v</>";
                case KeyCode.W: return "<icon>w</>";
                case KeyCode.X: return "<icon>x</>";
                case KeyCode.Escape: return "<icon>y</>";
                case KeyCode.Tab: return "<icon>z</>";
                case KeyCode.V: return "<icon>A</>";
                case KeyCode.Z: return "<icon>B</>";
                case KeyCode.Space: return "<icon>C</>";
                case KeyCode.Return: return "<icon>D</>";
                case KeyCode.Mouse0: return "<icon>E</>";
                case KeyCode.Mouse1: return "<icon>F</>";
                case KeyCode.LeftControl: return "<icon>G</>";
                case KeyCode.RightControl: return "<icon>G</>";
                case KeyCode.E: return "<icon>H</>";
                case KeyCode.Q: return "<icon>I</>";
                case KeyCode.Alpha7: return "<icon>Q</>";
                case KeyCode.Alpha8: return "<icon>Q</>";
                case KeyCode.LeftShift: return "<icon>L</>";
                case KeyCode.RightShift: return "<icon>L</>";
                case KeyCode.Delete: return "<icon>M</>";
                case KeyCode.K: return "<icon>O</>";
                case KeyCode.L: return "<icon>P</>";
                case KeyCode.F: return "<icon>N</>";
                case KeyCode.R: return "<icon>T</>";
                default: return keyCode.ToString();
            }
        }
    }
}
