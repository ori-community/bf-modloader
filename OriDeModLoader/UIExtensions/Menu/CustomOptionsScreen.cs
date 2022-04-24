using BaseModLib;
using UnityEngine;

namespace OriDeModLoader.UIExtensions
{
    public abstract class CustomOptionsScreen : MonoBehaviour
    {
        public CleverMenuItemLayout layout;
        public CleverMenuItemSelectionManager selectionManager;
        public CleverMenuItemTooltipController tooltipController;
        public Transform pivot;
        public CleverMenuItemGroup group;

        public virtual void Awake()
        {
            // Layout and selection manager
            layout = GetComponent<CleverMenuItemLayout>();
            selectionManager = GetComponent<CleverMenuItemSelectionManager>();
            group = GetComponent<CleverMenuItemGroup>();
            layout.MenuItems.Clear();
            selectionManager.MenuItems.Clear();
            group.Options.Clear();
            pivot = transform.FindChild("highlightFade/pivot");
            foreach (object obj in pivot)
            {
                Destroy(((Transform)obj).gameObject);
            }

            TransparencyAnimator[] transparencyAnimators = GetComponentsInChildren<TransparencyAnimator>();
            for (int i = 0; i < transparencyAnimators.Length; i++)
            {
                if (transparencyAnimators[i].gameObject != gameObject)
                    transparencyAnimators[i].Reset();
            }

            // Tooltip
            Transform originalToolip = SettingsScreen.Instance.transform.Find("highlightFade/pivot/tooltip");
            Transform tooltip = Instantiate(originalToolip);
            tooltip.SetParent(pivot);
            tooltip.position = originalToolip.position;
            tooltipController = tooltip.GetComponent<CleverMenuItemTooltipController>();
            tooltipController.Selection = selectionManager;
            tooltipController.UpdateTooltip();
            tooltipController.enabled = true;

            InitScreen();
            selectionManager.SetCurrentItem(0);
        }

        public abstract void InitScreen();

        private void AddToLayout(CleverMenuItem item)
        {
            layout.AddItem(item);
            layout.Sort();
            item.SetOpacity(1f);
            item.OnUnhighlight();
        }

        public CleverMenuItem AddItem(string label)
        {
            GameObject gameObject = Instantiate(SettingsScreen.Instance.transform.Find("highlightFade/pivot/damageText").gameObject);
            gameObject.transform.SetParent(pivot);
            foreach (var c in gameObject.GetComponentsInChildren<MonoBehaviour>())
                c.enabled = true;
            CleverMenuItem component = gameObject.GetComponent<CleverMenuItem>();
            component.Pressed = null;
            selectionManager.MenuItems.Add(component);
            AddToLayout(component);
            TransparencyAnimator[] transparencyAnimators = component.transform.GetComponentsInChildren<TransparencyAnimator>();
            for (int i = 0; i < transparencyAnimators.Length; i++)
            {
                transparencyAnimators[i].Reset();
                transparencyAnimators[i].enabled = true;
            }
            foreach (object obj in component.transform.FindChild("glowGroup"))
            {
                TransparencyAnimator.Register((Transform)obj);
            }
            gameObject.transform.Find("text/nameText").GetComponent<MessageBox>().SetMessage(new MessageDescriptor(label));
            return component;
        }

        public void AddToggle(BoolSetting setting, string tooltip)
        {
            CleverMenuItem cleverMenuItem = AddItem(setting.Name);
            cleverMenuItem.name = setting.Name;
            ToggleCustomSettingsAction toggleCustomSettingsAction = cleverMenuItem.gameObject.AddComponent<ToggleCustomSettingsAction>();
            toggleCustomSettingsAction.Setting = setting;
            toggleCustomSettingsAction.Init();
            cleverMenuItem.PressedCallback += toggleCustomSettingsAction.Toggle;

            ConfigureTooltip(cleverMenuItem.GetComponent<CleverMenuItemTooltip>(), tooltip);
        }

        public void AddSlider(FloatSetting setting, float min, float max, float step, string tooltip)
        {
            // Template is music volume slider
            GameObject clone = Instantiate(SettingsScreen.Instance.transform.Find("highlightFade/pivot/musicVolume").gameObject);
            clone.gameObject.name = setting.Name;
            foreach (var c in clone.GetComponentsInChildren<MonoBehaviour>())
                c.enabled = true;

            // Add to navigation manager (required for all option types)
            clone.transform.SetParent(pivot);
            CleverMenuItem cleverMenuItem = clone.GetComponent<CleverMenuItem>();
            selectionManager.MenuItems.Add(cleverMenuItem);
            AddToLayout(cleverMenuItem);

            // Add to group (required for sliders and dropdown items, but not toggles)
            CleverValueSlider slider = clone.transform.FindChild("slider").GetComponent<CleverValueSlider>();
            slider.NavigateMessageBoxes = new MessageBox[]
            {
                transform.FindChild("highlightFade/legend/pcLegend/navigate").GetComponent<MessageBox>(),
                transform.FindChild("highlightFade/legend/xBoxLegend/navigate").GetComponent<MessageBox>()
            };
            group.AddItem(cleverMenuItem, slider);

            slider.MinValue = min;
            slider.MaxValue = max;
            slider.Step = step;
            CustomSlider customSlider = slider.gameObject.AddComponent<CustomSlider>();
            customSlider.Setting = setting;

            MessageBox nameTextBox = clone.transform.Find("nameText").GetComponent<MessageBox>();
            nameTextBox.MessageProvider = null;
            nameTextBox.SetMessage(new MessageDescriptor(setting.Name));

            ConfigureTooltip(clone.GetComponent<CleverMenuItemTooltip>(), tooltip);
        }

        private void ConfigureTooltip(CleverMenuItemTooltip tooltipComponent, string tooltip)
        {
            var tooltipMessageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
            tooltipMessageProvider.SetMessage(tooltip);
            tooltipComponent.Tooltip = tooltipMessageProvider;
        }
    }
}
