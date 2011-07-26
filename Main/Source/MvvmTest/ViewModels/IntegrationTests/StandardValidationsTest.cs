namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System;
   using System.Collections.Generic;
   using System.Globalization;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class StandardValidationsTest : ValidationTestBase {
      [TestMethod]
      public void HasValue_PropertyHasNoValue_AddsValidationErrorToProperty() {
         var error = "Value required";
         var vm = CreateChild(b => b.Check(x => x.StringProperty).HasValue(error));

         vm.StringProperty = "Valid value";
         ValidationAssert.IsValid(vm);

         vm.StringProperty = String.Empty;
         ValidationAssert.ErrorMessages(vm.ValidationResult, error);
      }

      [TestMethod]
      public void CheckLength_PropertyLengthExceedsMaximumLength_AddsValidationErrorToProperty() {
         var error = "Value too long";
         var vm = CreateChild(b => b.Check(x => x.StringProperty).Length(4, error));

         vm.StringProperty = "1234";
         ValidationAssert.IsValid(vm);

         vm.StringProperty = "12345";
         ValidationAssert.ErrorMessages(vm.ValidationResult, error);
      }

      [TestMethod]
      public void IsUnique_WithDuplicateStringProperties_InvalidatesViewModel() {
         var error = "Duplicate item";
         var vm = CreateParent(b => b
            .CheckCollection(x => x.Children, x => x.StringProperty)
            .IsUnique(StringComparison.CurrentCultureIgnoreCase, error)
         );

         var item1 = new ChildVM("Item 1") { StringProperty = "VAL1" };
         var item2 = new ChildVM("Item 2") { StringProperty = "VAL2" };
         var item3 = new ChildVM("Item 3") { StringProperty = "VAL3" };

         vm.Children.Add(item1);
         vm.Children.Add(item2);
         vm.Children.Add(item3);

         ValidationAssert.IsValid(vm);
         item3.StringProperty = "VAL2";

         var expectedResult = CreateValidationResult(
            Error(error).For(item3, x => x.StringProperty),
            Error(error).For(item2, x => x.StringProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void IsUnique_WithDuplicateIntegerProperties_InvalidatesViewModel() {
         var error = "Duplicate item";
         var vm = CreateParent(b => b
            .CheckCollection(x => x.Children, x => x.IntegerProperty)
            .IsUnique(error)
         );

         var item1 = new ChildVM() { IntegerProperty = 1 };
         var item2 = new ChildVM() { IntegerProperty = 2 };
         var item3 = new ChildVM() { IntegerProperty = 3 };

         vm.Children.Add(item1);
         vm.Children.Add(item2);
         vm.Children.Add(item3);

         Assert.IsTrue(vm.IsValid);

         item2.IntegerProperty = 1;

         var expectedResult = CreateValidationResult(
            Error(error).For(item2, x => x.IntegerProperty),
            Error(error).For(item1, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void PropagateChildErrors_WithInvalidChildren_ParentHasError() {
         var error = "Child records are invalid";

         var validItem = new ChildVM();
         var invalidItem = new ChildVM();

         var childDescriptor = CreateChildDescriptor(b => b.CheckViewModel(args => {
            if (args.Target == invalidItem) {
               args.AddError("Item error");
            }
         }));

         ParentVM vm = CreateParent(b => b.PropagateChildErrors(error), childDescriptor);

         vm.Children.Add(validItem);
         ValidationAssert.IsValid(vm.ValidationResult);

         vm.Children.Add(invalidItem);
         var actualParentValidationState = vm.GetValidationResult(ValidationResultScope.Self);
         ValidationAssert.ErrorMessages(actualParentValidationState, error);
      }

      [TestMethod]
      public void RegexValidation_RegexPatternMatches_InvalidatesViewModel() {
         var error = "Invalid number";
         var vm = CreateChild(b => b
            .Check(x => x.StringProperty)
            .RegexValidation(@"\d+", error)
         );

         vm.StringProperty = "1234";
         ValidationAssert.IsValid(vm);

         vm.StringProperty = "AnInvalidNumber";
         ValidationAssert.ErrorMessages(vm.ValidationResult, error);
      }

      [TestMethod]
      public void ValidateProperties_InvalidProperty_InvalidatesViewModel() {
         var propertyValid = true;

         var error = "Some fields are invalid";
         var vm = CreateChild(b => {
            b.ValidateProperties(error);
            b.Check(x => x.StringProperty).Custom(x => {
               if (!propertyValid) {
                  x.AddError("Property error");
               }
            });
         });

         vm.Revalidate();
         ValidationAssert.IsValid(vm);

         propertyValid = false;
         vm.Revalidate();

         var viewModelOnlyResult = vm.GetValidationResult(ValidationResultScope.ViewModelValidationsOnly);
         ValidationAssert.ErrorMessages(viewModelOnlyResult, error);
      }

      [TestMethod]
      public void IsUnique_WithCustomDuplicateKey_InvalidatesViewModel() {
         var error = "Duplicate item";
         var vm = CreateParent(b => b
            .CheckCollection(x => x.Children)
            .IsUnique(x => Tuple.Create(x.IntegerProperty, x.StringProperty), error)
         );

         var item1 = new ChildVM() { IntegerProperty = 1, StringProperty = "Item1" };
         var item2 = new ChildVM() { IntegerProperty = 2, StringProperty = "Item2" };
         var item3 = new ChildVM() { IntegerProperty = 3, StringProperty = "Item3" };

         vm.Children.Add(item1);
         vm.Children.Add(item2);
         vm.Children.Add(item3);

         Assert.IsTrue(vm.IsValid);

         item2.IntegerProperty = 1;
         item2.StringProperty = "Item1";

         var expectedResult = CreateValidationResult(
           Error(error).For(item2),
           Error(error).For(item1)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void IsUnique_WithCustomDuplicateEqualityComparer_InvalidatesViewModel() {
         var error = "Duplicate item";
         var vm = CreateParent(b => b
            .CheckCollection(x => x.Children)
            .IsUnique(new CustomChildVMComparer(), error)
         );

         var item1 = new ChildVM() { IntegerProperty = 1, StringProperty = "Item1" };
         var item2 = new ChildVM() { IntegerProperty = 2, StringProperty = "Item2" };
         var item3 = new ChildVM() { IntegerProperty = 3, StringProperty = "Item3" };

         vm.Children.Add(item1);
         vm.Children.Add(item2);
         vm.Children.Add(item3);

         Assert.IsTrue(vm.IsValid);

         item2.IntegerProperty = 1;
         item2.StringProperty = "Item1";

         var expectedResult = CreateValidationResult(
           Error(error).For(item2),
           Error(error).For(item1)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void IsUnique_WithDuplicateItemViewModels_InvalidatesDuplicateItems() {
         var error = "Duplicate item";
         var vm = CreateParent(b => b
            .CheckCollection(x => x.Children)
            .IsUnique(error)
         );

         var item1 = new ChildVM() { IntegerProperty = 1, StringProperty = "Item1" };

         vm.Children.Add(item1);

         Assert.IsTrue(vm.IsValid);

         vm.Children.Add(item1);

         var expectedResult = CreateValidationResult(
           Error(error).For(item1),
           Error(error).For(item1)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void ValueInRange_ForIntergerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value not in range";
         int min = 1;
         int max = 10;
         int valueOutOfRange = max + 1;

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .ValueInRange(min, max, errorMessage)
         );

         vm.SetValue(x => x.IntegerProperty, 1);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.IntegerProperty, valueOutOfRange);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void ValueInRange_ForNullableIntergerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value not in range";
         int min = 1;
         int max = 10;
         int valueOutOfRange = max + 1;

         var vm = CreateParent(b => b
            .Check(x => x.NullableIntegerProperty)
               .ValueInRange(min, max, errorMessage)
         );

         vm.SetValue(x => x.NullableIntegerProperty, null);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, valueOutOfRange);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.NullableIntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Min_ForIntegerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value below min";
         int min = 10;
         int belowMin = min - 1;

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .Min(min, errorMessage)
         );

         vm.SetValue(x => x.IntegerProperty, min);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.IntegerProperty, belowMin);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Min_ForNullableIntegerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value below min";
         int min = 10;
         int belowMin = min - 1;

         var vm = CreateParent(b => b
            .Check(x => x.NullableIntegerProperty)
               .Min(min, errorMessage)
         );

         vm.SetValue(x => x.NullableIntegerProperty, null);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, min);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, belowMin);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.NullableIntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Min_ForIntegerPropertyWithMinValueSelector_ValidatesPropertyAccordingly() {
         var errorMessage = "Value below min";

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .Min(x => x.Limit, errorMessage)
         );
         vm.Limit = 3;
         vm.SetValue(x => x.IntegerProperty, vm.Limit);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.IntegerProperty, vm.Limit - 1);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Min_ForNullableIntegerPropertyWithMinValueSelector_ValidatesPropertyAccordingly() {
         var errorMessage = "Value below min";

         var vm = CreateParent(b => b
            .Check(x => x.NullableIntegerProperty)
               .Min(x => x.Limit, errorMessage)
         );
         vm.Limit = 3;

         vm.SetValue(x => x.NullableIntegerProperty, null);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, vm.Limit);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, vm.Limit - 1);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.NullableIntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Max_ForIntegerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value above max";
         int max = 10;
         int aboveMax = max + 1;

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .Max(max, errorMessage)
         );

         vm.SetValue(x => x.IntegerProperty, max);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.IntegerProperty, aboveMax);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Max_ForNullableIntegerProperty_ValidatesPropertyAccordingly() {
         var errorMessage = "Value above max";
         int max = 10;
         int aboveMax = max + 1;

         var vm = CreateParent(b => b
            .Check(x => x.NullableIntegerProperty)
               .Max(max, errorMessage)
         );

         vm.SetValue(x => x.NullableIntegerProperty, null);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, max);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, aboveMax);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.NullableIntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Max_ForIntegerPropertyWithMaxValueSelector_ValidatesPropertyAccordingly() {
         var errorMessage = "Value above max";

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .Max(x => x.Limit, errorMessage)
         );
         vm.Limit = 3;
         vm.SetValue(x => x.IntegerProperty, vm.Limit);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.IntegerProperty, vm.Limit + 1);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.IntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void Max_ForNullableIntegerPropertyWithMaxValueSelector_ValidatesPropertyAccordingly() {
         var errorMessage = "Value above max";

         var vm = CreateParent(b => b
            .Check(x => x.NullableIntegerProperty)
               .Max(x => x.Limit, errorMessage)
         );
         vm.Limit = 3;

         vm.SetValue(x => x.NullableIntegerProperty, null);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, vm.Limit);

         ValidationAssert.IsValid(vm);

         vm.SetValue(x => x.NullableIntegerProperty, vm.Limit + 1);

         var expectedResult = CreateValidationResult(
            Error(errorMessage).For(vm, x => x.NullableIntegerProperty)
         );

         ValidationAssert.AreEqual(expectedResult, vm.ValidationResult);
      }

      [TestMethod]
      public void ValueInRange_ErrorMessageLocalization() {
         CultureInfo ci = new CultureInfo("de-AT");
         System.Threading.Thread.CurrentThread.CurrentCulture = ci;
         System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

         int max = 10;

         var vm = CreateParent(b => b
            .Check(x => x.IntegerProperty)
               .Max(max)
         );

         vm.SetValue(x => x.IntegerProperty, max + 1);
      }

      private static ParentVM CreateParent(
         Action<RootValidatorBuilder<ParentVM, ParentVM, ParentVMDescriptor>> validatorConfigurationAction,
         ChildVMDescriptor childDescriptor = null
      ) {
         var descriptor = VMDescriptorBuilder
           .OfType<ParentVMDescriptor>()
           .For<ParentVM>()
           .WithProperties((d, c) => {
              var v = c.GetPropertyBuilder();
              d.IntegerProperty = v.Property.Of<int>();
              d.NullableIntegerProperty = v.Property.Of<Nullable<int>>();
              d.Children = v.Collection.Of<ChildVM>(childDescriptor ?? ChildVM.ClassDescriptor);
           })
           .WithValidators(validatorConfigurationAction)
           .Build();

         return new ParentVM(descriptor);
      }

      private static ChildVM CreateChild(
         Action<RootValidatorBuilder<ChildVM, ChildVM, ChildVMDescriptor>> validatorConfigurationAction
      ) {
         var descriptor = CreateChildDescriptor(validatorConfigurationAction);
         return new ChildVM(descriptor);
      }

      private static ChildVMDescriptor CreateChildDescriptor(
         Action<RootValidatorBuilder<ChildVM, ChildVM, ChildVMDescriptor>> validatorConfigurationAction
      ) {
         return VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               d.StringProperty = v.Property.Of<string>();
               d.IntegerProperty = v.Property.Of<int>();
            })
            .WithValidators(validatorConfigurationAction)
            .Build();
      }



      private class ParentVM : TestViewModel<ParentVMDescriptor> {
         public ParentVM(ParentVMDescriptor descriptor)
            : base(descriptor) {
         }

         public IVMCollection<ChildVM> Children {
            get { return GetValue(Descriptor.Children); }
            set { SetValue(Descriptor.Children, value); }
         }

         internal int Limit { get; set; }
      }

      private class ChildVM : TestViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = CreateChildDescriptor(b => {
            b.EnableParentValidation(x => x.StringProperty);
            b.EnableParentValidation(x => x.IntegerProperty);
            b.EnableParentViewModelValidation();
         });

         public ChildVM(string description = null)
            : this(ClassDescriptor, description) {
         }

         public ChildVM(ChildVMDescriptor descriptor, string description = null)
            : base(descriptor, description) {
         }

         public string StringProperty {
            get { return GetValue(Descriptor.StringProperty); }
            set { SetValue(Descriptor.StringProperty, value); }
         }

         public int IntegerProperty {
            get { return GetValue(Descriptor.IntegerProperty); }
            set { SetValue(Descriptor.IntegerProperty, value); }
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> Children { get; set; }
         public IVMPropertyDescriptor<int> IntegerProperty { get; set; }
         public IVMPropertyDescriptor<Nullable<int>> NullableIntegerProperty { get; set; }
      }

      private class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> StringProperty { get; set; }
         public IVMPropertyDescriptor<int> IntegerProperty { get; set; }
      }

      private class CustomChildVMComparer : IEqualityComparer<ChildVM> {

         public bool Equals(ChildVM x, ChildVM y) {
            return x.IntegerProperty == y.IntegerProperty &&
                   x.StringProperty == x.StringProperty;
         }

         public int GetHashCode(ChildVM obj) {
            return HashCodeService.CalculateHashCode(
               obj,
               obj.IntegerProperty,
               obj.StringProperty
            );
         }
      }
   }
}