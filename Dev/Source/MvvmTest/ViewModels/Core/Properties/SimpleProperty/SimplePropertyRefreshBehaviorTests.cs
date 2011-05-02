namespace Inspiring.MvvmTest.ViewModels.Core.Properties.SimpleProperty {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.ViewModels.Core;
using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class SimplePropertyChangeNotifierBehaviorTests {
      private SimplePropertyChangeNotifierBehavior Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }
      private IVMPropertyDescriptor Property { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new SimplePropertyChangeNotifierBehavior();
         
         Property = PropertyStub
            .WithBehaviors(Behavior)
            .Build();

         Context = ViewModelStub
            .WithProperties(Property)
            .BuildContext();
      }

      [TestMethod]
      public void Refresh_CallsNotifyChange() {
         Behavior.Refresh(Context);
         var expectedChangeArgs = ChangeArgs.PropertyChanged(Property);
         DomainAssert.AreEqual(new[] { expectedChangeArgs }, Context.NotifyChangeInvocations);
      }
   }
}