namespace Inspiring.MvvmExample.Views {
   using System.Windows.Controls;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmExample.Screens;

   public partial class ShellView : UserControl, IView<ShellScreen> {
      public ShellView() {
         InitializeComponent();
      }

      public ShellScreen Model {
         set { DataContext = value; }
      }
   }
}
