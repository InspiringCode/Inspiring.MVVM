using System.Diagnostics.Contracts;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class PropertyValidationBehavior<TValue> :
      Behavior,
      IInitializationPropertyBehavior,
      IAccessPropertyBehavior<TValue>,
      IValidationStatePropertyBehavior {

      private IVMProperty _property;

      public void Initialize(PropertyBehaviorInitializationContext context) {
         _property = context.Property;
      }

      public TValue GetValue(IBehaviorContext vm) {
         return this.CallNext(x => x.GetValue(vm));
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         this.CallNext(x => x.SetValue(vm, value));
      }

      public ValidationState GetValidationState(IPropertyBehaviorContext context) {
         return null;
      }

      internal void Validate(IPropertyBehaviorContext context, ValidationContext validationContext) {
         Contract.Assert(_property != null, "Behavior was not properly initialized.");

         ValidationState newState = new ValidationState();
         context.NotifyPropertyValidating(_property, newState);
      }

      internal void Validate(IPropertyBehaviorContext context) {

      }
   }
}
