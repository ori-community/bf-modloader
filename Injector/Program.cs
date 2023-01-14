using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using MInject;

namespace Injector
{
    class Program
    {
        static int Main(string[] args)
        {
            var gameExePath = GetPath();
            if (gameExePath == null)
                return 1;

            var monoProcess = LaunchAndAttach(gameExePath);
            if (monoProcess == null)
            {
                Console.WriteLine("Error attaching process");
                return 1;
            }

            //Load both required DLLs
            byte[] harmony = File.ReadAllBytes("0harmony.dll");
            byte[] assembly = File.ReadAllBytes("OriDeModLoader.dll");

            //Prepare for injection
            IntPtr monoDomain = monoProcess.GetRootDomain();
            monoProcess.ThreadAttach(monoDomain);
            monoProcess.SecuritySetMode(0);

            //Inject the harmony DLL first since we're not changing Assembly Lookup Paths yet
            IntPtr harmonyImage = monoProcess.ImageOpenFromDataFull(harmony);
            monoProcess.AssemblyLoadFromFull(harmonyImage);

            //Inject and boot our mod loader 
            IntPtr modLoaderImage = monoProcess.ImageOpenFromDataFull(assembly);
            IntPtr assemblyPointer = monoProcess.AssemblyLoadFromFull(modLoaderImage);
            IntPtr assemblyImage = monoProcess.AssemblyGetImage(assemblyPointer);
            IntPtr classPointer = monoProcess.ClassFromName(assemblyImage, "OriDeModLoader.Loader", "EntryPoint");
            IntPtr methodPointer = monoProcess.ClassGetMethodFromName(classPointer, "BootModLoader");

            //Boot our ModLoader
            monoProcess.RuntimeInvoke(methodPointer);
            //Dispose of the monoProcess before finishing
            monoProcess.Dispose();
            return 0;
        }

        static string GetPath()
        {
            const string steamLaunchUrl = "steam://run/387290";
            const string configFilename = "injector.config";

            var config = Config.FromFile(configFilename);

            if (config.steam)
                return steamLaunchUrl;

            if (File.Exists(config.path))
                return config.path;

            if (Dialog.Show("Use steam?", "Steam", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                config.steam = true;
                config.WriteToFile(configFilename);
                return steamLaunchUrl;
            }

            if (Dialog.Show("Locate oriDE.exe", "Non-steam", MessageBoxButtons.OkCancel) == DialogResult.Cancel)
                return null;

            var exePath = OpenFileDialog.ShowDialog("Locate game executable...");
            if (exePath == null || !File.Exists(exePath))
                return null;

            config.path = exePath;
            config.steam = false;
            config.WriteToFile(configFilename);
            return config.path;
        }

        static MonoProcess LaunchAndAttach(string path)
        {
            if (Process.GetProcessesByName("oriDE").Length > 0)
            {
                Console.WriteLine("Error: oriDE already running. Please close and try again.");
                return null;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            if (Regex.IsMatch(path, "^steam://run/[0-9]+$"))
            {
                Console.WriteLine("Launching via Steam");
                startInfo.UseShellExecute = true;
            }
            else if (!File.Exists(path))
            {
                Console.WriteLine($"Could not find the file {path}");
                return null;
            }

            Process.Start(startInfo);

            //Try to attach to targetProcess Mono module
            MonoProcess monoProcess = null;
            var attempts = 0;
            const int maxAttempts = 30;
            Process[] runningOri;
            bool success = false;
            do
            {
                if (attempts != 0)
                    Thread.Sleep(1000);

                attempts++;

                runningOri = Process.GetProcessesByName("oriDE");
                if (runningOri.Length > 0)
                {
                    var proc = runningOri[0];
                    success = MonoProcess.Attach(proc, out monoProcess);
                }
            } while (attempts < maxAttempts && !success);

            //Nop, can't find Ori :(
            if (!success || monoProcess == null)
            {
                Console.WriteLine("Unable to connect to oriDE");
                return null;
            }

            Console.WriteLine("Successfully attached to process");
            return monoProcess;
        }
    }
}
