namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EnumSelectionWithCustomValidationTests {

      /// <summary>
      /// This integration test was written to reproduce a InvalidOperationException - Sequence contains no matching elements.
      /// The error occured when mapping a EnumSelection and the enum property wasn't assigned with the enum default value.
      /// When the first value of the enum was inserted a validation of the selectedItem couldn't find the value in the collection.
      /// </summary>
      [TestMethod]
      public void AccessSingleSelectionVM_withViewModelValidationThatAccessesSelectedItem_doesNotThrowStackOverflowException() {
         DomainObjectVM viewModel = new DomainObjectVM();
         viewModel.PropertyOfEnumerationType = Enumeration.Value2;

         viewModel.Enumeration.Equals(null);
      }

      private class DomainObjectVM : ViewModel<DomainObjectVMDescriptor> {

         public static readonly DomainObjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<DomainObjectVMDescriptor>()
            .For<DomainObjectVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.PropertyOfEnumerationType = vm.EnumSelection(x => x.PropertyOfEnumerationType);
            })
            .WithValidators(b => {
               b.CheckViewModel((vm, args) => {
                  vm.GetValue(vm.Descriptor.PropertyOfEnumerationType).SelectedItem.Equals(null);
               });
            })
            .Build();

         public Enumeration PropertyOfEnumerationType { get; set; }

         public Enumeration Enumeration {
            get {
               return GetValue(Descriptor.PropertyOfEnumerationType)
                  .SelectedItem
                  .Source;
            }
         }

         public DomainObjectVM()
            : base(ClassDescriptor) {

         }
      }

      private class DomainObjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<Enumeration>> PropertyOfEnumerationType { get; set; }
      }

      enum Enumeration {
         Value1,
         Value2
      }

      private class DomainObject {
         public Enumeration PropertyOfEnumerationType { get; set; }
      }
   }
}
