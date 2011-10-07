namespace Inspiring.MvvmTest.ViewModels.Core.Properties.SimpleProperty {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimplePropertyChangeNotifierBehaviorTests {
      private RefreshablePropertyChangedNotifierBehavior<object> Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }
      private IVMPropertyDescriptor Property { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new RefreshablePropertyChangedNotifierBehavior<object>();

         Property = PropertyStub
            .WithBehaviors(Behavior)
            .Build();

         Context = ViewModelStub
            .WithProperties(Property)
            .BuildContext();
      }

      [TestMethod]
      public void Refresh_CallsNotifyChange() {
         Behavior.Refresh(Context, false);
         var expectedChangeArgs = ChangeArgs.PropertyChanged(Property, null);
         DomainAssert.AreEqual(new[] { expectedChangeArgs }, Context.NotifyChangeInvocations);
      }
   }
}