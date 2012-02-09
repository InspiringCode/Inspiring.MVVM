using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace InstanceStarter {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();
      }

      private void HandleStartInstancesClick(object sender, RoutedEventArgs e) {
         string currentExePath = Assembly.GetExecutingAssembly().Location;
         string currentDir = Path.GetDirectoryName(currentExePath);
         string otherExePath = Path.Combine(currentDir, "MainProgram.exe");

         for (int i = 0; i < 5; i++) {
            string message = String.Format("Message-{0}-{1}", i, DateTime.Now.Ticks);
            Process.Start(otherExePath, message);
         }
      }
   }
}
