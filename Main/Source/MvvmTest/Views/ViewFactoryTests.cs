namespace Inspiring.MvvmTest.Views {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.Views;

   [TestClass]
   public class ViewFactoryTests {
      [TestMethod]
      public void TryInitializeView_WorksWithModelInterfaces() {
         var view = new TestView();
         var vm = new TestVM();

         Assert.IsTrue(ViewFactory.TryInitializeView(view, vm));
         Assert.AreEqual(vm, view.Model);
      }

      public class TestView : IView<IViewModel<TestVMDescriptor>> {
         public IViewModel<TestVMDescriptor> Model { get; set; }
      }

      public class TestVM : ViewModel<TestVMDescriptor> {
         public TestVM()
            : base(new TestVMDescriptor()) {
         }
      }

      public class TestVMDescriptor : VMDescriptor { }

      public class GenericTestVMDescriptor<T> : TestVMDescriptor { }
   }
}