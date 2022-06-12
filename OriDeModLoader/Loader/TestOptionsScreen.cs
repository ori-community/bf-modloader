using BaseModLib;
using OriDeModLoader.UIExtensions;

namespace OriDeModLoader
{
    public class TestOptionsScreen : CustomOptionsScreen
    {
        public static BoolSetting testSetting = new BoolSetting("testSetting", false);
        public static BoolSetting testSetting2 = new BoolSetting("testSetting2", false);
        public static BoolSetting testSetting3 = new BoolSetting("testSetting3", false);
        public static BoolSetting testSetting4 = new BoolSetting("testSetting4", false);
        public static BoolSetting testSetting5 = new BoolSetting("testSetting5", false);
        public static BoolSetting testSetting6 = new BoolSetting("testSetting6", false);

        public override void InitScreen()
        {
            AddToggle(testSetting, "Test setting", "This is a test setting");
            AddToggle(testSetting2, "Test setting 2", "This is a test setting 2");
            AddToggle(testSetting3, "Test setting 3", "This is a test setting ");
            AddToggle(testSetting4, "Test setting 4", "This is a test setting 4");
            AddToggle(testSetting5, "Test setting 5", "This is a test setting 5");
            AddToggle(testSetting6, "Test setting 6", "This is a test setting 6");
        }
    }
}
