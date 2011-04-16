namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationResultManagerTests : ValidationTestBase {
      private ValidationResultManager Manager { get; set; }
      private ViewModelStub VM { get; set; }
      private BehaviorContextStub Context { get; set; }
      private IVMPropertyDescriptor Property { get; set; }

      [TestInitialize]
      public void Setup() {
         var behavior = new TestBehavior();
         VM = ViewModelStub.WithBehaviors(behavior).Build();
         Manager = behavior.Manager;
         Context = new BehaviorContextStub(VM);
      }

      [TestMethod]
      public void GetValidationResult_Intially_ReturnsValidResult() {
         SetupForViewModel();

         Assert.IsTrue(Manager.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void UpdateValidationResult_GetValidationResultReturnsNewResult() {
         SetupForViewModel();

         var newResult = CreateValidationResult("New error");
         Manager.UpdateValidationResult(Context, newResult);
         Assert.AreEqual(newResult, Manager.GetValidationResult(Context));
      }

      [TestMethod]
      public void UpdateValidationResult_IfNewResultDoesNotEqualOldResult_CallsNotifyChange() {
         SetupForViewModel();

         var oldResult = CreateValidationResult("First result");
         Manager.UpdateValidationResult(Context, oldResult);
         Assert.AreEqual(1, Context.NotifyChangeInvocations.Count);

         var newResult = CreateValidationResult("Second result");
         Manager.UpdateValidationResult(Context, newResult);

         Assert.AreEqual(2, Context.NotifyChangeInvocations.Count);
      }

      [TestMethod]
      public void UpdateValidationResult_AsViewModelBehavior_CallsNotifyChangeWithCorrectArgs() {
         SetupForViewModel();

         var oldResult = CreateValidationResult("New result");

         Manager.UpdateValidationResult(Context, oldResult);

         DomainAssert.AreEqual(
            ChangeArgs.ValidationStateChanged(),
            Context.NotifyChangeInvocations.LastOrDefault()
         );
      }

      [TestMethod]
      public void UpdateValidationResult_AsPropertyBehavior_CallsNotifyChangeWithCorrectArgs() {
         SetupForProperty();

         var oldResult = CreateValidationResult("New result");

         Manager.UpdateValidationResult(Context, oldResult);

         DomainAssert.AreEqual(
            ChangeArgs.ValidationStateChanged(Property),
            Context.NotifyChangeInvocations.LastOrDefault()
         );
      }

      [TestMethod]
      public void UpdateValidationResult_IfNewResultEqualsOldResult_DoesNotCallNotifyChange() {
         SetupForViewModel();

         var oldResult = CreateValidationResult("Equal result");
         Manager.UpdateValidationResult(Context, oldResult);
         Context.NotifyChangeInvocations.Clear();

         var newResult = CreateValidationResult("Equal result");
         Manager.UpdateValidationResult(Context, newResult);

         Assert.AreEqual(0, Context.NotifyChangeInvocations.Count);
      }

      private void SetupForViewModel() {
         var behavior = new TestBehavior();
         VM = ViewModelStub.WithBehaviors(behavior).Build();
         Manager = behavior.Manager;
         Context = new BehaviorContextStub(VM);
      }

      private void SetupForProperty() {
         var behavior = new TestBehavior();
         Property = PropertyStub.WithBehaviors(behavior).Of<object>();
         VM = ViewModelStub.WithProperties(Property).Build();
         Manager = behavior.Manager;
         Context = new BehaviorContextStub(VM);
      }

      private class TestBehavior : Behavior, IBehaviorInitializationBehavior {
         private static readonly FieldDefinitionGroup ResultGroup = new FieldDefinitionGroup();

         public ValidationResultManager Manager { get; private set; }

         public void Initialize(BehaviorInitializationContext context) {
            Manager = new ValidationResultManager(context, ResultGroup);
         }
      }
   }
}