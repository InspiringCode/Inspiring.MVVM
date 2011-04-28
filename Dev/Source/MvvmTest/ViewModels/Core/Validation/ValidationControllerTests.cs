namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Text;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationControllerTests : ValidationTestBase {
      private const string BeginValidationLog = "BeginValidation ";
      private const string EndValidationLog = "EndValidation ";
      private const string PropertyRevalidateLog = "Revalidate(Property) ";
      private const string ViewModelRevalidateLog = "Revalidate(ViewModel) ";

      private ValidationController Controller { get; set; }
      private StringBuilder ActionLog { get; set; }

      [TestInitialize]
      public void Setup() {
         ActionLog = new StringBuilder();
         Controller = new ValidationController();
      }

      [TestMethod]
      public void GetResult_ForProperty_ExecutesPropertyValidatorsAndCollectionValidators() {
         var setup = SetupItemInCollection();

         Controller.GetResult(ValidationStep.Value, setup.Item, setup.ItemProperty);

         var collectionPath = Path.Empty
            .Append(setup.Collection.OwnerVM)
            .Append(setup.Collection)
            .Append(setup.ItemProperty);

         var propertyRequest = new ValidationRequest(ValidationStep.Value, setup.Item, setup.ItemProperty);
         var collectionRequest = new ValidationRequest(ValidationStep.Value, collectionPath);

         DomainAssert.AreEqual(new[] { propertyRequest }, setup.ItemExecutor.Requests);
         DomainAssert.AreEqual(new[] { collectionRequest }, setup.CollectionOwnerExecutor.Requests);
      }

      [TestMethod]
      public void GetResult_ForProperty_ReturnsErrorsOfPropertyAndCollectionValidators() {
         var setup = SetupItemInCollection();

         var itemResult = CreateValidationResult(setup.Item, "Property error");
         var collectionResult = CreateValidationResult(setup.Item, "Collection error");

         setup.ItemExecutor.ResultToReturn = itemResult;
         setup.CollectionOwnerExecutor.ResultToReturn = collectionResult;

         var expected = ValidationResult.Join(itemResult, collectionResult);
         var actual = Controller.GetResult(ValidationStep.Value, setup.Item, setup.ItemProperty);

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void GetResult_ForViewModel_ExecutesViewModelValidatorsAndCollectionValidators() {
         var setup = SetupItemInCollection();

         Controller.GetResult(ValidationStep.Value, setup.Item);

         var collectionPath = Path.Empty
            .Append(setup.Collection.OwnerVM)
            .Append(setup.Collection);

         var propertyRequest = new ValidationRequest(ValidationStep.Value, setup.Item);
         var collectionRequest = new ValidationRequest(ValidationStep.Value, collectionPath);

         DomainAssert.AreEqual(new[] { propertyRequest }, setup.ItemExecutor.Requests);
         DomainAssert.AreEqual(new[] { collectionRequest }, setup.CollectionOwnerExecutor.Requests);
      }

      [TestMethod]
      public void GetResult_ForViewModel_ReturnsErrorsOfViewModelAndCollectionValidators() {
         var setup = SetupItemInCollection();

         var viewModelResult = CreateValidationResult(setup.Item, "View model error");
         var collectionResult = CreateValidationResult(setup.Item, "Collection error");

         setup.ItemExecutor.ResultToReturn = viewModelResult;
         setup.CollectionOwnerExecutor.ResultToReturn = collectionResult;

         var expected = ValidationResult.Join(viewModelResult, collectionResult);
         var actual = Controller.GetResult(ValidationStep.ViewModel, setup.Item);

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void ProcessPendingValidations_ForPropertyValidation_CallsBeginValidationAndRevalidateAndEndValidation() {
         var setup = SetupItemInCollection();

         Controller.RequestPropertyRevalidation(setup.Item, setup.ItemProperty);
         Controller.ProcessPendingValidations();

         Assert.AreEqual(
            BeginValidationLog + PropertyRevalidateLog + EndValidationLog,
            ActionLog.ToString()
         );
      }

      [TestMethod]
      public void ProcessPendingValidations_ForViewModelValidation_CallsRevalidate() {
         var setup = SetupItemInCollection();

         Controller.RequestViewModelRevalidation(setup.Item);
         Controller.ProcessPendingValidations();

         Assert.AreEqual(ViewModelRevalidateLog, ActionLog.ToString());
      }

      [TestMethod]
      public void ManuallyBeginValidation_CallsBeginValidationOnBehavior() {
         var setup = SetupItemInCollection();
         Controller.ManuallyBeginValidation(setup.Item, setup.ItemProperty);
         Assert.AreEqual(BeginValidationLog, ActionLog.ToString());
      }

      [TestMethod]
      public void ManuallyEndValidation_CallsEndValidationOnBehavior() {
         var setup = SetupItemInCollection();
         Controller.ManuallyEndValidation(setup.Item, setup.ItemProperty);
         Assert.AreEqual(EndValidationLog, ActionLog.ToString());
      }

      [TestMethod]
      public void RequestPropertyValidation_InvokesPropertyRevalidationBehaviors() {
         var setup = SetupItemInCollection();

         Controller.RequestPropertyRevalidation(setup.Item, setup.ItemProperty);
         Controller.ProcessPendingValidations();

         Assert.AreEqual(
            BeginValidationLog + PropertyRevalidateLog + EndValidationLog,
            ActionLog.ToString()
         );
      }

      [TestMethod]
      public void RequestViewModelValidation_InvokesViewModelRevalidationBehaviors() {
         var setup = SetupItemInCollection();

         Controller.RequestViewModelRevalidation(setup.Item);
         Controller.ProcessPendingValidations();

         Assert.AreEqual(ViewModelRevalidateLog, ActionLog.ToString());
      }

      [TestMethod]
      public void RequestPropertyRevalidation_WhenManuallyBeginValidationWasCalled_IgnoresRequest() {
         var setup = SetupItemInCollection();

         Controller.ManuallyBeginValidation(setup.Item, setup.ItemProperty);
         ActionLog.Clear();

         Controller.RequestPropertyRevalidation(setup.Item, setup.ItemProperty);
         Controller.ProcessPendingValidations();

         Assert.AreEqual("", ActionLog.ToString());
      }

      [TestMethod]
      public void RequestPropertyRevalidation_WhenManuallyBeginAndEndValidationWasCalled_ProcessesRequest() {
         var setup = SetupItemInCollection();

         Controller.ManuallyBeginValidation(setup.Item, setup.ItemProperty);
         Controller.ManuallyEndValidation(setup.Item, setup.ItemProperty);
         ActionLog.Clear();

         Controller.RequestPropertyRevalidation(setup.Item, setup.ItemProperty);
         Controller.ProcessPendingValidations();

         Assert.AreEqual(
            BeginValidationLog + PropertyRevalidateLog + EndValidationLog,
            ActionLog.ToString()
         );
      }

      [TestMethod]
      public void GetResults_CalledSecondTime_DoesNotPerformCollectionValidation() {
         var setup = SetupItemInCollection();

         Controller.GetResult(ValidationStep.Value, setup.Item);
         Controller.GetResult(ValidationStep.Value, setup.Item);

         Assert.AreEqual(1, setup.CollectionOwnerExecutor.Requests.Count);
      }

      [TestMethod]
      public void GetResults_AfterInvalidateCollectionResults_PerformsCollectionValidationAgain() {
         var setup = SetupItemInCollection();

         Controller.GetResult(ValidationStep.Value, setup.Item, setup.ItemProperty);
         Controller.InvalidCollectionResults(ValidationStep.Value, setup.Item, setup.ItemProperty);
         Controller.GetResult(ValidationStep.Value, setup.Item, setup.ItemProperty);

         Assert.AreEqual(2, setup.CollectionOwnerExecutor.Requests.Count);
      }

      private ItemInCollectionSetup SetupItemInCollection() {
         return new ItemInCollectionSetup(ActionLog);
      }

      private class ItemInCollectionSetup {
         public ItemInCollectionSetup(StringBuilder log) {
            ItemExecutor = new ValidationExecutorStub();
            CollectionOwnerExecutor = new ValidationExecutorStub();

            ItemProperty = PropertyStub
               .WithBehaviors(new RevalidationBehaviorSpy(log))
               .Build();

            Item = ViewModelStub
               .WithBehaviors(ItemExecutor, new RevalidationBehaviorSpy(log))
               .WithProperties(ItemProperty)
               .Build();

            var collectionOwner = ViewModelStub
               .WithBehaviors(CollectionOwnerExecutor)
               .Build();

            Collection = VMCollectionStub
               .WithItems(Item)
               .WithOwner(collectionOwner)
               .Build();
         }

         public ViewModelStub Item { get; private set; }
         public PropertyStub<object> ItemProperty { get; private set; }
         public VMCollectionStub<ViewModelStub> Collection { get; private set; }

         public ValidationExecutorStub ItemExecutor { get; private set; }
         public ValidationExecutorStub CollectionOwnerExecutor { get; private set; }
      }

      private class RevalidationBehaviorSpy :
         Behavior,
         IPropertyRevalidationBehavior,
         IViewModelRevalidationBehavior {

         public RevalidationBehaviorSpy(StringBuilder log) {
            Log = log;
         }

         private StringBuilder Log { get; set; }

         public void BeginValidation(IBehaviorContext context, ValidationController controller) {
            Log.Append(BeginValidationLog);
         }

         public void Revalidate(IBehaviorContext context) {
            Log.Append(PropertyRevalidateLog);
         }

         public void EndValidation(IBehaviorContext context) {
            Log.Append(EndValidationLog);
         }

         public void Revalidate(IBehaviorContext context, ValidationController controller) {
            Log.Append(ViewModelRevalidateLog);
         }
      }
   }
}