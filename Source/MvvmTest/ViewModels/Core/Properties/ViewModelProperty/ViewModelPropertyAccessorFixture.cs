namespace Inspiring.MvvmTest.ViewModels.Core.Properties.ViewModelProperty {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyAccessorFixture : TestBase {
      protected class ChildSource { }

      protected class ChildVM : ViewModelStub, IHasSourceObject<ChildSource> {
         public ChildVM() {
         }

         public ChildVM(RefreshDetectorBehavior behavior)
            : base(DescriptorStub
               .WithBehaviors(behavior)
               .Build()) {
         }

         public ChildSource Source { get; set; }
      }

      protected class RefreshDetectorBehavior : Behavior, IRefreshControllerBehavior {
         public bool WasCalled { get; set; }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property, RefreshOptions options) {
            WasCalled = true;
         }

         public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
            WasCalled = true;
         }
      }
   }
}