namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class TestViewModel<TDescriptor> :
      ViewModel<TDescriptor>
      where TDescriptor : VMDescriptorBase {

      private string _name;

      public TestViewModel(TDescriptor descriptor, string name = null)
         : base(descriptor) {
         _name = name;
      }

      public void Revalidate() {
         base.Revalidate();
      }

      public void Revalidate(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         var property = propertySelector(Descriptor);
         Kernel.Revalidate(property, ValidationMode.CommitValidValues);
      }

      public void RevalidateViewModelValidations() {
         Revalidator.RevalidateViewModelValidations(this);
      }

      public override string ToString() {
         return _name ?? GetType().Name;
      }
   }
}
