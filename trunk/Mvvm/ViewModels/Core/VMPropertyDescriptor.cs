namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;
   using System.ComponentModel;

   internal abstract class SimplePropertyDescriptor : PropertyDescriptor {
      public SimplePropertyDescriptor(string propertyName)
         : base(propertyName, attrs: null) {
      }

      public override bool IsReadOnly {
         get { return false; }
      }

      public override bool CanResetValue(object component) {
         return false;
      }

      public override void ResetValue(object component) {
      }

      public override bool ShouldSerializeValue(object component) {
         return false;
      }
   }

   internal class CustomPropertyDescriptor : SimplePropertyDescriptor {
      private VMProperty _property;

      public CustomPropertyDescriptor(VMProperty property)
         : base(property.PropertyName) {
         _property = property;
      }

      public override Type ComponentType {
         get { return typeof(ViewModel<>); }
      }

      public override Type PropertyType {
         get { return _property.PropertyType; }
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
