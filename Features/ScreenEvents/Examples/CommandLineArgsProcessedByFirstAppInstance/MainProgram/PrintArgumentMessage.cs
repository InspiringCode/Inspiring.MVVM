namespace MainProgram {
   using System;
   using System.Linq;
   using System.Windows;

   [Serializable]
   internal sealed class PrintArgumentMessage {
      public PrintArgumentMessage(StartupEventArgs e) {
         FirstCommandLineArgument = e.Args.FirstOrDefault() ?? "<NONE>";
      }

      public string FirstCommandLineArgument { get; private set; }
   }
}
