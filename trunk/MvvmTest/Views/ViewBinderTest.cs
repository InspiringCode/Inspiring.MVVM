using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.Views;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Views {
   [TestClass]
   public class ViewBinderTest {
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
