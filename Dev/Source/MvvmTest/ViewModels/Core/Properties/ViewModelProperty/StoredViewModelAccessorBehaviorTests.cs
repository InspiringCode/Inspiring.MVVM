namespace Inspiring.MvvmTest.ViewModels.Core.Properties.ViewModelProperty {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class StoredViewModelAccessorBehaviorTests {
      private StoredViewModelAccessorBehavior<ChildVM> Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new StoredViewModelAccessorBehavior<ChildVM>();

         Context = PropertyStub
            .WithBehaviors(Behavior)
            .GetContext();
      }

      [TestMethod]
      public void Refresh_RefreshesChildVM() {
         var refreshDetector = new RefreshDetectorBehavior();
         var child = new ChildVM(refreshDetector);
         Behavior.SetValue(Context, child);

         Behavior.Refresh(Context);
         Assert.IsTrue(refreshDetector.WasCalled);
      }

      [TestMethod]
      public void Refresh_WhenValueIsNull_DoesNothing() {
         Behavior.Refresh(Context);
      }

      private class ChildVM : ViewModelStub {
         public ChildVM(RefreshDetectorBehavior behavior)
            : base(DescriptorStub
               .WithBehaviors(behavior)
               .Build()) {
         }
      }

      private class RefreshDetectorBehavior : Behavior, IRefreshControllerBehavior {
         public bool WasCalled { get; set; }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
            WasCalled = true;
         }

         public void Refresh(IBehaviorContext context) {
            WasCalled = true;
         }
      }
   }
}