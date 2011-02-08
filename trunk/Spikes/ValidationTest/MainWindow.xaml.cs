using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ValidationTest {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();

         Validation.AddErrorHandler(_tabItem1, HandleError1);
         Validation.AddErrorHandler(_tabItem2, HandleError2);

         DataContext = new Company();


      }

      private void HandleError1(object sender, ValidationErrorEventArgs args) {
         //Debug.WriteLine(args.Action + " 1");
      }

      private void HandleError2(object sender, ValidationErrorEventArgs args) {
         Debug.WriteLine(args.Action + " 2");
      }

      private void Button_Click(object sender, RoutedEventArgs e) {
         Debugger.Break();
      }
   }

   public class Company : IDataErrorInfo {
      public Company() {
         Employees = new List<Employee>();

         Employees.Add(new Employee { Name = "John First", Salary = 900 });
         for (int i = 0; i < 500; i++)
			{
            Employees.Add(new Employee { Name = "John " + i, Salary = 1000 + i * 10 });
			}
         Employees.Add(new Employee { Name = "John Last", Salary = 9000 });
                     
         Turnover = 1000;
      }

      public int Turnover { get; set; }

      public List<Employee> Employees { get; set; }

      public string Error {
         get { return null; }
      }

      public string this[string columnName] {
         get {
            if (columnName == "Turnover" && Turnover < 500) {
               return "Fire the sales!";
            }
            return null;
         }
      }
   }

   public class Employee : IDataErrorInfo {
      public string Name { get; set; }
      public int Salary { get; set; }

      public string Error {
         get { return null; }
      }

      public string this[string columnName] {
         get {
            if (columnName == "Salary" && Salary < 1000) {
               return "Salary too loo!";
            }
            return null;
         }
      }
   }
}
