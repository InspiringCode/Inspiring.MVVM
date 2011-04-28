namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RevalidatorTests : ValidationTestBase {
      private const string RevalidateProperty = "Revalidate(Property) ";
      private const string RevalidateViewModel = "Revalidate(ViewModel) ";
      private const string RevalidateDescendants = "RevalidateDescendants(Property) ";

      private TestViewModel VM { get; set; }
      private StringBuilder ActionLogBuilder { get; set; }

      private string ActionLog {
         get { return ActionLogBuilder.ToString(); }
      }

      [TestInitialize]
      public void Setup() {
         ActionLogBuilder = new StringBuilder();
         VM = CreateVM();
      }

      [TestMethod]
      public void RevalidatePropertyValidationsSelfOnly_CallsRevalidationBehaviorOfProperty() {
         Revalidator.RevalidatePropertyValidations(VM, VM.Property, ValidationScope.SelfOnly);

         Assert.AreEqual(RevalidateProperty, ActionLog);
         Assert.AreEqual(VM.GetContext(), VM.PropertyBehavior.LastRevalidateContext);
         Assert.IsNotNull(VM.PropertyBehavior.LastRevalidateContext);
      }

      [TestMethod]
      public void RevalidatePropertyValidationsDescendants_CallsRevalidationAndDescendantsValidationBehavior() {
         var scope = ValidationScope.FullSubtree;

         Revalidator.RevalidatePropertyValidations(VM, VM.Property, scope);

         Assert.AreEqual(RevalidateDescendants + RevalidateProperty, ActionLog);
         Assert.AreEqual(VM.GetContext(), VM.PropertyBehavior.LastDescendantsContext);
         Assert.AreEqual(scope, VM.PropertyBehavior.LastDescendantsScope);
      }

      [TestMethod]
      public void PerformViewModelValidations_CallsRevalidationViewModelBehavior() {
         Revalidator.RevalidateViewModelValidations(VM);

         Assert.AreEqual(RevalidateViewModel, ActionLog);
         Assert.AreEqual(VM.GetContext(), VM.ViewModelBehavior.LastRevalidateContext);
         Assert.IsNotNull(VM.ViewModelBehavior.LastValidationController);
      }

      [TestMethod]
      public void Revalidate_PerformsPropertyAndViewModelValidation() {
         Revalidator.Revalidate(VM, ValidationScope.FullSubtree);
         Assert.AreEqual(RevalidateDescendants + RevalidateProperty + RevalidateViewModel, ActionLog);
      }

      [TestMethod]
      public void RevalidateItems_PerformsPropertyAndViewModelValidationsForAllItems() {
         var items = new[] { CreateVM(), CreateVM() };

         Revalidator.RevalidateItems(items, ValidationScope.FullSubtree);

         var expectedSequence =
            RevalidateDescendants + RevalidateDescendants +
            RevalidateProperty + RevalidateProperty +
            RevalidateViewModel + RevalidateViewModel;

         Assert.AreEqual(expectedSequence, ActionLog);
      }

      [TestMethod]
      public void RevalidateItems_PassesSameValidationControllerToAllItems() {
         var first = CreateVM();
         var second = CreateVM();
         var items = new[] { first, second };

         Revalidator.RevalidateItems(items, ValidationScope.FullSubtree);

         Assert.AreEqual(
            first.PropertyBehavior.LastValidationController,
            second.PropertyBehavior.LastValidationController
         );

         Assert.AreEqual(
            first.ViewModelBehavior.LastValidationController,
            second.ViewModelBehavior.LastValidationController
         );
      }

      private TestViewModel CreateVM() {
         return new TestViewModel(ActionLogBuilder);
      }

      private class TestViewModel : ViewModelStub {
         public TestViewModel(StringBuilder log)
            : this(
               viewModelBehavior: new TestRevalidationBehavior(log, RevalidateViewModel),
               propertyBehavior: new TestRevalidationBehavior(log, RevalidateProperty)
            ) {
         }

         private TestViewModel(
            TestRevalidationBehavior viewModelBehavior,
            TestRevalidationBehavior propertyBehavior
         )
            : this(
                PropertyStub.WithBehaviors(propertyBehavior).Of<string>(),
                viewModelBehavior
            ) {

            ViewModelBehavior = viewModelBehavior;
            PropertyBehavior = propertyBehavior;
         }

         private TestViewModel(
            PropertyStub<string> property,
            TestRevalidationBehavior viewModelBehavior
         )
            : base(DescriptorStub
               .WithBehaviors(viewModelBehavior)
               .WithProperties(property)
               .Build()
            ) {

            Property = property;
         }

         public PropertyStub<string> Property { get; private set; }
         public TestRevalidationBehavior ViewModelBehavior { get; private set; }
         public TestRevalidationBehavior PropertyBehavior { get; private set; }
      }

      private class TestRevalidationBehavior :
         Behavior,
         IPropertyRevalidationBehavior,
         IDescendantValidationBehavior,
         IViewModelRevalidationBehavior {

         public TestRevalidationBehavior(StringBuilder log, string revalidateLogText) {
            Log = log;
            RevalidateLogText = revalidateLogText;
         }

         public IBehaviorContext LastRevalidateContext { get; set; }
         public ValidationController LastValidationController { get; set; }

         public IBehaviorContext LastDescendantsContext { get; set; }
         public ValidationScope LastDescendantsScope { get; set; }

         private StringBuilder Log { get; set; }
         private string RevalidateLogText { get; set; }

         public void Revalidate(IBehaviorContext context, ValidationController controller) {
            LastRevalidateContext = context;
            LastValidationController = controller;
            Log.Append(RevalidatorTests.RevalidateViewModel);
         }

         public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
            LastRevalidateContext = context;
            Log.Append(RevalidateLogText);
         }

         public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {
            LastDescendantsContext = context;
            LastDescendantsScope = scope;
            Log.Append(RevalidatorTests.RevalidateDescendants);
         }

         public void BeginValidation(IBehaviorContext context, ValidationController controller) {
            LastValidationController = controller;
         }

         public void EndValidation(IBehaviorContext context) {
         }
      }

   }
}