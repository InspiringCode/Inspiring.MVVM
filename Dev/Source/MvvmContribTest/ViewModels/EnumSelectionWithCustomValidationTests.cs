namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EnumSelectionWithCustomValidationTests : TestBase {

      /// <summary>
      /// This integration test was written to reproduce a InvalidOperationException - Sequence contains no matching elements.
      /// The error occured when mapping a EnumSelection and the enum property wasn't assigned with the enum default value.
      /// When the first value of the enum was inserted a validation of the selectedItem couldn't find the value in the collection.
      /// </summary>
      /// <remarks>
      ///    1. Access SelectedItem
      ///    2. Delegate VM accessor accesses AllItems
      ///    3. AllItems is lazy and lazy loading triggers Validation
      ///    4. Validation accesses SelectedItem
      /// </remarks>
      [TestMethod]
      public void GetValueOfSelectedItem_WithViewModelValidationThatAccessesSelectedItem_DoesNotThrowException() {
         TestVM viewModel = new TestVM() { SelectedSourceEnum = TestEnum.ChangedValue };
         var lazyLoadTrigger = viewModel.EnumSelectionProperty.SelectedItem;
      }

      private class TestVM : ViewModel<TestVMDescriptor> {

         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();
               d.EnumSelectionProperty = vm.EnumSelection(x => x.SelectedSourceEnum);
            })
            .WithValidators(b => {
               b.CheckViewModel(args => {
                  var lazyLoadTrigger = args
                     .Owner
                     .GetValue(TestVM.ClassDescriptor.EnumSelectionProperty)
                     .SelectedItem;
               });
            })
            .Build();

         public TestEnum SelectedSourceEnum { get; set; }

         public SingleSelectionVM<TestEnum> EnumSelectionProperty {
            get { return GetValue(Descriptor.EnumSelectionProperty); }
         }

         public TestVM()
            : base(ClassDescriptor) {
         }
      }

      private class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<TestEnum>> EnumSelectionProperty { get; set; }
      }

      enum TestEnum {
         InitialValue,
         ChangedValue
      }

      private class DomainObject {
         public TestEnum PropertyOfEnumerationType { get; set; }
      }
   }
}
