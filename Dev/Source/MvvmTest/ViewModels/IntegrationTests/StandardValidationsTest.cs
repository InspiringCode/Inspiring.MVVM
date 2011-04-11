namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class StandardValidationsTest : ValidationTestBase {

      [TestMethod]
      public void HasValue_PropertyHasNoValue_InvalidatesViewModel() {
         ChildVM vm = new ChildVM();
         vm.StringProperty = String.Empty;

         var expectedResult = CreateValidationResult(ChildVM.HasValueValidationErrorMessage);
         var actualResult = vm.GetValidationState(ChildVM.ClassDescriptor.StringProperty);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void CheckLength_PropertyLengthExceedsMaximumLength_InvalidatesViewModel() {
         ChildVM vm = new ChildVM();
         vm.StringProperty = "Wert";

         Assert.IsTrue(vm.IsValid);

         vm.StringProperty = "Wert!";

         var expectedErrorMessage = String.Format(
            ChildVM.LengthValidationErrorMessage,
            ChildVM.MaxStringPropertyLength
         );

         var expectedResult = CreateValidationResult(expectedErrorMessage);
         var actualResult = vm.GetValidationState(ChildVM.ClassDescriptor.StringProperty);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void IsUnique_WithDuplicateStringProperties_InvalidatesViewModel() {
         ParentVM vm = new ParentVM();

         ChildVM child1 = new ChildVM() { StringProperty = "Val1" };
         ChildVM child2 = new ChildVM() { StringProperty = "Val2" };
         ChildVM child3 = new ChildVM() { StringProperty = "Val3" };

         vm.Children.Add(child1);
         vm.Children.Add(child2);
         vm.Children.Add(child3);

         Assert.IsTrue(vm.IsValid);

         child1.StringProperty = "Val1";
         child2.StringProperty = "Val2";
         child3.StringProperty = "Val2";

         var child2Result = CreateValidationResult(child2, ParentVM.DuplicateStringPropertyValidationErrorMessage);
         var child3Result = CreateValidationResult(child2, ParentVM.DuplicateStringPropertyValidationErrorMessage);

         var expectedResult = ValidationResult.Join(child2Result, child3Result);

         var actualResult = vm.GetValidationState(ParentVM.ClassDescriptor.Children);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void IsUnique_WithDuplicateIntegerProperties_InvalidatesViewModel() {
         ParentVM vm = new ParentVM();

         ChildVM child1 = new ChildVM() { StringProperty = "Val1", IntegerProperty = 1 };
         ChildVM child2 = new ChildVM() { StringProperty = "Val2", IntegerProperty = 2 };
         ChildVM child3 = new ChildVM() { StringProperty = "Val3", IntegerProperty = 3 };

         vm.Children.Add(child1);
         vm.Children.Add(child2);
         vm.Children.Add(child3);

         Assert.IsTrue(vm.IsValid);

         child2.IntegerProperty = 1;

         var child1Result = CreateValidationResult(child1, ParentVM.DuplicateIntegerPropertyValidationErrorMessage);
         var child2Result = CreateValidationResult(child2, ParentVM.DuplicateIntegerPropertyValidationErrorMessage);

         var expectedResult = ValidationResult.Join(child1Result, child2Result);

         var actualResult = vm.GetValidationState(ParentVM.ClassDescriptor.Children);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void PropagateChildErrors_WithInvalidChildren_ParentHasError() {
         ParentVM vm = new ParentVM();

         ChildVM child1 = new ChildVM() { StringProperty = "Val1" };
         ChildVM child2 = new ChildVM() { StringProperty = "Val2" };

         vm.Children.Add(child1);
         vm.Children.Add(child2);

         Assert.IsTrue(vm.IsValid);

         child2.StringProperty = String.Empty;

         var expectedResult = CreateValidationResult(ParentVM.ChildValidationErrorMessage);
         var actualResult = vm.GetValidationState(ValidationResultScope.ViewModelValidationsOnly);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void RegexValidation_RegexPatternMatches_InvalidatesViewModel() {
         ChildVM vm = new ChildVM();

         vm.StringProperty = "RegexPattern";

         var expectedResult = CreateValidationResult(ChildVM.RegexValidationErrorMessage);
         var actualResult = vm.GetValidationState(ChildVM.ClassDescriptor.StringProperty);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void ValidateProperties_InvalidProperty_InvalidatesViewModel() {
         ChildVM vm = new ChildVM();

         vm.StringProperty = String.Empty;

         var expectedResult = CreateValidationResult(ChildVM.ValidatePropertiesErrorMessage);
         var actualResult = vm.GetValidationState(ValidationResultScope.ViewModelValidationsOnly);
         DomainAssert.AreEqual(expectedResult, actualResult);
      }

      private class ParentVM : ViewModel<ParentVMDescriptor> {
         public const string ChildValidationErrorMessage = "Child invalid";
         public const string DuplicateStringPropertyValidationErrorMessage = "Duplicate string";
         public const string DuplicateIntegerPropertyValidationErrorMessage = "Duplicate string";

         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               d.Children = v.Collection.Of<ChildVM>(ChildVM.ClassDescriptor);
            })
            .WithValidators(c => {
               c.CheckCollection(x => x.Children, x => x.StringProperty).IsUnique(
                  StringComparison.CurrentCultureIgnoreCase,
                  DuplicateStringPropertyValidationErrorMessage
               );
               c.CheckCollection(x => x.Children, x => x.IntegerProperty).IsUnique(
                  DuplicateIntegerPropertyValidationErrorMessage
               );
               c.PropagateChildErrors(ChildValidationErrorMessage);
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
         }

         public IVMCollection<ChildVM> Children {
            get { return GetValue(Descriptor.Children); }
            set { SetValue(Descriptor.Children, value); }
         }

         public bool IsValid {
            get { return Kernel.GetValidationState().IsValid; }
         }
      }

      private class ChildVM : ViewModel<ChildVMDescriptor> {
         public const string HasValueValidationErrorMessage = "No value";
         public const int MaxStringPropertyLength = 4;
         public const string LengthValidationErrorMessage = "Max length {0}";
         public const string RegexValidationErrorMessage = "Regex error";
         public const string ValidatePropertiesErrorMessage = "Property validation error message";

         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               d.StringProperty = v.Property.Of<string>();
               d.IntegerProperty = v.Property.Of<int>();
            })
            .WithValidators(c => {
               c.ValidateProperties(ValidatePropertiesErrorMessage);
               c.Check(x => x.StringProperty).HasValue(HasValueValidationErrorMessage);
               c.Check(x => x.StringProperty).Length(MaxStringPropertyLength, LengthValidationErrorMessage);
               c.Check(x => x.StringProperty).RegexValidation(@"RegexPattern", RegexValidationErrorMessage);
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public string StringProperty {
            get { return GetValue(Descriptor.StringProperty); }
            set { SetValue(Descriptor.StringProperty, value); }
         }

         public int IntegerProperty {
            get { return GetValue(Descriptor.IntegerProperty); }
            set { SetValue(Descriptor.IntegerProperty, value); }
         }

         public bool IsValid {
            get { return Kernel.GetValidationState().IsValid; }
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> Children { get; set; }
      }

      private class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> StringProperty { get; set; }
         public IVMPropertyDescriptor<int> IntegerProperty { get; set; }
      }
   }
}