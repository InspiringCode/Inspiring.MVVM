namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelTest {
      [TestMethod]
      public void OnValidating() {
         TestVM vm = new TestVM();
         IBehaviorContext context = vm;

         var args = new ValidationEventArgs(TestVM.Descriptor.LocalProperty, 0.0m, vm);

         bool validatingCalled = false;

         vm.Validating += (sender, e) => {
            validatingCalled = true;
            Assert.AreEqual(args, e);
         };

         context.OnValidating(args);
         Assert.IsTrue(validatingCalled);
      }

      [TestMethod]
      public void OnValidated() {
         TestVM vm = new TestVM();
         IBehaviorContext context = vm;

         var args = new ValidationEventArgs(TestVM.Descriptor.LocalProperty, 0.0m, vm);

         bool validatedCalled = false;

         vm.Validated += (sender, e) => {
            validatedCalled = true;
            Assert.AreEqual(args, e);
         };

         context.OnValidated(args);
         Assert.IsTrue(validatedCalled);
      }
   }
}