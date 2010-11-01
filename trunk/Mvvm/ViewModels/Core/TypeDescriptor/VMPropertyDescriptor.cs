namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   /// <summary>
   ///   A <see cref="System.ComponentModel.PropertyDescriptor"/> that allows
   ///   the WPF binding infrastructure to bind to VMProperties even if no CLR
   ///   wrapper is defined on the ViewModel class.
   /// </summary>
   internal class VMPropertyDescriptor : SimplePropertyDescriptor {
      private VMPropertyBase _property;

      public VMPropertyDescriptor(VMPropertyBase property)
         : base(
            property.PropertyName,
            property.PropertyType,
            componentType: typeof(IViewModel)
         ) {

         _property = property;
      }

      public override object GetValue(object component) {
         IBehaviorContext vm = CastComponent(component);
         return _property.GetDisplayValue(vm);
      }

      public override void SetValue(object component, object value) {
         IBehaviorContext vm = CastComponent(component);
         _property.SetDisplayValue(vm, value);
      }

      public void RaiseValueChanged(IBehaviorContext vm) {
         OnValueChanged(vm, EventArgs.Empty);
      }

      private IBehaviorContext CastComponent(object component) {
         IBehaviorContext vm = component as IBehaviorContext;

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
