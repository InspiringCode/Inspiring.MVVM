namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class ValidationForNullableDecimalTests {

      [TestMethod]
      public void ValueDisplayValue_ChangeFromValidToInvalidValue_SetsDisplayValueToInvlaidValueAndValueRemainsUnChanged() {
         var vm = new TestVM();

         decimal? validValue = 1;
         vm.SetValue(x => x.Value, validValue);

         Assert.AreEqual(validValue, vm.GetValidatedValue());
         Assert.AreEqual(validValue, vm.GetDisplayValue());

         decimal? invalidValue = 2;
         vm.InvalidateValue = true;

         vm.SetDisplayValue(invalidValue);

         Assert.AreEqual(validValue, vm.GetValidatedValue());
         Assert.AreEqual(invalidValue, vm.GetDisplayValue());
      }

      private sealed class TestVM : ViewModel<TestVMDescriptor> {

         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.Value = vm.Property.MapsTo(x => x.Value);
            })
            .WithValidators(b => {
               b.Check(x => x.Value)
                  .Custom(args => {
                     if (args.Owner.InvalidateValue) {
                        args.AddError("Test triggered error");
                     }
                  });
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {

         }

         internal void SetDisplayValue(Nullable<decimal> value) {
            SetDisplayValue(Descriptor.Value, value);
         }

         internal Nullable<decimal> GetDisplayValue() {
            return (Nullable<decimal>)GetDisplayValue(Descriptor.Value);
         }

         internal Nullable<decimal> GetValidatedValue() {
            return GetValidatedValue(Descriptor.Value);
         }

         internal bool InvalidateValue { get; set; }

         private Nullable<decimal> Value { get; set; }
      }

      private sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<Nullable<decimal>> Value { get; set; }
      }
   }
}
