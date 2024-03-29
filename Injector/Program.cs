﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MInject;

namespace Injector
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Injector.exe <game path or steam launch url>");
                return 1;
            }

            var monoProcess = LaunchAndAttach(args[0]);
            if (monoProcess == null)
            {
                Console.WriteLine("Error attaching process");
                return 1;
            }

            bool debug = args.Contains("--debug");

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
            IntPtr classPointer = monoProcess.ClassFromName(assemblyImage, "OriDeModLoader", "EntryPoint");
            IntPtr methodPointer = monoProcess.ClassGetMethodFromName(classPointer, debug ? "BootModLoaderWithDebug" : "BootModLoader");

            //Boot our ModLoader
            monoProcess.RuntimeInvoke(methodPointer);
            //Dispose of the monoProcess before finishing
            monoProcess.Dispose();
            return 0;
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
