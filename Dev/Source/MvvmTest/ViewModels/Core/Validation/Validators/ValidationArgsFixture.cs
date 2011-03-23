namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationArgsFixture : ValidationTestBase {
      protected sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.DelegatesTo(vm => {
                  vm.NamePropertyAccesses++;
                  return vm._name;
               });
            })
            .Build();

         private string _name;

         public EmployeeVM(string name = "Test employee name")
            : base(ClassDescriptor) {
            _name = name;
         }

         public int NamePropertyAccesses { get; set; }
      }

      protected sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}