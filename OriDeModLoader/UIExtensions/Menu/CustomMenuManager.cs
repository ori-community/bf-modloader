using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Logger = BFModLoader.ModLoader.Logger;

namespace OriDeModLoader.UIExtensions
{
    public class CustomMenuManager
    {
        private static readonly List<CustomOptionsScreenDef> optionsScreens = new List<CustomOptionsScreenDef>();

        /// <summary>
        /// Register a screen to appear in the Options sub-menu
        /// </summary>
        public static void RegisterOptionsScreen<TController>(string name, int index) where TController : MonoBehaviour
        {
            RegisterOptionsScreen<TController>(name, index ,_ => { });
        } 
        
        public static CustomOptionsScreenDef RegisterOptionsScreen<TController>(string name, int index, Action<TController> init) where TController : MonoBehaviour
        {
            var def = new CustomOptionsScreenDef
            {
                ControllerType = typeof(TController),
                Index = index,
                Name = name,
                Init = controller => init(controller as TController)
            };
            optionsScreens.Add(def);
            return def;
        }

        public static void UnregisterSettingsScreen(CustomOptionsScreenDef def)
        {
            optionsScreens.Remove(def);
            //TODO: MOOOOOAR
        }
        internal static IEnumerable<CustomOptionsScreenDef> GetOptionsScreens()
        {
            return optionsScreens.OrderBy(o => o.Index).ThenBy(o => o.Name);
        }

    }

    [HarmonyPatch(typeof(OptionsScreen), nameof(OptionsScreen.Awake))]
    internal class CustomMenuCreator
    {
        private static void Postfix(OptionsScreen __instance)
        {
            var screens = CustomMenuManager.GetOptionsScreens().ToArray();
            for (int i = 0; i < screens.Length; i++)
            {
                AddSubscreen(__instance, screens[i].ControllerType, screens[i].Name.ToUpper(), i + 2, screens[i].Init);
            }
        }

        private static void AddSubscreen(OptionsScreen optionsScreen, Type controllerType, string label, int index, Action<object> init)
        {
            optionsScreen.Navigation.AddMenuItem(label, index, optionsScreen.Navigation.transform.FindChild("mainMenuUI").GetComponent<CleverMenuItemLayout>(), null);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(optionsScreen.transform.FindChild("*settings").gameObject);
            gameObject.name = "*" + label.ToLower();
            gameObject.transform.SetParent(optionsScreen.transform);
            UnityEngine.Object.Destroy(gameObject.GetComponent<SettingsScreen>());
            var component = gameObject.AddComponent(controllerType);
            init(component);
            gameObject.SetActive(false);
            optionsScreen.GetComponent<CleverMenuItemGroup>().AddItem(optionsScreen.Navigation.MenuItems[index], gameObject.GetComponent<CleverMenuItemGroupBase>());
        }
    }
}
