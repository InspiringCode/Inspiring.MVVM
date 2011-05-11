namespace Inspiring.Mvvm.ViewModels.Core {

   internal interface IPropertySelector {
      IVMPropertyDescriptor GetProperty(IVMPropertyDescriptor descriptor);
   }
}
