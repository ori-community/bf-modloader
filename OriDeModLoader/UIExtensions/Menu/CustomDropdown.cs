using System;

namespace OriDeModLoader.UIExtensions
{
    public class CustomDropdown : CleverMenuOptionsList
    {
        public OptionsListItem[] items;
        public int defaultSelection;
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

            foreach (var item in items)
            {
                AddItem(item.label, () =>
                {
                    item.onPressed?.Invoke();

                    MessageBox valueTextBox = this.transform.parent.Find("text/stateText").GetComponent<MessageBox>();
                    valueTextBox.MessageProvider = null;
                    valueTextBox.SetMessage(new MessageDescriptor(item.label));

                    if (dismissOnChoose)
                    {
                        this.GetComponent<CleverMenuItemGroup>().OnSelectionManagerBackPressed();
                    }
                });
            }

            SetSelection(defaultSelection);
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
