namespace Inspiring.Mvvm.Views {
   using System;
   using System.Runtime.InteropServices;
   using System.Text;
   using System.Windows;
   using System.Windows.Interop;

   internal sealed class FolderBrowserDialog {
      private string _description;
      private string _selectedPath;

      public FolderBrowserDialog() {
         Reset();
      }

      /// <summary>
      /// Gets or sets the descriptive text displayed above the tree view control in the dialog box, or below the list view control
      /// in the Vista style dialog.
      /// </summary>
      /// <value>
      /// The description to display. The default is an empty string ("").
      /// </value>
      public string Description {
         get {
            return _description ?? string.Empty;
         }
         set {
            _description = value;
         }
      }

      /// <summary>
      /// Gets or sets the root folder where the browsing starts from. This property has no effect if the Vista style
      /// dialog is used.
      /// </summary>
      /// <value>
      /// One of the <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
      /// </value>
      /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="System.Environment.SpecialFolder" /> values.</exception>
      public System.Environment.SpecialFolder RootFolder { get; set; }

      /// <summary>
      /// Gets or sets the path selected by the user.
      /// </summary>
      /// <value>
      /// The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an empty string ("").
      /// </value>
      public string SelectedPath {
         get {
            return _selectedPath ?? string.Empty;
         }
         set {
            _selectedPath = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box. This
      /// property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
      /// </summary>
      /// <value>
      /// <see langword="true" /> if the New Folder button is shown in the dialog box; otherwise, <see langword="false" />. The default is <see langword="true" />.
      /// </value>
      public bool ShowNewFolderButton { get; set; }

      /// <summary>
      /// Displays the folder browser dialog.
      /// </summary>
      /// <param name="owner">Handle to the window that owns the dialog.</param>
      /// <returns>If the user clicks the OK button, <see langword="true" /> is returned; otherwise, <see langword="false" />.</returns>
      public bool ShowDialog(Window owner) {
         IntPtr ownerHandle = owner == null ? NativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
         return RunDialog(ownerHandle);
      }

      /// <summary>
      /// Resets all properties to their default values.
      /// </summary>
      public void Reset() {
         _description = string.Empty;
         _selectedPath = string.Empty;
         RootFolder = Environment.SpecialFolder.Desktop;
         ShowNewFolderButton = true;
      }

      private bool RunDialog(IntPtr owner) {
         IntPtr rootItemIdList = IntPtr.Zero;
         IntPtr resultItemIdList = IntPtr.Zero;
         if (NativeMethods.SHGetSpecialFolderLocation(owner, RootFolder, ref rootItemIdList) != 0) {
            if (NativeMethods.SHGetSpecialFolderLocation(owner, 0, ref rootItemIdList) != 0) {
               throw new InvalidOperationException(ExceptionTexts.FolderBrowserDialogNoRootFolder);
            }
         }
         try {
            NativeMethods.BROWSEINFO info = new NativeMethods.BROWSEINFO();
            info.hwndOwner = owner;
            info.lpfn = new NativeMethods.BrowseCallbackProc(BrowseCallbackProc);
            info.lpszTitle = Description;
            info.pidlRoot = rootItemIdList;
            info.pszDisplayName = new string('\0', 260);
            info.ulFlags = NativeMethods.BrowseInfoFlags.NewDialogStyle | NativeMethods.BrowseInfoFlags.ReturnOnlyFsDirs;
            if (!ShowNewFolderButton)
               info.ulFlags |= NativeMethods.BrowseInfoFlags.NoNewFolderButton;
            resultItemIdList = NativeMethods.SHBrowseForFolder(ref info);
            if (resultItemIdList != IntPtr.Zero) {
               StringBuilder path = new StringBuilder(260);
               NativeMethods.SHGetPathFromIDList(resultItemIdList, path);
               SelectedPath = path.ToString();
               return true;
            } else
               return false;
         } finally {
            if (rootItemIdList != null) {
               IMalloc malloc = NativeMethods.SHGetMalloc();
               malloc.Free(rootItemIdList);
               Marshal.ReleaseComObject(malloc);
            }
            if (resultItemIdList != null) {
               Marshal.FreeCoTaskMem(resultItemIdList);
            }
         }
      }

      private int BrowseCallbackProc(IntPtr hwnd, NativeMethods.FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam) {
         switch (msg) {
            case NativeMethods.FolderBrowserDialogMessage.Initialized:
               if (SelectedPath.Length != 0)
                  NativeMethods.SendMessage(hwnd, NativeMethods.FolderBrowserDialogMessage.SetSelection, new IntPtr(1), SelectedPath);
               break;
            case NativeMethods.FolderBrowserDialogMessage.SelChanged:
               if (lParam != IntPtr.Zero) {
                  StringBuilder path = new StringBuilder(260);
                  bool validPath = NativeMethods.SHGetPathFromIDList(lParam, path);
                  NativeMethods.SendMessage(hwnd, NativeMethods.FolderBrowserDialogMessage.EnableOk, IntPtr.Zero, validPath ? new IntPtr(1) : IntPtr.Zero);
               }
               break;
         }
         return 0;
      }
   }
}
