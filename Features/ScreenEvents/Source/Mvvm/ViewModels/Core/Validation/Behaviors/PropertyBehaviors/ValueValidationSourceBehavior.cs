namespace Inspiring.Mvvm.ViewModels.Core {

   internal class ValueValidationSourceBehavior<TValue> :
      PropertyValidationSourceBehaviorBase<TValue>,
      IValueAccessorBehavior<TValue>,
      IValidatedValueAccessorBehavior<TValue> {

      public ValueValidationSourceBehavior()
         : base(ValidationStep.Value, ValueStage.Value) {
      }

      public TValue GetValue(IBehaviorContext context) {
         return GetInvalidValueOrNext(context);
      }

      public TValue GetValidatedValue(IBehaviorContext context) {
         return GetValueNext(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         SetValueNextIfValidationSucceeds(context, value);
      }

      protected override TValue GetValueNext(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      protected override void SetValueNext(IBehaviorContext context, TValue value) {
         this.SetValueNext<TValue>(context, value);
      }
   }
}