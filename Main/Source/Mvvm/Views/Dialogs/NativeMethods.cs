namespace Inspiring.Mvvm.Views {
   using System;
   using System.Runtime.InteropServices;
   using System.Text;

   static class NativeMethods {

      [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern IntPtr GetActiveWindow();

      [DllImport("shell32.dll", SetLastError = true)]
      public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Environment.SpecialFolder nFolder, ref IntPtr ppidl);

      [DllImport("user32.dll")]
      public static extern IntPtr SendMessage(IntPtr hWnd, FolderBrowserDialogMessage msg, IntPtr wParam, IntPtr lParam);

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      public static extern IntPtr SendMessage(IntPtr hWnd, FolderBrowserDialogMessage msg, IntPtr wParam, string lParam);

      [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

      [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
      public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

      [DllImport("shell32.dll", PreserveSig = false)]
      public static extern IMalloc SHGetMalloc();

      public enum FolderBrowserDialogMessage {
         Initialized = 1,
         SelChanged = 2,
         ValidateFailedA = 3,
         ValidateFailedW = 4,
         EnableOk = 0x465,
         SetSelection = 0x467
      }

      public delegate int BrowseCallbackProc(IntPtr hwnd, FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam);

      [Flags]
      public enum BrowseInfoFlags {
         ReturnOnlyFsDirs = 0x00000001,
         DontGoBelowDomain = 0x00000002,
         StatusText = 0x00000004,
         ReturnFsAncestors = 0x00000008,
         EditBox = 0x00000010,
         Validate = 0x00000020,
         NewDialogStyle = 0x00000040,
         UseNewUI = NewDialogStyle | EditBox,
         BrowseIncludeUrls = 0x00000080,
         UaHint = 0x00000100,
         NoNewFolderButton = 0x00000200,
         NoTranslateTargets = 0x00000400,
         BrowseForComputer = 0x00001000,
         BrowseForPrinter = 0x00002000,
         BrowseIncludeFiles = 0x00004000,
         Shareable = 0x00008000,
         BrowseFileJunctions = 0x00010000
      }

      public struct BROWSEINFO {
         public IntPtr hwndOwner;
         public IntPtr pidlRoot;
         public string pszDisplayName;
         public string lpszTitle;
         public BrowseInfoFlags ulFlags;
         public BrowseCallbackProc lpfn;
         public IntPtr lParam;
         public int iImage;
      }
   }
}
