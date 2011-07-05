namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;

   public interface IPropertyDescriptorProviderBehavior : IBehavior {
      PropertyDescriptor PropertyDescriptor { get; }
   }
}
