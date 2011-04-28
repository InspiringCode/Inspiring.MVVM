namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelValidationSourceBehaviorTests : ValidationTestBase {
      private const string Validate = "Validate ";
      private const string RefreshNext = "RefreshNext ";
      private const string RefreshNextProperty = "RefreshNextProperty ";
      private const string HandleChangeNext = "HandleChange ";
      private const string RevalidateNext = "RevalidateNext ";

      private ViewModelValidationSourceBehavior Behavior { get; set; }
      private NextBehavior Next { get; set; }
      private TestExecutor Executor { get; set; }
      private ViewModelStub VM { get; set; }
      private IBehaviorContext Context { get; set; }

      private StringBuilder ActionLogBuilder { get; set; }
      private string ActionLog {
         get { return ActionLogBuilder.ToString(); }
      }

      [TestInitialize]
      public void Setup() {
         ActionLogBuilder = new StringBuilder();
         Behavior = new ViewModelValidationSourceBehavior();
         Next = new NextBehavior(ActionLogBuilder);
         Executor = new TestExecutor(ActionLogBuilder);

         VM = ViewModelStub
            .WithBehaviors(Behavior, Next, Executor)
            .Build();

         Context = VM.GetContext();
      }

      [TestMethod]
      public void HandleChange_OwnPropertyChanged_PerformsViewModelValidations() {
         var args = ChangeArgs
            .PropertyChanged(PropertyStub.Of<string>())
            .PrependViewModel(VM);

         Behavior.HandleChange(Context, args);

         Assert.AreEqual(HandleChangeNext + Validate + RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void HandleChange_OwnValidationStateChanged_PerformsViewModelValidation() {
         Behavior.HandleChange(Context, ChangeArgs.ValidationStateChanged().PrependViewModel(VM));
         Assert.AreEqual(HandleChangeNext + Validate + RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void HandleChange_DescendantValidationStateChanged_PerformsViewModelValidation() {
         var args = ChangeArgs.ValidationStateChanged()
            .PrependViewModel(VM)
            .PrependViewModel(ViewModelStub.Build());

         Behavior.HandleChange(Context, args);
         Assert.AreEqual(HandleChangeNext + Validate + RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void HandleChange_DescendantPropertyChanged_PerformsViewModelValidation() {
         var args = ChangeArgs.PropertyChanged(PropertyStub.Of<string>())
            .PrependViewModel(VM)
            .PrependViewModel(new ViewModelStub());

         Behavior.HandleChange(Context, args);
         Assert.AreEqual(HandleChangeNext + Validate + RevalidateNext, ActionLog);
      }

      [TestMethod]
      public void Revalidate_InvokesExecutorWithCorrectArguments() {
         var cache = new CollectionResultCache();
         InvokeRevalidate();

         Assert.AreEqual(Validate + RevalidateNext, ActionLog);
         Assert.IsNotNull(Executor.LastContext);
         Assert.AreEqual(ValidationStep.ViewModel, Executor.LastRequest.Step);
         DomainAssert.AreEqual(Path.Empty.Append(VM), Executor.LastRequest.TargetPath);
      }

      [TestMethod]
      public void Revalidate_UpdatesValidationResult() {
         Executor.ResultToReturn = CreateValidationResult("Validation error");
         InvokeRevalidate();
         Assert.IsFalse(Behavior.GetValidationResult(Context).IsValid);

         Executor.ResultToReturn = ValidationResult.Valid;
         InvokeRevalidate();
         Assert.IsTrue(Behavior.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void Refresh_PerformsViewModelValidations() {
         Behavior.Refresh(Context);
         Assert.AreEqual(RefreshNext + Validate + RevalidateNext, ActionLog);
      }

      private void InvokeRevalidate() {
         var controller = new ValidationController();
         Behavior.Revalidate(Context, controller);
      }

      private class TestExecutor : Behavior, IValidationExecutorBehavior {
         public TestExecutor(StringBuilder log) {
            Log = log;
            ResultToReturn = ValidationResult.Valid;
         }

         public ValidationResult ResultToReturn { get; set; }
         public IBehaviorContext LastContext { get; set; }
         public ValidationRequest LastRequest { get; set; }

         private StringBuilder Log { get; set; }

         public ValidationResult Validate(IBehaviorContext context, ValidationRequest request) {
            Log.Append(ViewModelValidationSourceBehaviorTests.Validate);
            LastContext = context;
            LastRequest = request;
            return ResultToReturn;
         }
      }

      private class NextBehavior :
         Behavior,
         IViewModelRevalidationBehavior,
         IRefreshControllerBehavior,
         IChangeHandlerBehavior {

         public NextBehavior(StringBuilder log) {
            Log = log;
         }

         private StringBuilder Log { get; set; }

         public void Refresh(IBehaviorContext context) {
            Log.Append(RefreshNext);
         }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
            Log.Append(RefreshNextProperty);
         }

         public void Revalidate(IBehaviorContext context, ValidationController controller) {
            Log.Append(RevalidateNext);
         }

         public void HandleChange(IBehaviorContext context, ChangeArgs args) {
            Log.Append(HandleChangeNext);
         }
      }
   }
}