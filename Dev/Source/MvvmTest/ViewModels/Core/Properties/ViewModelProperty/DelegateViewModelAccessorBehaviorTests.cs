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
         ValueAccessor.Value = new ChildVM();
         Behavior.GetValue(Context);
         ValueAccessor.Value = new ChildVM();
         Behavior.Refresh(Context);

         var expectedChangeArgs = ChangeArgs.PropertyChanged(Property);
         DomainAssert.AreEqual(new[] { expectedChangeArgs }, Context.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Refresh_ViewModelInstanceHasNotChanged_DoesNotRaiseNotifyChange() {
         ValueAccessor.Value = new ChildVM();
         Behavior.GetValue(Context);
         Behavior.Refresh(Context);
         Assert.IsFalse(Context.NotifyChangeInvocations.Any());
      }
   }
}