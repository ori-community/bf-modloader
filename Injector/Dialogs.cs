using System;
using System.Runtime.InteropServices;

namespace Injector
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
    }

    public static class OpenFileDialog
    {
        private const int OFN_NOCHANGEDIR = 0x00000008;

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName(ref OpenFileName ofn);

        public static string ShowDialog(string title)
        {
            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFilter = "Executable Files (*.exe)\0*.exe\0All Files (*.*)\0*.*\0";
            ofn.lpstrFile = new string(new char[256]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[64]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = title;
            ofn.Flags = OFN_NOCHANGEDIR;

            if (GetOpenFileName(ref ofn))
                return ofn.lpstrFile;

            return string.Empty;
        }
    }

    public static class Dialog
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static DialogResult Show(string text, string title, MessageBoxButtons buttons)
        {
            return (DialogResult)MessageBox((IntPtr)0, text, title, (int)buttons);
        }
    }

    public enum MessageBoxButtons
    {
        OkCancel = 1,
        YesNo = 4
    }

    public enum DialogResult
    {
        Ok = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }
}
