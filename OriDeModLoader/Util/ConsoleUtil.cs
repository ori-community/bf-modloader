using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

//Merged together from a lot of stackoverflow threads
namespace BFModLoader.Util
{
    public class ConsoleUtil
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName
            , [MarshalAs(UnmanagedType.U4)] DesiredAccess dwDesiredAccess
            , [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode
            , uint lpSecurityAttributes
            , [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition
            , [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes
            , uint hTemplateFile);

       
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr hHandle);
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        // GetWindowRect gets the win32 RECT by a window handle.
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        // MoveWindow moves a window or changes its size based on a window handle.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        private enum StdHandle : int
        {
            Input = -10,
            Output = -11,
            Error = -12
        }

        [Flags]
        enum DesiredAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        public static void CreateConsole()
        {
            if (!AllocConsole()) return;
            
            //https://developercommunity.visualstudio.com/content/problem/12166/console-output-is-gone-in-vs2017-works-fine-when-d.html
            // Console.OpenStandardOutput eventually calls into GetStdHandle. As per MSDN documentation of GetStdHandle: http://msdn.microsoft.com/en-us/library/windows/desktop/ms683231(v=vs.85).aspx will return the redirected handle and not the allocated console:
            // "The standard handles of a process may be redirected by a call to  SetStdHandle, in which case  GetStdHandle returns the redirected handle. If the standard handles have been redirected, you can specify the CONIN$ value in a call to the CreateFile function to get a handle to a console's input buffer. Similarly, you can specify the CONOUT$ value to get a handle to a console's active screen buffer."
            // Get the handle to CONOUT$.    
            var stdOutHandle = CreateFile("CONOUT$", DesiredAccess.GenericRead | DesiredAccess.GenericWrite,
                FileShare.ReadWrite, 0, FileMode.Open, FileAttributes.Normal, 0);

            if (stdOutHandle == new IntPtr(-1))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!SetStdHandle(StdHandle.Output, stdOutHandle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            
            if (!SetStdHandle(StdHandle.Error, stdOutHandle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.Title = "Ori BF Mod Loader Debug Console";

            var hWndFrom = GetConsoleWindow();
            if (!GetWindowRect(hWndFrom, out var rectFrom)) return;

            //TODO this should *probably* be loaded from config/calced from monitor size
            //These just put it on my seconds monitor - xem :p
            const int xTo = 3000;
            const int yTo = 200; 
            var widthFrom = rectFrom.Right - rectFrom.Left; 
            var heightFrom = rectFrom.Bottom - rectFrom.Top;
            MoveWindow(hWndFrom, xTo, yTo, widthFrom, heightFrom, true);
            
            var handle = FindWindow(null, "Ori And The Blind Forest: Definitive Edition");
            ShowWindow(handle, ShowWindowEnum.Restore);
            SetForegroundWindow(handle);
        }
    }
}
