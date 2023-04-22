using System.Collections.Generic;
using System.Linq;

namespace OriDeModLoader
{
    public static class ModLoader
    {
        public static IEnumerable<LoadedModInfo> GetLoadedMods()
        {
            foreach (var mod in EntryPoint.loadedMods)
                yield return mod;
        }

        /// <summary>
        /// Returns the mod with the name as defined in its IMod implementation, or null if it is not loaded
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IMod GetMod(string name)
        {
            return GetLoadedMods().FirstOrDefault(m => m.Mod.Name == name)?.Mod;
        }
    }
}
