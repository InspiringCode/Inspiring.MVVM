namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RevalidatorTests : ValidationTestBase {
      private const string RevalidateProperty = "Revalidate(Property) ";
      private const string RevalidateViewModel = "Revalidate(Property) ";
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
         Assert.IsNotNull(VM.PropertyBehavior.LastRevalidateCache);
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
         Assert.IsNotNull(VM.ViewModelBehavior.LastRevalidateCache);
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

         var perItemSequence = RevalidateDescendants + RevalidateProperty + RevalidateViewModel;
         Assert.AreEqual(perItemSequence + perItemSequence, ActionLog);
      }

      [TestMethod]
      public void RevalidateItems_PassesSameCacheInstanceToAllItems() {
         var first = CreateVM();
         var second = CreateVM();
         var items = new[] { first, second };

         Revalidator.RevalidateItems(items, ValidationScope.FullSubtree);

         Assert.AreEqual(
            first.PropertyBehavior.LastRevalidateCache,
            second.PropertyBehavior.LastRevalidateCache
         );

         Assert.AreEqual(
            first.ViewModelBehavior.LastRevalidateCache,
            second.ViewModelBehavior.LastRevalidateCache
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

      private class TestRevalidationBehavior : Behavior, IRevalidationBehavior, IDescendantValidationBehavior {
         public TestRevalidationBehavior(StringBuilder log, string revalidateLogText) {
            Log = log;
            RevalidateLogText = revalidateLogText;
         }

         public IBehaviorContext LastRevalidateContext { get; set; }
         public CollectionResultCache LastRevalidateCache { get; set; }

         public IBehaviorContext LastDescendantsContext { get; set; }
         public ValidationScope LastDescendantsScope { get; set; }

         private StringBuilder Log { get; set; }
         private string RevalidateLogText { get; set; }

         public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
            LastRevalidateContext = context;
            LastRevalidateCache = cache;
            Log.Append(RevalidateLogText);
         }

         public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {
            LastDescendantsContext = context;
            LastDescendantsScope = scope;
            Log.Append(RevalidatorTests.RevalidateDescendants);
         }
      }

   }
}