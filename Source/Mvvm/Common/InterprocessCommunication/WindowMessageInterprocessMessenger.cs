namespace Inspiring.Mvvm.Common.Core {
   using System;
   using System.IO;
   using System.Runtime.InteropServices;
   using System.Runtime.Serialization.Formatters.Binary;

   public abstract class WindowMessageInterprocessMessenger : AbstractInterprocessMessenger {
      private const int CopyDataResultSuccess = 0xFFFF;

      public WindowMessageInterprocessMessenger(string sharedIdentifier = null)
         : base(sharedIdentifier) {
      }

      protected abstract bool IsOwnWindow(IntPtr windowHandle);

      protected IntPtr WindowMessageHook(
         int msg,
         IntPtr wParam,
         IntPtr lParam,
         ref bool handled
      ) {
         handled = false;

         if (msg != NativeMethods.WM_COPYDATA) {
            return IntPtr.Zero;
         }

         MessageEnvelope envelope = EnvelopeCopyDataStruct.TryUnmarshalEnvelope(lParam);

         if (envelope != null && envelope.SharedIdentifier == SharedIdentifier) {
            InvokeMessageHandlers(envelope.Message);
            handled = true;
            return new IntPtr(CopyDataResultSuccess);
         }

         return IntPtr.Zero;
      }

      protected override bool DispatchMessage(object message, DispatchTarget target) {
         if (target == DispatchTarget.LocalProcess) {
            InvokeMessageHandlers(message);
            return true;
         }

         bool successfullyDispatched = false;
         NativeMethods.EnumWindowsProc enumProc = null;

         switch (target) {
            case DispatchTarget.FirstOtherProcess:
               enumProc = delegate(IntPtr hWnd, IntPtr lParam) {
                  if (!IsOwnWindow(hWnd)) {
                     bool wasHandled = SendMessage(hWnd, message);

                     if (wasHandled) {
                        successfullyDispatched = true;
                        return NativeMethods.StopWindowEnumeration;
                     }
                  }

                  return NativeMethods.ContinueWindowEnumeration;
               };
               break;
            case DispatchTarget.AllOtherProcesses:
               successfullyDispatched = true;
               enumProc = delegate(IntPtr hWnd, IntPtr lParam) {
                  if (!IsOwnWindow(hWnd)) {
                     SendMessage(hWnd, message);
                  }

                  return NativeMethods.ContinueWindowEnumeration;
               };
               break;
            default:
               throw new NotSupportedException();
         }

         NativeMethods.EnumWindows(enumProc, lParam: IntPtr.Zero);
         return successfullyDispatched;
      }

      private bool SendMessage(IntPtr targetWindowHandle, object message) {
         var envelope = new MessageEnvelope(SharedIdentifier, message);

         using (EnvelopeCopyDataStruct cds = new EnvelopeCopyDataStruct(envelope)) {

            int result;
            int success = NativeMethods.SendMessageTimeout(
               hWnd: targetWindowHandle,
               Msg: NativeMethods.WM_COPYDATA,
               wParam: IntPtr.Zero,
               lParam: cds.Pointer,
               fuFlags: NativeMethods.SMTO_BLOCK | NativeMethods.SMTO_ABORTIFHUNG,
               uTimeout: 10000,
               lpdwResult: out result
            );

            return result == CopyDataResultSuccess;
         }
      }

      [Serializable]
      private class MessageEnvelope {
         public MessageEnvelope(string sharedIdentifier, object message) {
            SharedIdentifier = sharedIdentifier;
            Message = message;
         }

         public string SharedIdentifier { get; private set; }
         public object Message { get; private set; }
      }

      private class EnvelopeCopyDataStruct : IDisposable {
         private const int StructIdentifier = 0x00ABCDEF;

         private readonly IntPtr _messageBuffer = IntPtr.Zero;
         private readonly IntPtr _structBuffer = IntPtr.Zero;

         public EnvelopeCopyDataStruct(MessageEnvelope envelope) {
            using (MemoryStream stream = new MemoryStream()) {
               BinaryFormatter formatter = new BinaryFormatter();
               formatter.Serialize(stream, envelope);

               byte[] bytes = stream.ToArray();
               _messageBuffer = Marshal.AllocCoTaskMem(bytes.Length);
               Marshal.Copy(bytes, 0, _messageBuffer, bytes.Length);

               COPYDATASTRUCT cds = new COPYDATASTRUCT();
               cds.dwData = StructIdentifier;
               cds.cbData = bytes.Length;
               cds.lpData = _messageBuffer;

               _structBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
               Marshal.StructureToPtr(cds, _structBuffer, true);
            }
         }

         public IntPtr Pointer {
            get { return _structBuffer; }
         }

         public static MessageEnvelope TryUnmarshalEnvelope(IntPtr pointer) {
            var cds = (COPYDATASTRUCT)Marshal.PtrToStructure(
                pointer,
                typeof(COPYDATASTRUCT)
            );

            if (cds.dwData != StructIdentifier) {
               return null;
            }

            byte[] bytes = new byte[cds.cbData];
            Marshal.Copy(cds.lpData, bytes, 0, cds.cbData);

            using (MemoryStream stream = new MemoryStream(bytes)) {
               BinaryFormatter formatter = new BinaryFormatter();
               object packet = formatter.Deserialize(stream);
               return packet as MessageEnvelope;
            }
         }

         public void Dispose() {
            if (_messageBuffer != IntPtr.Zero) {
               Marshal.FreeCoTaskMem(_messageBuffer);
            }

            if (_structBuffer != IntPtr.Zero) {
               Marshal.FreeCoTaskMem(_structBuffer);
            }
         }

         [StructLayout(LayoutKind.Sequential)]
         public struct COPYDATASTRUCT {
            public int dwData;
            public int cbData;
            public IntPtr lpData;
         }
      }

      private static class NativeMethods {
         public const int WM_COPYDATA = 0x004A;

         public const int SMTO_ABORTIFHUNG = 0x0002;
         public const int SMTO_BLOCK = 0x0001;

         public const int StopWindowEnumeration = 0;
         public const int ContinueWindowEnumeration = 1;

         public delegate int EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

         [DllImport("user32.dll", CharSet = CharSet.Auto)]
         public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

         [DllImport("user32.dll", CharSet = CharSet.Auto)]
         public static extern int SendMessageTimeout(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            IntPtr lParam,
            int fuFlags,
            int uTimeout,
            out int lpdwResult
         );
      }
   }
}
