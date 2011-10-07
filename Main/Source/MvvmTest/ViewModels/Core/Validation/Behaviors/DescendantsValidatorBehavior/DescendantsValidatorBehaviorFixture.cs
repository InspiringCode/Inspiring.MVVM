namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class DescendantsValidatorBehaviorFixture : ValidationTestBase {
      protected const string InitializeValueNext = "InitializeValueNext ";
      protected const string RefreshNext = "RefreshNext ";
      protected const string RevalidateNext = "RevalidateNext ";
      protected const string RevalidateCore = "RevalidateCore ";
      protected const string GetDescendantsValidationResultNext = "GetDescendantsValidationResultNext ";

      internal NextBehaviorsStub Next { get; set; }
      internal StringBuilder ActionLogBuilder { get; set; }

      protected string ActionLog {
         get { return ActionLogBuilder.ToString(); }
      }

      [TestInitialize]
      public void BaseSetup() {
         ActionLogBuilder = new StringBuilder();
         Next = new NextBehaviorsStub(ActionLogBuilder);
      }

      protected void SetupUnloaded() {
         Next.IsLoadedResult = false;
         ActionLogBuilder.Clear();
      }

      protected void SetupUnloadedAndValidated() {
         Next.IsLoadedResult = false;
         GetConcreteBehavior().RevalidateDescendants(GetContext(), ValidationScope.SelfAndLoadedDescendants);
         ActionLogBuilder.Clear();
      }

      protected void SetupLoadedAndValidated(ValidationScope lastScope = ValidationScope.SelfAndAllDescendants) {
         Next.IsLoadedResult = true;
         GetConcreteBehavior().RevalidateDescendants(GetContext(), lastScope);
         ActionLogBuilder.Clear();
      }

      protected BehaviorContextStub CreateBehaviorContextFor(IVMPropertyDescriptor configuredProperty) {
         return ViewModelStub
            .WithProperties(configuredProperty)
            .BuildContext();
      }

      internal abstract DescendantsValidatorBehavior GetConcreteBehavior();

      internal abstract IBehaviorContext GetContext();

      internal class NextBehaviorsStub :
         Behavior,
         IValueInitializerBehavior,
         IRefreshBehavior,
         IDescendantValidationBehavior,
         IIsLoadedIndicatorBehavior,
         IDescendantsValidationResultProviderBehavior {

         public NextBehaviorsStub(StringBuilder log) {
            Log = log;
         }

         public bool IsLoadedResult { get; set; }
         private StringBuilder Log { get; set; }

         public void InitializeValue(IBehaviorContext context) {
            Log.Append(InitializeValueNext);
            IsLoadedResult = true;
         }

         public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
            Log.Append(RefreshNext);
         }

         public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {
            Log.Append(RevalidateNext);
         }

         public bool IsLoaded(IBehaviorContext context) {
            return IsLoadedResult;
         }

         public ValidationResult GetDescendantsValidationResult(IBehaviorContext context) {
            Log.Append(GetDescendantsValidationResultNext);
            return ValidationResult.Valid;
         }
      }
   }
}