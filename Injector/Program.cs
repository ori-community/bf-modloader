using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MInject;

namespace Injector
{
    class Program
    {
        static void Main(string[] args)
        {
             //Try to attach to targetProcess Mono module
            MonoProcess monoProcess;
            var attempts = 0;
            const int maxAttempts = 10;
            bool success;
            do
            {
                if(attempts != 0)
                    Thread.Sleep(1000);
                attempts++;
                
                //Grab the target process by its name or start a new one
                var runningOri = Process.GetProcessesByName("oriDE");
                var targetProcess = runningOri.Length > 0
                    ? runningOri[0]
                    : Process.Start("C:\\SteamLibrary\\steamapps\\common\\Ori DE\\oriDE.exe");
                //TODO/FIXME: Should probably get this from config/env
                
                success = MonoProcess.Attach(targetProcess, out monoProcess);
            } while (attempts < maxAttempts && !success);

            //Nop, can't find Ori :(
            if (!success) return;
            
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
            IntPtr methodPointer = monoProcess.ClassGetMethodFromName(classPointer, "BootModLoader");

            //Boot our ModLoader
            monoProcess.RuntimeInvoke(methodPointer);
            //Dispose of the monoProcess before finishing
            monoProcess.Dispose();
        }
    }
}
