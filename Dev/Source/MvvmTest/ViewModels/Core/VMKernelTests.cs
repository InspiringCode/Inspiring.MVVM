namespace Inspiring.MvvmTest.ViewModels.Core {
   using Inspiring.Mvvm;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMKernelTests : TestBase {
      [TestMethod]
      public void NotifyChange_CallsNotifyChangeOfViewModel() {
         var vm = ViewModelStub.Build();

         var args = CreateChangeArgs();
         IBehaviorContext kernel = CreateKernel(vm);
         kernel.NotifyChange(args);

         DomainAssert.AreEqual(args, vm.NotifyChangeInvocations.SingleOrDefault());
      }

      [TestMethod]
      public void NotifyChange_CallsParentWithExtendedPath() {
         var parentVM = ViewModelStub.Build();

         IBehaviorContext kernel = CreateKernel();
         var parent = CreateKernel(parentVM);

         var args = CreateChangeArgs();
         kernel.NotifyChange(args);

         var expectedArgs = args.PrependViewModel(parentVM);
         DomainAssert.AreEqual(expectedArgs, parentVM.NotifyChangeInvocations.SingleOrDefault());         
      }

      private static ChangeArgs CreateChangeArgs() {
         return new ChangeArgs(
            ChangeType.PropertyChanged,
            ViewModelStub.Build(),
            PropertyStub.Of<object>()
         );
      }

      private static VMKernel CreateKernel(IViewModel vm = null) {
         vm = vm ?? ViewModelStub.Build();
         return new VMKernel(vm, vm.Descriptor, ServiceLocator.Current);
      }
   }
}