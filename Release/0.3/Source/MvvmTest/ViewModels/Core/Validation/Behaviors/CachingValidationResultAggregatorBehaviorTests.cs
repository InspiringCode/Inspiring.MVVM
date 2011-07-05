namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CachingValidationResultAggregatorBehaviorTests : ValidationTestBase {
      [TestMethod]
      public void GetValidationResult_Initially_ReturnsValidResults() {
         var vm = new TestVM();

         ParameterizedTest
            .TestCase(ValidationResultScope.PropertiesOnly)
            .TestCase(ValidationResultScope.ViewModelValidationsOnly)
            .TestCase(ValidationResultScope.Self)
            .TestCase(ValidationResultScope.Descendants)
            .TestCase(ValidationResultScope.All)
            .Run(scope => {
               var result = vm
                  .Behavior
                  .GetValidationResult(vm.GetContext(), scope);

               ValidationAssert.IsValid(result);
            });
      }

      [TestMethod]
      public void HandleChange_WithValidationResultChangedOfOwnProperty_UpdatesCachedResults() {
         var vm = new TestVM();

         var expected = new ExpectedResults {
            Properties = CreateValidationResult("First property error")
         };

         vm.FirstPropertyResultSetup = expected.Properties;
         vm.CallHandleChangeForFirstProperty();

         AssertBehaviorResults(vm, expected);
      }

      [TestMethod]
      public void HandleChange_WithValidationResultChangedOfOwnViewModel_UpdatesCachedResults() {
         var vm = new TestVM();

         var expected = new ExpectedResults {
            ViewModel = CreateValidationResult("View model error")
         };

         vm.ViewModelResultSetup = expected.ViewModel;
         vm.CallHandleChangeForViewModel();

         AssertBehaviorResults(vm, expected);
      }

      [TestMethod]
      public void HandleChange_WithValidationResultChangedOfDescendantProperty_DoesNotUpdateCachedResults() {
         var vm = new TestVM();

         var descendantArgs = ChangeArgs
            .ValidationResultChanged(PropertyStub.Build())
            .PrependViewModel(ViewModelStub.Build())
            .PrependViewModel(vm);

         vm.FirstPropertyResultSetup = CreateValidationResult("Irrelevant property error");
         vm.CallHandleChangeWith(descendantArgs);

         AssertBehaviorResults(vm, ExpectedResults.AllValid);
      }

      [TestMethod]
      public void HandleChange_WithItemsAddedOrRemovedFromOwnCollection_UpdatesCachedResults() {
         var owner = new TestVM();
         owner.MakePropertiesAndViewModelInvalid();
         var expected = owner.GetCurrentlySetupResults();

         //   F I R S T   A D D 
         var firstNewItem = new ViewModelStub {
            ValidationResult = CreateValidationResult("First new item error")
         };

         var collectionChangeArgs = ChangeArgs
            .ItemsAdded(VMCollectionStub.Build(), new[] { firstNewItem })
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(collectionChangeArgs);

         expected.Descenants = firstNewItem.ValidationResult;
         AssertBehaviorResults(owner, expected);

         //   S E C O N D   A D D 
         var secondNewItem = new ViewModelStub {
            ValidationResult = CreateValidationResult("Second new Item error")
         };

         collectionChangeArgs = ChangeArgs
           .ItemsAdded(VMCollectionStub.Build(), new[] { secondNewItem })
           .PrependViewModel(owner);

         owner.CallHandleChangeWith(collectionChangeArgs);

         expected.Descenants = ValidationResult.Join(
            firstNewItem.ValidationResult,
            secondNewItem.ValidationResult
         );

         AssertBehaviorResults(owner, expected);

         //   R E M O V AL
         collectionChangeArgs = ChangeArgs
            .ItemsRemoved(VMCollectionStub.Build(), new[] { firstNewItem })
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(collectionChangeArgs);

         expected.Descenants = secondNewItem.ValidationResult;
         AssertBehaviorResults(owner, expected);
      }

      [TestMethod]
      public void HandleChange_WithItemsAddedOrRemovedFromDescendantCollection_DoesNotUpdateCachedResults() {
         var owner = new TestVM();

         var newItem = new ViewModelStub {
            ValidationResult = CreateValidationResult("Irrelevant item error")
         };

         var collectionChangeArgs = ChangeArgs
            .ItemsAdded(VMCollectionStub.Build(), new[] { newItem })
            .PrependViewModel(ViewModelStub.Build())
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(collectionChangeArgs);

         AssertBehaviorResults(owner, ExpectedResults.AllValid);
      }

      [TestMethod]
      public void HandleChange_WithPropertyChangedOfOwnViewModelProperty_UpdatesCachedResults() {
         var owner = new TestVM();
         owner.MakePropertiesAndViewModelInvalid();
         var expected = owner.GetCurrentlySetupResults();

         //   S E T   C H I L D
         var firstChild = new ViewModelStub {
            ValidationResult = CreateValidationResult("First child error")
         };

         var propertyChangedArgs = ChangeArgs
            .ViewModelPropertyChanged(
               owner.SecondProperty,
               oldValue: null,
               newValue: firstChild)
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(propertyChangedArgs);

         expected.Descenants = firstChild.ValidationResult;
         AssertBehaviorResults(owner, expected);

         //   C H A N G E   C H I L D
         var secondChild = new ViewModelStub {
            ValidationResult = CreateValidationResult("Second child error")
         };

         propertyChangedArgs = ChangeArgs
            .ViewModelPropertyChanged(
               owner.SecondProperty,
               oldValue: firstChild,
               newValue: secondChild)
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(propertyChangedArgs);

         expected.Descenants = secondChild.ValidationResult;
         AssertBehaviorResults(owner, expected);

         //   S E T   C H I L D   T O   N U L L
         propertyChangedArgs = ChangeArgs
            .ViewModelPropertyChanged(
               owner.SecondProperty,
               oldValue: secondChild,
               newValue: null)
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(propertyChangedArgs);

         expected.Descenants = ValidationResult.Valid;
         AssertBehaviorResults(owner, expected);
      }

      [TestMethod]
      public void HandleChange_WithValidationResultChangedOfOwnProperty_UpdatesBehaviorOfParentViewModel() {
         var child = new TestVM();
         var parent = new TestVM();

         parent.MakePropertiesAndViewModelInvalid();
         child.FirstPropertyResultSetup = CreateValidationResult("Property error");

         child.Kernel.Parents.Add(parent);
         child.CallHandleChangeForFirstProperty();

         var expectedParentResult = parent.GetCurrentlySetupResults();
         expectedParentResult.Descenants = child.ValidationResult;

         AssertBehaviorResults(parent, expectedParentResult);
      }

      [TestMethod]
      public void HandleChange_WithValidationResultChangedOfOwnViewModel_UpdatesBehaviorOfParentViewModel() {
         var child = new TestVM();
         var parent = new TestVM();

         parent.MakePropertiesAndViewModelInvalid();
         child.ViewModelResultSetup = CreateValidationResult("View model error");

         child.Kernel.Parents.Add(parent);
         child.CallHandleChangeForViewModel();

         var expectedParentResult = parent.GetCurrentlySetupResults();
         expectedParentResult.Descenants = child.ValidationResult;

         AssertBehaviorResults(parent, expectedParentResult);
      }

      [TestMethod]
      public void HandleChange_WithItemsAddedToOwnCollection_UpdatesBehaviorOfParentViewModel() {
         var parent = new TestVM();
         var owner = new TestVM();

         parent.MakePropertiesAndViewModelInvalid();
         owner.Kernel.Parents.Add(parent);

         var newItem = new ViewModelStub {
            ValidationResult = CreateValidationResult("New item error")
         };

         var collectionChangeArgs = ChangeArgs
            .ItemsAdded(VMCollectionStub.Build(), new[] { newItem })
            .PrependViewModel(owner);

         owner.CallHandleChangeWith(collectionChangeArgs);

         var expectedParentResult = parent.GetCurrentlySetupResults();
         expectedParentResult.Descenants = newItem.ValidationResult;
         AssertBehaviorResults(parent, expectedParentResult);
      }

      [TestMethod]
      public void HandleChange_UpdatesBehaviorOfParentViewModelBeforeCallingHandleChangeNext() {
         var child = new TestVM();
         var parent = new TestVM();

         child.FirstPropertyResultSetup = CreateValidationResult("Property error");
         child.Kernel.Parents.Add(parent);

         child.NextChangeHandler.HandleChangeNext += delegate {
            // Assert parent was already updated
            var expected = new ExpectedResults { Descenants = child.FirstPropertyResultSetup };
            AssertBehaviorResults(parent, expected);
         };

         child.CallHandleChangeForFirstProperty();
      }


      //
      //   T E S T   I N F R A S T R U C T U R E
      //

      private class TestVM : ViewModelStub {
         private ValidationResultProviderStub _firstPropertyResultProvider = new ValidationResultProviderStub();
         private ValidationResultProviderStub _secondPropertyResultProvider = new ValidationResultProviderStub();
         private ValidationResultProviderStub _viewModelResultProvider = new ValidationResultProviderStub();

         public TestVM() {
            NextChangeHandler = new NextChangeHandlerBehavior();
            Behavior = new CachingValidationResultAggregatorBehavior();

            FirstProperty = PropertyStub
              .WithBehaviors(_firstPropertyResultProvider)
              .Of<object>();

            SecondProperty = PropertyStub
              .WithBehaviors(_secondPropertyResultProvider)
              .Of<ViewModelStub>();

            Descriptor = DescriptorStub
               .WithProperties(FirstProperty, SecondProperty)
               .WithBehaviors(Behavior, _viewModelResultProvider, NextChangeHandler)
               .Build();
         }

         public CachingValidationResultAggregatorBehavior Behavior { get; set; }

         public NextChangeHandlerBehavior NextChangeHandler { get; set; }

         public IVMPropertyDescriptor<object> FirstProperty { get; set; }
         public IVMPropertyDescriptor<ViewModelStub> SecondProperty { get; set; }

         public ValidationResult FirstPropertyResultSetup {
            get { return _firstPropertyResultProvider.ReturnedResult; }
            set { _firstPropertyResultProvider.ReturnedResult = value; }
         }

         public ValidationResult SecondPropertyResultSetup {
            get { return _secondPropertyResultProvider.ReturnedResult; }
            set { _secondPropertyResultProvider.ReturnedResult = value; }
         }

         public ValidationResult ViewModelResultSetup {
            get { return _viewModelResultProvider.ReturnedResult; }
            set { _viewModelResultProvider.ReturnedResult = value; }
         }

         public void CallHandleChangeForPropertiesAndViewModel() {
            CallHandleChangeForFirstProperty();
            CallHandleChangeForSecondProperty();
            CallHandleChangeForViewModel();
         }


         public void CallHandleChangeForFirstProperty() {
            CallHandleChangeWith(ChangeArgs
               .ValidationResultChanged(FirstProperty)
               .PrependViewModel(this));
         }


         public void CallHandleChangeForSecondProperty() {
            CallHandleChangeWith(ChangeArgs
               .ValidationResultChanged(SecondProperty)
               .PrependViewModel(this));
         }


         public void CallHandleChangeForViewModel() {
            CallHandleChangeWith(ChangeArgs
               .ValidationResultChanged()
               .PrependViewModel(this));
         }

         public void CallHandleChangeWith(ChangeArgs args) {
            Behavior.HandleChange(
               GetContext(),
               args
            );
         }

         public void MakePropertiesAndViewModelInvalid() {
            FirstPropertyResultSetup = CreateValidationResult("First property error");
            SecondPropertyResultSetup = CreateValidationResult("Second property error");
            ViewModelResultSetup = CreateValidationResult("View model error");
            CallHandleChangeForPropertiesAndViewModel();
         }

         public ExpectedResults GetCurrentlySetupResults() {
            return new ExpectedResults {
               Properties = ValidationResult.Join(FirstPropertyResultSetup, SecondPropertyResultSetup),
               ViewModel = ViewModelResultSetup
            };
         }

         public ValidationResult GetResultFromBehavior(ValidationResultScope scope) {
            return Behavior.GetValidationResult(
               GetContext(),
               scope
            );
         }
      }

      private class ExpectedResults {
         public static readonly ExpectedResults AllValid = new ExpectedResults();

         public ExpectedResults() {
            Properties = ValidationResult.Valid;
            ViewModel = ValidationResult.Valid;
            Descenants = ValidationResult.Valid;
         }

         public ValidationResult Properties { get; set; }
         public ValidationResult ViewModel { get; set; }
         public ValidationResult Descenants { get; set; }

         public ValidationResult Self {
            get { return ValidationResult.Join(Properties, ViewModel); }
         }

         public ValidationResult All {
            get { return ValidationResult.Join(Self, Descenants); }
         }
      }

      private static void AssertBehaviorResults(TestVM vm, ExpectedResults results) {
         ValidationAssert.AreEqual(
            results.Properties,
            vm.GetResultFromBehavior(ValidationResultScope.PropertiesOnly)
         );

         ValidationAssert.AreEqual(
            results.ViewModel,
            vm.GetResultFromBehavior(ValidationResultScope.ViewModelValidationsOnly)
         );

         ValidationAssert.AreEqual(
            results.Self,
            vm.GetResultFromBehavior(ValidationResultScope.Self)
         );

         ValidationAssert.AreEqual(
            results.Descenants,
            vm.GetResultFromBehavior(ValidationResultScope.Descendants)
         );

         ValidationAssert.AreEqual(
            results.All,
            vm.GetResultFromBehavior(ValidationResultScope.All)
         );
      }

      private class NextChangeHandlerBehavior : Behavior, IChangeHandlerBehavior {
         public event Action HandleChangeNext;

         public void HandleChange(IBehaviorContext context, ChangeArgs args) {
            if (HandleChangeNext != null) {
               HandleChangeNext();
            }
         }
      }
   }
}