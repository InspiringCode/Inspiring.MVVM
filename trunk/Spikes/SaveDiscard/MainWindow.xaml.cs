using System.Windows;

namespace SaveDiscard {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window, IView<EmployeeScreen> {
      public MainWindow() {
         InitializeComponent();
      }

      public EmployeeScreen Model {
         set { DataContext = value; }
      }
   }
}
