using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OriDeModLoader.Util;

namespace Doorstop
{
    // Allows modding using Doorstop instead of MInject
    public static class Entrypoint
    {
        public static void Start()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            Assembly.Load(File.ReadAllBytes(PathUtil.MakeAbsolute("0Harmony.dll")));
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            TestLog($"Loaded {args.LoadedAssembly.FullName}");
            if (args.LoadedAssembly.GetName().Name == "Assembly-CSharp")
            {
                TestLog("Loading loader");

                bool debug = Environment.GetCommandLineArgs().Contains("--debug");

                if (debug)
                    OriDeModLoader.EntryPoint.BootModLoaderWithDebug();
                else
                    OriDeModLoader.EntryPoint.BootModLoader();
            }
        }

        public static void TestLog(string message)
        {
            using (var sw = new StreamWriter(@"G:\Games\Steam\steamapps\common\Ori DE\test.log", append:true))
                sw.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
        }
    }
}
