namespace Inspiring.MvvmTest.ViewModels.Core.Properties.ViewModelProperty {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Rename, Delegate is not correct
   [TestClass]
   public class DelegateViewModelAccessorBehaviorTests : ViewModelPropertyAccessorFixture {
      private ValueAccessorStub<ChildVM> ValueAccessor { get; set; }
      private DelegateViewModelAccessorBehavior<ChildVM> Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }
      private PropertyStub<object> Property { get; set; }

      [TestInitialize]
      public void Setup() {
         ValueAccessor = new ValueAccessorStub<ChildVM>();
         Behavior = new DelegateViewModelAccessorBehavior<ChildVM>();

         Property = PropertyStub
            .WithBehaviors(Behavior, ValueAccessor)
            .Build();

         Context = ViewModelStub
            .WithProperties(Property)
            .BuildContext();
      }

      [TestMethod]
      public void Refresh_ViewModelInstanceHasChanged_RaisesNotifyChange() {
         var oldChild = new ChildVM();
         var newChild = new ChildVM();

         ValueAccessor.Value = oldChild;
         Behavior.GetValue(Context);
         ValueAccessor.Value = newChild;
         Behavior.Refresh(Context, false);

         var expectedChangeArgs = ChangeArgs.ViewModelPropertyChanged(Property, oldChild, newChild, null);
         DomainAssert.AreEqual(new[] { expectedChangeArgs }, Context.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Refresh_ViewModelInstanceHasNotChanged_DoesNotRaiseNotifyChange() {
         ValueAccessor.Value = new ChildVM();
         Behavior.GetValue(Context);
         Behavior.Refresh(Context, false);
         Assert.IsFalse(Context.NotifyChangeInvocations.Any());
      }
   }
}