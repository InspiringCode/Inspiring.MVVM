namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class TestViewModel<TDescriptor> :
      ViewModel<TDescriptor>
      where TDescriptor : VMDescriptorBase {

      private string _name;

      public TestViewModel(TDescriptor descriptor, string name = null)
         : base(descriptor) {
         _name = name;
         NotifyChangeInvocations = new List<ChangeArgs>();
      }

      public List<ChangeArgs> NotifyChangeInvocations {
         get;
         private set;
      }

      public void Load(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         Load(propertySelector(Descriptor));
      }

      public bool IsLoaded(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         return Kernel.IsLoaded(propertySelector(Descriptor));
      }

      public void Revalidate(ValidationScope scope = ValidationScope.Self) {
         base.Revalidate(scope);
      }

      public void Revalidate(Func<TDescriptor, IVMPropertyDescriptor> propertySelector, ValidationScope scope = ValidationScope.Self) {
         var property = propertySelector(Descriptor);
         Kernel.Revalidate(property, ValidationMode.CommitValidValues, scope);
      }

      public void RevalidateViewModelValidations() {
         Revalidator.RevalidateViewModelValidations(this);
      }

      public void Refresh(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         var property = propertySelector(Descriptor);
         base.Refresh(property);
      }

      public override string ToString() {
         return _name ?? GetType().Name;
      }

      protected override void OnChange(ChangeArgs args) {
         NotifyChangeInvocations.Add(args);
      }
   }
}
