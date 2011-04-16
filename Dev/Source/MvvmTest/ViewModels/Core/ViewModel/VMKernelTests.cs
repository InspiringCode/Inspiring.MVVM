namespace Inspiring.MvvmTest.ViewModels.Core.ViewModel {
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMKernelTests : TestBase {
      [TestMethod]
      public void NotifyChange_CallsNotifyChangeOfViewModelWithPrependedViewModel() {
         var vm = ViewModelStub.Build();

         var args = CreateChangeArgs();
         IBehaviorContext kernel = CreateKernel(vm);
         kernel.NotifyChange(args);

         var expectedArgs = args.PrependViewModel(vm);
         DomainAssert.AreEqual(expectedArgs, vm.NotifyChangeInvocations.SingleOrDefault());
      }

      [TestMethod]
      public void NotifyChange_CallsParentWithExtendedPath() {
         var parentVM = ViewModelStub.Build();
         var vm = ViewModelStub.Build();


         var parentKernel = CreateKernel(parentVM);
         var kernel = CreateKernel(vm);
         kernel.Parents.Add(parentVM);

         IBehaviorContext kernelContext = kernel;

         var args = CreateChangeArgs();
         kernelContext.NotifyChange(args);

         var expectedArgs = args
            .PrependViewModel(vm)
            .PrependViewModel(parentVM);

         DomainAssert.AreEqual(expectedArgs, parentVM.NotifyChangeInvocations.SingleOrDefault());
      }

      private static ChangeArgs CreateChangeArgs() {
         return ChangeArgs.PropertyChanged(PropertyStub.Build());
      }

      private static VMKernel CreateKernel(IViewModel vm = null) {
         vm = vm ?? ViewModelStub.Build();
         return new VMKernel(vm, vm.Descriptor, ServiceLocator.Current);
      }
   }
}