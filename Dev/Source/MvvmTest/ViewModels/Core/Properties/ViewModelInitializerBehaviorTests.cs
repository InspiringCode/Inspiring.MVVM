namespace Inspiring.MvvmTest.ViewModels.Core.PropertyBehaviors {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelInitializerBehaviorTests : TestBase {
      private ViewModelInitializerBehavior<IViewModel> Behavior { get; set; }
      private NextBehaviors Next { get; set; }
      private IBehaviorContext Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new ViewModelInitializerBehavior<IViewModel>();
         Next = new NextBehaviors();
         Context = PropertyStub
            .WithBehaviors(Behavior, Next)
            .GetContext();
      }

      [TestMethod]
      public void InitializeValue_AddsParent() {
         var initialValue = new ViewModelStub();
         Next.ValueToReturn = initialValue;
         Behavior.InitializeValue(Context);
         Assert.IsTrue(initialValue.Kernel.Parents.Contains(Context.VM));
      }

      [TestMethod]
      public void SetValue_PreviousValueWasNull_AddsParentToNewValue() {
         var newValue = new ViewModelStub();
         Next.ValueToReturn = null;
         Behavior.SetValue(Context, newValue);
         Assert.IsTrue(newValue.Kernel.Parents.Contains(Context.VM));
      }

      [TestMethod]
      public void SetValue_RemovesParentFromPreviousValue() {
         var previousValue = new ViewModelStub();
         previousValue.Kernel.Parents.Add(Context.VM);
         Next.ValueToReturn = previousValue;

         Behavior.SetValue(Context, null);

         Assert.IsFalse(previousValue.Kernel.Parents.Contains(Context.VM));
      }

      private class NextBehaviors :
         Behavior,
         IValueAccessorBehavior<IViewModel>,
         IValueInitializerBehavior {

         public IViewModel ValueToReturn { get; set; }
         public IViewModel LastSetValue { get; set; }
         public bool InitializeValueWasCalled { get; set; }

         public IViewModel GetValue(IBehaviorContext context) {
            return ValueToReturn;
         }

         public void SetValue(IBehaviorContext context, IViewModel value) {
            LastSetValue = value;
         }

         public void InitializeValue(IBehaviorContext context) {
            InitializeValueWasCalled = true;
         }
      }
   }
}