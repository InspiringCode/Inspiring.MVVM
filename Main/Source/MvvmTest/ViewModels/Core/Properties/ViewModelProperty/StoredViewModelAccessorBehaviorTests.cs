namespace Inspiring.MvvmTest.ViewModels.Core.Properties.ViewModelProperty {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class StoredViewModelAccessorBehaviorTests : ViewModelPropertyAccessorFixture {
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

         InvokeRefresh();
         Assert.IsTrue(refreshDetector.WasCalled);
      }

      [TestMethod]
      public void Refresh_WhenValueIsNull_DoesNothing() {
         InvokeRefresh();
      }

      [TestMethod]
      public void Refresh_DoesNotRaiseNotifyPropertyChanged() {
         InvokeRefresh();
         Assert.IsFalse(Context.NotifyChangeInvocations.Any());
      }

      private void InvokeRefresh() {
         Behavior.Refresh(Context, new RefreshOptions());
      }
   }
}