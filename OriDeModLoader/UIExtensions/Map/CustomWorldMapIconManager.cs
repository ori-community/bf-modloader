using System.Collections.Generic;

namespace OriDeModLoader.UIExtensions
{
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
}
