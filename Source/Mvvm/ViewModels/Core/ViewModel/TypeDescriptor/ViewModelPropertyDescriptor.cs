namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   /// <summary>
   ///   A <see cref="System.ComponentModel.PropertyDescriptor"/> that allows
   ///   the WPF binding infrastructure to bind to VM properties even if no CLR
   ///   wrapper is defined on the ViewModel class.
   /// </summary>
   internal class ViewModelPropertyDescriptor : SimplePropertyDescriptor {
      public ViewModelPropertyDescriptor(IVMPropertyDescriptor property, Type propertyType)
         : base(
            property.PropertyName,
            propertyType: propertyType,
            componentType: typeof(IViewModel)
         ) {
         Check.NotNull(property, nameof(property));

         Property = property;
      }

      public IVMPropertyDescriptor Property { get; private set; }

      public override object GetValue(object component) {
         IViewModel vm = CastComponent(component);
         return vm.Kernel.GetDisplayValue(Property);
      }

      public override void SetValue(object component, object value) {
         IViewModel vm = CastComponent(component);
         vm.Kernel.SetDisplayValue(Property, value);
      }

      public void RaiseValueChanged(IViewModel vm) {
         OnValueChanged(vm, EventArgs.Empty);
      }

      private IViewModel CastComponent(object component) {

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

   internal class ViewModelPropertyDescriptor<TValue> : ViewModelPropertyDescriptor {
      public ViewModelPropertyDescriptor(IVMPropertyDescriptor<TValue> property, Type propertyType)
         : base(property, propertyType) {
      }
   }
}
