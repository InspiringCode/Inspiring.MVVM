namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class EmployeeVM : ViewModel<VMDescriptor> {
      public static readonly EmployeeVMDescriptor ClassDescriptor = new EmployeeVMDescriptor();

      public EmployeeVM()
         : base() {

      }
   }

   internal sealed class EmployeeVMDescriptor : VMDescriptor {

   }
}
