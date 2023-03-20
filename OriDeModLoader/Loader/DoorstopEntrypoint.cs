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
            if (args.LoadedAssembly.GetName().Name == "System")
            {
                bool debug = Environment.GetCommandLineArgs().Contains("--debug");

                if (debug)
                    OriDeModLoader.EntryPoint.BootModLoaderWithDebug();
                else
                    OriDeModLoader.EntryPoint.BootModLoader();
            }
        }
    }
}
