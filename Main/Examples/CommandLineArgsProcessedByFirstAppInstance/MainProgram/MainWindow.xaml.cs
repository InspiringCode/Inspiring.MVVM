using System;
using System.Collections.ObjectModel;
using System.Windows;
using Inspiring.Mvvm.Common;

namespace MainProgram {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public ObservableCollection<string> Messages { get; private set; }

      public MainWindow() {
         InitializeComponent();
         Messages = new ObservableCollection<string>();

         this.Loaded += HandleLoaded;
         DataContext = this;
      }

      protected override void OnClosed(EventArgs e) {
         base.OnClosed(e);

         // You should do this in the OnClose of the ShellScreen!
         App.IpcMessenger.Dispose();
      }

      private void HandleLoaded(object sender, RoutedEventArgs e) {
         App.IpcMessenger.AddMessageReceiver<PrintArgumentMessage>(msg => {
            Messages.Add(msg.FirstCommandLineArgument);
         });

         App.IpcMessenger.DispatchMessages(DispatchTarget.LocalProcess);
      }
   }
}
