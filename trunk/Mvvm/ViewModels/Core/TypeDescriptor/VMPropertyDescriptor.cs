namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A <see cref="System.ComponentModel.PropertyDescriptor"/> that allows
   ///   the WPF binding infrastructure to bind to VM properties even if no CLR
   ///   wrapper is defined on the ViewModel class.
   /// </summary>
   internal class TypeDescriptorPropertyDescriptor : SimplePropertyDescriptor {
      private IVMPropertyDescriptor _property;

      public TypeDescriptorPropertyDescriptor(IVMPropertyDescriptor property)
         : base(
            property.PropertyName,
            property.PropertyType,
            componentType: typeof(IViewModel)
         ) {
         Contract.Requires(property != null);

         _property = property;
      }

      public override object GetValue(object component) {
         IViewModel vm = CastComponent(component);
         return vm.Kernel.GetDisplayValue(_property);
      }

      public override void SetValue(object component, object value) {
         IViewModel vm = CastComponent(component);
         vm.Kernel.SetDisplayValue(_property, value);
      }

      public void RaiseValueChanged(IViewModel vm) {
         OnValueChanged(vm, EventArgs.Empty);
      }

      private IViewModel CastComponent(object component) {
         Contract.Ensures(Contract.Result<IViewModel>() != null);

         var vm = component as IViewModel;

         if (vm == null) {
            throw new ArgumentException(
               ExceptionTexts.InvalidComponentInstance.FormatWith(component),
               "component"
            );
         }

         return vm;
      }
   }
}
