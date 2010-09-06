using System.Windows.Controls;
using System.Windows.Data;
using Inspiring.Mvvm.Common;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Views {
   [TestClass]
   public class CustomPropertyDescriptorTest {
      private PersonView _view;
      private PersonVM _vm;

      private string ViewFirstName {
         get { return _view._firstName.Text; }
         set {
            _view._firstName.Text = value;
            _view._firstName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
         }
      }

      private string VMFirstName {
         get { return _vm.GetValue(PersonVM.Descriptor.FirstName); }
         set { _vm.SetValue(PersonVM.Descriptor.FirstName, value); }
      }

      [TestInitialize]
      public void Setup() {
         _view = new PersonView();
         _vm = SampleDataFactory.CreatePersonVM();

         WpfTestHelper.SetupDataContext(_view, _vm);
         SetupBinding();
      }

      [TestMethod]
      public void CheckBoundValues() {
         Assert.AreEqual(VMFirstName, ViewFirstName);
      }

      [TestMethod]
      public void UpdateTextBox() {
         ViewFirstName = "Karl";
         Assert.AreEqual(ViewFirstName, VMFirstName);
      }


      [TestMethod]
      public void TestMethod1() {
         var t = new Test { FirstName = "John", LastName = "Doe" };
         PersonView v = new PersonView();
         WpfTestHelper.SetupDataContext(v, t);


         //v._firstName.DataContext = t;
         //v._lastName.DataContext = t;
         v._firstName.SetBinding(TextBox.TextProperty, new Binding("FirstName") { Mode = BindingMode.TwoWay });
         v._lastName.SetBinding(TextBox.TextProperty, "LastName");

         Assert.AreEqual("John", v._firstName.Text);
         Assert.AreEqual("Doe", v._lastName.Text);


         //v._firstName.SelectAll();
         //v._firstName.SelectedText = "Karl";
         v._firstName.Text = "Karl";
         WpfTestHelper.UpdateSource(v, TextBox.TextProperty);
         Assert.AreEqual("Karl", t.FirstName);

         //t.FirstName = "Daniel";
         //t.LastName = "Berghold";

         //Assert.AreEqual("Daniel", v._firstName.Text);
         //Assert.AreEqual("Berghold", v._lastName.Text);
      }

      public class Test : NotifyObject {
         private string _firstName;

         public string FirstName {
            get { return _firstName; }
            set {
               _firstName = value;
               OnPropertyChanged(() => FirstName);
            }
         }

         private string _lastName;

         public string LastName {
            get { return _lastName; }
            set {
               _lastName = value;
               OnPropertyChanged(() => LastName);
            }
         }

      }


      private void SetupBinding() {
         _view._firstName.SetBinding(TextBox.TextProperty, "FirstName");
      }
   }
}
