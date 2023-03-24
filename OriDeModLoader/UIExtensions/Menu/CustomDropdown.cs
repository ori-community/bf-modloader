using System;
using BaseModLib;

namespace OriDeModLoader
{
    public class CustomDropdown : CleverMenuOptionsList
    {
        public OptionsListItem[] items;
        public IntSetting intSetting;
        public bool dismissOnChoose;

        new void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public new void OnEnable()
        {
            base.OnEnable();
            ClearItems();

            for (int i = 0; i < items.Length; i++)
            {
                int index = i; // fun capture stuff
                AddItem(items[index].label, () =>
                {
                    items[index].onPressed?.Invoke();

                    intSetting.Value = index;

                    MessageBox valueTextBox = this.transform.parent.Find("text/stateText").GetComponent<MessageBox>();
                    valueTextBox.MessageProvider = null;
                    valueTextBox.SetMessage(new MessageDescriptor(items[index].label));

                    if (dismissOnChoose)
                    {
                        this.GetComponent<CleverMenuItemGroup>().OnSelectionManagerBackPressed();
                    }
                });
            }

            SetSelection(intSetting.Value);
        }
    }

    public class OptionsListItem
    {
        public string label;

        public OptionsListItem(string label)
        {
            this.label = label;
        }

        public string tooltip;
        public Action onPressed;
    }
}
