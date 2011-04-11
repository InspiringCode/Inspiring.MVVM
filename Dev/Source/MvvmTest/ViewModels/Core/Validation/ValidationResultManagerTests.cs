namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationResultManagerTests : ValidationTestBase {
      private ValidationResultManager Manager { get; set; }
      private ViewModelStub VM { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         var behavior = new TestBehavior();
         VM = ViewModelStub.WithBehaviors(behavior).Build();
         Manager = behavior.Manager;
         Context = new BehaviorContextStub(VM);
      }

      [TestMethod]
      public void GetValidationResult_Intially_ReturnsValidResult() {
         Assert.IsTrue(Manager.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void UpdateValidationResult_GetValidationResultReturnsNewResult() {
         var newResult = CreateValidationResult("New error");
         Manager.UpdateValidationResult(Context, newResult);
         Assert.AreEqual(newResult, Manager.GetValidationResult(Context));
      }

      [TestMethod]
      public void UpdateValidationResult_IfNewResultDoesNotEqualOldResult_CallsNotifyChange() {
         var oldResult = CreateValidationResult("First result");
         Manager.UpdateValidationResult(Context, oldResult);
         Assert.AreEqual(1, Context.NotifyChangeInvocations.Count);

         var newResult = CreateValidationResult("Second result");
         Manager.UpdateValidationResult(Context, newResult);

         Assert.AreEqual(2, Context.NotifyChangeInvocations.Count);
      }

      [TestMethod]
      public void UpdateValidationResult_IfNewResultEqualsOldResult_DoesNotCallNotifyChange() {
         var oldResult = CreateValidationResult("Equal result");
         Manager.UpdateValidationResult(Context, oldResult);
         Context.NotifyChangeInvocations.Clear();

         var newResult = CreateValidationResult("Equal result");
         Manager.UpdateValidationResult(Context, newResult);

         Assert.AreEqual(0, Context.NotifyChangeInvocations.Count);
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