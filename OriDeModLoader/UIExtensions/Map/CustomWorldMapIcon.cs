using System;
using UnityEngine;

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
        public Func<MoonGuid, bool> Visible;

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
}
