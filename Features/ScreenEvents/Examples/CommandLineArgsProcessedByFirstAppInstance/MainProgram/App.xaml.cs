using System.Windows;
using Inspiring.Mvvm.Common;

namespace MainProgram {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      static App() {
         IpcMessenger = new InterprocessMessenger();
      }

      /// <summary>
      ///   You should use Dependency Injection or some other mechanism in your app!
      /// </summary>
      public static InterprocessMessenger IpcMessenger { get; private set; }

      protected override void OnStartup(StartupEventArgs e) {
         bool continueStartup;
         IpcMessenger.EnqueueMessage(new PrintArgumentMessage(e));

         using (new InterprocessLock()) {
            IpcMessenger.DispatchMessages(DispatchTarget.FirstOtherProcess);
            continueStartup = IpcMessenger.HasUndispatchedMessages;

            if (continueStartup) {
               IpcMessenger.StartListening();
            }
         }

         if (continueStartup) {
            base.OnStartup(e);
         } else {
            Shutdown();
         }
      }
   }
}
