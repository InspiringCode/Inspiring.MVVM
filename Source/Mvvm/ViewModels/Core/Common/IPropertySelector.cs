namespace Inspiring.Mvvm.ViewModels.Core {

   internal interface IPropertySelector {
      string PropertyName { get; }

      IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor);
   }
}
