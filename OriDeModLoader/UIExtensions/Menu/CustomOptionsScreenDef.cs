using System;

namespace OriDeModLoader.UIExtensions
{
    public struct CustomOptionsScreenDef
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public Type ControllerType { get; set; }

        public Action<object> Init { get; set; }
    }
}
