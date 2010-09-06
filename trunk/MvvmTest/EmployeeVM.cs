namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class EmployeeVM : ViewModel<VMDescriptor> {
      public static readonly EmployeeVMDescriptor Descriptor = new EmployeeVMDescriptor();

      public EmployeeVM()
         : base(Descriptor) {

      }
   }

   internal sealed class EmployeeVMDescriptor : VMDescriptor {

   }
}
