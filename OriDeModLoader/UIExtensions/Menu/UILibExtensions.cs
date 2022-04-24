using System;
using HarmonyLib;

namespace OriDeModLoader.UIExtensions
{
    internal static class UILibExtensions
    {
        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, Action onPress)
        {
            selectionManager.AddMenuItem(label, selectionManager.MenuItems.Count - 1, onPress);
        }

        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, int index, Action onPress)
        {
            CleverMenuItemLayout component = selectionManager.gameObject.GetComponent<CleverMenuItemLayout>();
            if (component != null)
            {
                selectionManager.AddMenuItem(label, index, component, onPress);
                return;
            }
        }

        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, int index, CleverMenuItemLayout layout, Action onPress)
        {
            CleverMenuItem cleverMenuItem = UnityEngine.Object.Instantiate<CleverMenuItem>(selectionManager.MenuItems[0]);
            cleverMenuItem.gameObject.name = label;
            cleverMenuItem.transform.SetParent(selectionManager.MenuItems[1].transform.parent);
            if (onPress != null)
                cleverMenuItem.PressedCallback += onPress;
            cleverMenuItem.gameObject.GetComponentInChildren<MessageBox>().SetMessage(new MessageDescriptor(label));
            cleverMenuItem.ApplyColors();
            selectionManager.MenuItems.Insert(index, cleverMenuItem);
            layout.AddItem(cleverMenuItem, index);
        }

        internal static void AddItem(this CleverMenuItemLayout layout, CleverMenuItem item)
        {
            layout.MenuItems.Add(item);
            layout.Sort();
        }

        internal static void AddItem(this CleverMenuItemLayout layout, CleverMenuItem item, int index)
        {
            layout.MenuItems.Insert(index, item);
            layout.Sort();
        }

        internal static void AddItem(this CleverMenuItemGroup group, CleverMenuItem item, CleverMenuItemGroupBase itemGroup)
        {
            CleverMenuItemGroup.CleverMenuItemGroupItem cleverMenuItemGroupItem = new CleverMenuItemGroup.CleverMenuItemGroupItem
            {
                ItemGroup = itemGroup,
                MenuItem = item
            };
            cleverMenuItemGroupItem.ItemGroup.IsActive = false;
            itemGroup.OnBackPressed = (Action)Delegate.Combine(itemGroup.OnBackPressed, new Action(group.OnOptionBackPressed));
            group.Options.Add(cleverMenuItemGroupItem);
        }

        internal static void Reset(this TransparencyAnimator animator)
        {
            var childTransparencyAnimators = Traverse.Create(animator).Field("m_childTransparencyAnimators").GetValue();
            var cleverMenuItems = Traverse.Create(animator).Field("m_cleverMenuItems").GetValue();
            var rendererData = Traverse.Create(animator).Field("m_rendererData").GetValue();
            var renderers = Traverse.Create(animator).Field("m_renderers").GetValue();

            if (childTransparencyAnimators != null)
                Traverse.Create(childTransparencyAnimators).Method("Clear");
            if (cleverMenuItems != null)
                Traverse.Create(cleverMenuItems).Method("Clear");
            if (rendererData != null)
                Traverse.Create(rendererData).Method("Clear");
            if (renderers != null)
                Traverse.Create(renderers).Method("Clear");
        }
    }
}
