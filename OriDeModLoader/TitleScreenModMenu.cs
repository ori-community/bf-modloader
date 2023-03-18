using System;
using System.Collections.Generic;
using System.Linq;
using BaseModLib;
using Game;
using OriDeModLoader.UIExtensions;
using UnityEngine;

namespace OriDeModLoader
{
    public class TitleScreenModMenu
    {
        public static void Init()
        {
            SceneBootstrap.RegisterHandler(Bootstrap);
        }

        private static void Bootstrap(SceneBootstrap bootstrap)
        {
            bootstrap.BootstrapActions = new Dictionary<string, Action<SceneRoot>>
            {
                ["titleScreenSwallowsNest"] = sceneRoot =>
                {
                    var manager = sceneRoot.transform.Find("ui/group/3. fullGameMainMenu").GetComponent<CleverMenuItemSelectionManager>();

                    var messageProvider = ScriptableObject.CreateInstance<BasicMessageProvider>();
                    messageProvider.SetMessage(string.Join("\n", ModLoader.GetLoadedMods().Select(m => m.Mod.Name).ToArray()));

                    manager.AddMenuItem("MODS", manager.MenuItems.Count - 1, () =>
                    {
                        UI.Hints.Show(messageProvider, HintLayer.Gameplay, 3);
                    });
                }
            };
        }
    }
}
