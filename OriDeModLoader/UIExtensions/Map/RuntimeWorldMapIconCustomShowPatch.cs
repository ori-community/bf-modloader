using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriDeModLoader.UIExtensions
{
    [HarmonyPatch(typeof(RuntimeWorldMapIcon), nameof(RuntimeWorldMapIcon.Show))]
    class RuntimeWorldMapIconCustomShowPatch
    {
        public static bool Prefix(RuntimeWorldMapIcon __instance)
        {
            var m_iconGameObject = Traverse.Create(__instance).Field("m_iconGameObject");

            if (__instance.Icon == WorldMapIconType.Invisible)
            {
                return false;
            }
            if (!__instance.IsVisible(AreaMapUI.Instance))
            {
                return false;
            }

            var iconGameObject = m_iconGameObject.GetValue<GameObject>();
            if (iconGameObject)
            {
                iconGameObject.SetActive(true);
            }
            else
            {
                iconGameObject = CreateIcon(__instance);
                m_iconGameObject.SetValue(iconGameObject);
            }

            iconGameObject.transform.localPosition = __instance.Position;

            return false;
        }

        private static GameObject CreateIcon(RuntimeWorldMapIcon instance)
        {
            if (CustomWorldMapIconManager.iconMap.ContainsKey(instance.Guid))
            {
                var customIcon = CustomWorldMapIconManager.iconMap[instance.Guid];
                if (customIcon.IconFunc != null)
                    return CreateCustomFuncIcon(customIcon.IconFunc());
                else if (customIcon.Type == CustomWorldMapIconType.None)
                    return CreateStandardIcon(customIcon.NormalType);
                else
                    return CreateCustomIcon(customIcon.Type);
            }

            return CreateStandardIcon(instance.Icon);
        }

        private static GameObject CreateStandardIcon(WorldMapIconType iconType)
        {
            GameObject icon = AreaMapUI.Instance.IconManager.GetIcon(iconType);
            var iconGameObject = (GameObject)InstantiateUtility.Instantiate(icon);
            Transform transform = iconGameObject.transform;
            transform.parent = AreaMapUI.Instance.Navigation.MapPivot.transform;
            transform.localRotation = Quaternion.identity;
            transform.localScale = icon.transform.localScale;
            TransparencyAnimator.Register(transform);
            return iconGameObject;
        }

        private static GameObject CreateCustomFuncIcon(GameObject gameObject)
        {
            gameObject.SetActive(true);
            gameObject.transform.SetParent(AreaMapUI.Instance.Navigation.MapPivot.transform);
            //gameObject.transform.localScale = new Vector3(scale, scale, 1f);

            TransparencyAnimator.Register(gameObject.transform);
            return gameObject;
        }

        private static GameObject CreateCustomIcon(CustomWorldMapIconType iconType)
        {
            switch (iconType)
            {
                case CustomWorldMapIconType.WaterVein:
                    return CreateIconFromInventory("ginsoKeyIcon/ginsoKeyGraphic", 4);

                case CustomWorldMapIconType.CleanWater:
                    var icon = CreateIconFromInventory("waterPurifiedIcon/waterPurifiedGraphics", 20);
                    var offset = icon.transform.Find("waterPurifiedGraphic").localPosition;
                    foreach (var child in icon.transform)
                        ((Transform)child).localPosition -= offset;
                    return icon;

                case CustomWorldMapIconType.WindRestored:
                    return CreateIconFromInventory("windRestoredIcon/windRestoredIcon", 10);

                case CustomWorldMapIconType.Sunstone:
                    return CreateIconFromInventory("mountHoru/sunStoneA", 8);

                case CustomWorldMapIconType.HoruRoom:
                    return CreateIconFromInventory("warmthReturned/warmthReturnedGraphics", 10);

                case CustomWorldMapIconType.Plant:
                    var plantIcon = CreateStandardIcon(WorldMapIconType.HealthUpgrade);
                    plantIcon.name = "plantMapIcon(Clone)";
                    Renderer[] plantRenderers = plantIcon.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < plantRenderers.Length; i++)
                        plantRenderers[i].material.color = new Color(0.1792157f, 0.2364706f, 0.8656863f);
                    plantIcon.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                    return plantIcon;
            }

            return null;
        }

        private static Transform inventoryTemplate;
        private static GameObject CreateIconFromInventory(string name, float scale)
        {
            if (!inventoryTemplate)
            {
                inventoryTemplate = SceneManager.GetSceneByName("loadBootstrap").GetRootGameObjects().First((GameObject go) => go.name == "inventoryScreen").transform;
            }

            GameObject gameObject = UnityEngine.Object.Instantiate(inventoryTemplate.transform.Find("progression").Find(name)).gameObject;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(AreaMapUI.Instance.Navigation.MapPivot.transform);
            gameObject.transform.localScale = new Vector3(scale, scale, 1f);

            TransparencyAnimator.Register(gameObject.transform);
            return gameObject;
        }
    }
}
