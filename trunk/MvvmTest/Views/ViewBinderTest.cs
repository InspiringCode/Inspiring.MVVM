using System.Windows.Controls;
using System.Windows.Data;
using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.Views;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Views {
   [TestClass]
   public class ViewBinderTest {
      [TestMethod]
      public void BindScreen() {
         PersonScreenView screenView = new PersonScreenView();
         TextBox targetElement = new TextBox();

         ViewBinder.BindScreen<PersonScreen>(screenView, c => {
            c.BindVM(x => x.VM, b => {
               b.Property(x => x.FirstName).To(targetElement).On(TextBox.TextProperty);
            });
         });

         Binding binding = BindingOperations.GetBinding(targetElement, TextBox.TextProperty);
         Assert.AreEqual("VM.FirstName", binding.Path.Path);
         Assert.IsNull(binding.Source);
      }

      [TestMethod]
      public void TestMethod1() {
         PersonVM vm = SampleDataFactory.CreatePersonVM();
         PersonVMView view = new PersonVMView();
         ViewBinder.BindVM(view, b => {

         });

         PersonScreenView screenView = null;
         ViewBinder.BindScreen(screenView, b => {

         });
      }

      private class PersonScreenView : IView<PersonScreen> {

         public PersonScreen Model {
            set { throw new System.NotImplementedException(); }
         }
      }

      private class PersonVMView : IView<PersonVM> {

         public PersonVM Model {
            set { throw new System.NotImplementedException(); }
         }
      }

      private class PersonScreen : Screen {
         public PersonVM VM { get; private set; }
      }
   }
}
