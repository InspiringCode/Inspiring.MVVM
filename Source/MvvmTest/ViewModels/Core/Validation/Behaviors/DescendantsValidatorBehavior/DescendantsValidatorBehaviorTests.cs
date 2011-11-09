namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using System;
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DescendantsValidatorBehaviorTests : DescendantsValidatorBehaviorFixture {
      private TestDescendantsValidationBehavior Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new TestDescendantsValidationBehavior(ActionLogBuilder);

         var property = PropertyStub
            .WithBehaviors(Behavior, Next)
            .Of<string>();

         Context = CreateBehaviorContextFor(property);
      }

      [TestMethod]
      public void InitializeValue_OnUnloadedDescendant_OnlyCallsNextBehavior() {
         SetupUnloaded();
         Behavior.InitializeValue(Context);
         Assert.AreEqual(InitializeValueNext, ActionLog);
      }

      [TestMethod]
      public void Refresh_OnUnloadedDescendant_OnlyCallsNextBehavior() {
         SetupUnloaded();
         InvokeRefresh();
         Assert.AreEqual(RefreshNext, ActionLog);
      }

      [TestMethod]
      public void RevalidateForLoadedDescendants_OnUnloadedDescendant_OnlyCallsNextBehavior() {
         SetupUnloaded();
         Behavior.RevalidateDescendants(Context, ValidationScope.SelfAndLoadedDescendants);
         Assert.AreEqual(RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void RevalidateForAllDescendants_OnUnloadedDescendant_RevalidatesOnce() {
         SetupUnloaded();
         Behavior.RevalidateDescendants(Context, ValidationScope.SelfAndAllDescendants);
         Assert.AreEqual(RevalidateCore + InitializeValueNext + RevalidateNext, ActionLog);
         Assert.AreEqual(ValidationScope.SelfAndAllDescendants, Behavior.LastValidationScope);
      }

      [TestMethod]
      public void InitializeValue_OnUnloadedValidatedDescendant_RevalidatesOnceWithLastScope() {
         // The last scope can only be LoadedDescendants because AllDescendants
         // would trigger a load.
         SetupUnloadedAndValidated();
         Behavior.InitializeValue(Context);
         Assert.AreEqual(InitializeValueNext + RevalidateCore, ActionLog);
         Assert.AreEqual(ValidationScope.SelfAndLoadedDescendants, Behavior.LastValidationScope);
      }

      [TestMethod]
      public void Refresh_OnUnloadedValidatedDescendant_OnlyCallsNextBehavior() {
         SetupUnloadedAndValidated();
         InvokeRefresh();
         Assert.AreEqual(RefreshNext, ActionLog);
      }

      [TestMethod]
      public void RevalidateForLoadedDescendants_OnUnloadedValidatedDescendant_OnlyCallsNextBehavior() {
         SetupUnloadedAndValidated();
         Behavior.RevalidateDescendants(Context, ValidationScope.SelfAndLoadedDescendants);
         Assert.AreEqual(RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void RevalidateForAllDescendants_OnUnloadedValidatedDescendant_RevalidatesOnce() {
         SetupUnloadedAndValidated();
         Behavior.RevalidateDescendants(Context, ValidationScope.SelfAndAllDescendants);
         Assert.AreEqual(RevalidateCore + InitializeValueNext + RevalidateNext, ActionLog);
         Assert.AreEqual(ValidationScope.SelfAndAllDescendants, Behavior.LastValidationScope);
      }

      [TestMethod]
      public void InitializeValue_OnLoadedAndValidatedDescendant_OnlyCallsNextBehavior() {
         SetupLoadedAndValidated();
         Behavior.InitializeValue(Context);
         Assert.AreEqual(InitializeValueNext, ActionLog);
      }

      [TestMethod]
      public void Refresh_OnLoadedAndValidatedDescendant_RevalidatesOnceWithLastScope() {
         var relevantScopes = new[] {
            ValidationScope.SelfAndLoadedDescendants, 
            ValidationScope.SelfAndAllDescendants 
         };

         foreach (var lastScope in relevantScopes) {
            SetupLoadedAndValidated(lastScope);

            InvokeRefresh();
            Assert.AreEqual(RefreshNext + RevalidateCore, ActionLog);
            Assert.AreEqual(lastScope, Behavior.LastValidationScope);
         }
      }

      [TestMethod]
      public void Revalidate_OnLoadedAndValidatedDescendant_RevalidatesOnce() {
         var relevantScopes = new[] {
            ValidationScope.SelfAndLoadedDescendants, 
            ValidationScope.SelfAndAllDescendants 
         };

         foreach (var scope in relevantScopes) {
            SetupLoadedAndValidated(scope);

            Behavior.RevalidateDescendants(Context, scope);
            Assert.AreEqual(RevalidateCore + RevalidateNext, ActionLog);
            Assert.AreEqual(scope, Behavior.LastValidationScope);
         }
      }

      internal override DescendantsValidatorBehavior GetConcreteBehavior() {
         return Behavior;
      }

      internal override IBehaviorContext GetContext() {
         return Context;
      }

      private void InvokeRefresh() {
         Behavior.Refresh(Context, new RefreshOptions());
      }

      private class TestDescendantsValidationBehavior : DescendantsValidatorBehavior {
         public TestDescendantsValidationBehavior(StringBuilder log) {
            Log = log;
         }

         public ValidationScope LastValidationScope { get; private set; }
         private StringBuilder Log { get; set; }

         protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
            Log.Append(RevalidateCore);

            // Simulate a validator that triggers lazy loading.
            if (!this.IsLoadedNext(context)) {
               InitializeValue(context);
            }

            LastValidationScope = scope;
         }

         protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
            throw new NotSupportedException();
         }
      }
   }

   [TestClass]
   public class CollectionDescendantsValidationBehaviorTests : DescendantsValidatorBehaviorFixture {

      internal override DescendantsValidatorBehavior GetConcreteBehavior() {
         throw new NotImplementedException();
      }

      internal override IBehaviorContext GetContext() {
         throw new NotImplementedException();
      }
   }
}
