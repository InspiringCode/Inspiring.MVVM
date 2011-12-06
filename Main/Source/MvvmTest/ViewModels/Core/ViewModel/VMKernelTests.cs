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

      [TestMethod]
      public void NotifyChange_CallsHandlePropertyChangedBehaviorOnlyIfOwnPropertyHasChanged() {
         var mock = new PropertyChangedMock();

         var property = PropertyStub
            .WithBehaviors(mock)
            .Build();

         var vm = ViewModelStub
            .WithProperties(property)
            .Build();

         var context = vm.GetContext();

         var args = ChangeArgs.PropertyChanged(property, ValueStage.ValidatedValue);
         context.NotifyChange(args);
         Assert.IsTrue(mock.PropertyChangedWasCalled);

         mock.PropertyChangedWasCalled = false;
         args = ChangeArgs
            .PropertyChanged(property, ValueStage.ValidatedValue)
            .PrependViewModel(ViewModelStub.Build());
         context.NotifyChange(args);
         Assert.IsFalse(mock.PropertyChangedWasCalled);

         mock.PropertyChangedWasCalled = false;
         args = ChangeArgs.ValidationResultChanged(property, ValueStage.Value);
         context.NotifyChange(args);
         Assert.IsFalse(mock.PropertyChangedWasCalled);

         mock.PropertyChangedWasCalled = false;
         args = ChangeArgs.ItemsAdded(VMCollectionStub.Build(), new[] { ViewModelStub.Build() });
         context.NotifyChange(args);
         Assert.IsFalse(mock.PropertyChangedWasCalled);
      }

      private static bool HandlePropertyChangedBehaviorWasCalled(ChangeArgs args) {
         var mock = new PropertyChangedMock();

         var vm = ViewModelStub
            .WithProperties(PropertyStub
               .WithBehaviors(mock)
               .Build())
            .Build();

         vm.GetContext().NotifyChange(args);

         return mock.PropertyChangedWasCalled;
      }

      private static ChangeArgs CreateChangeArgs() {
         return ChangeArgs.PropertyChanged(PropertyStub.Build(), ValueStage.ValidatedValue);
      }

      private static VMKernel CreateKernel(IViewModel vm = null) {
         vm = vm ?? ViewModelStub.Build();
         return new VMKernel(vm, vm.Descriptor, ServiceLocator.Current);
      }

      private class PropertyChangedMock : Behavior, IHandlePropertyChangedBehavior {
         public bool PropertyChangedWasCalled { get; set; }

         public void HandlePropertyChanged(IBehaviorContext context, ChangeArgs args) {
            PropertyChangedWasCalled = true;
         }
      }
   }
}