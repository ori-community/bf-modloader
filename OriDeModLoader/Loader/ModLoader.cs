using System.Collections.Generic;

namespace OriDeModLoader
{
    public static class ModLoader
    {
        public static IEnumerable<LoadedModInfo> GetLoadedMods()
        {
            foreach (var mod in EntryPoint.loadedMods)
                yield return mod;
        }
    }
}
