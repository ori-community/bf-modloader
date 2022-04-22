using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriDeModLoader.UIExtensions
{
    public class CustomWorldMapIcon
    {
        public MoonGuid Guid;
        public CustomWorldMapIconType Type = CustomWorldMapIconType.None;
        public WorldMapIconType NormalType;
        public Func<GameObject> IconFunc;
        public Vector3 Position;
        public Vector3 Scale = new Vector3(4, 4, 1);
        public bool IsSecret;

        private CustomWorldMapIcon(Vector3 position, MoonGuid guid)
        {
            Guid = guid;
            Position = position;
        }

        public CustomWorldMapIcon(CustomWorldMapIconType type, Vector3 position, MoonGuid guid) : this(position, guid)
        {
            Type = type;
        }

        public CustomWorldMapIcon(WorldMapIconType type, Vector3 position, MoonGuid guid) : this(position, guid)
        {
            NormalType = type;
        }

        public CustomWorldMapIcon(Func<GameObject> iconFunc, Vector3 position, MoonGuid guid) : this(position, guid)
        {
            IconFunc = iconFunc;
        }
    }

    public enum CustomWorldMapIconType
    {
        None,
        Plant,
        WaterVein,
        Sunstone,
        CleanWater,
        WindRestored,
        HoruRoom
    }

    public class CustomWorldMapIconManager
    {
        internal static readonly List<CustomWorldMapIcon> Icons = new List<CustomWorldMapIcon>();

        public static void Register(CustomWorldMapIcon icon)
        {
            Icons.Add(icon);
            iconMap[icon.Guid] = icon;
        }

        public static void Register(IEnumerable<CustomWorldMapIcon> icons)
        {
            foreach (var icon in icons)
                Register(icon);
        }

        internal static Dictionary<MoonGuid, CustomWorldMapIcon> iconMap = new Dictionary<MoonGuid, CustomWorldMapIcon>();
    }

    [HarmonyPatch(typeof(AreaMapIconManager), nameof(AreaMapIconManager.ShowAreaIcons))]
    class AreaMapIconManagerCustomIcons
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;

                if (instruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1); // runtimeGameWorldArea
                    yield return CodeInstruction.Call(typeof(AreaMapIconManagerCustomIcons), "AddIcons"); // AddIcons(runtimeGameWorldArea)
                }
            }
        }

        private static void AddIcons(RuntimeGameWorldArea runtimeGameWorldArea)
        {
            foreach (var icon in CustomWorldMapIconManager.Icons)
            {
                if (!runtimeGameWorldArea.Area.InsideFace(icon.Position))
                    continue;

                RuntimeWorldMapIcon runtimeWorldMapIcon = null;

                for (int i = runtimeGameWorldArea.Icons.Count - 1; i >= 0; i--)
                {
                    if (runtimeGameWorldArea.Icons[i].Guid == icon.Guid)
                    {
                        runtimeWorldMapIcon = runtimeGameWorldArea.Icons[i];
                        break;
                    }
                }

                bool collected = false; // TODO RandomizerLocationManager.IsPickupCollected(icon.Guid);
                if (runtimeWorldMapIcon == null && !collected)
                {
                    var worldMapIcon = (GameWorldArea.WorldMapIcon)FormatterServices.GetUninitializedObject(typeof(GameWorldArea.WorldMapIcon));
                    worldMapIcon.Guid = icon.Guid;
                    worldMapIcon.IsSecret = icon.IsSecret;
                    worldMapIcon.Position = icon.Position;

                    runtimeGameWorldArea.Icons.Add(new RuntimeWorldMapIcon(worldMapIcon, runtimeGameWorldArea));
                }
                else if (runtimeWorldMapIcon != null)
                {
                    runtimeWorldMapIcon.Position = icon.Position;
                    runtimeWorldMapIcon.Icon = collected ? WorldMapIconType.Invisible : WorldMapIconType.EnergyGateTwelve;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RuntimeWorldMapIcon), nameof(RuntimeWorldMapIcon.Show))]
    class RuntimeWorldMapIconCustomShow
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
