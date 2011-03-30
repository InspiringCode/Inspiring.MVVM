namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class ValueInitializerBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IIsLoadedIndicatorBehavior {

      private static readonly FieldDefinitionGroup IsLoadedGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<bool> _isLoadedField;

      public void Initialize(BehaviorInitializationContext context) {
         _isLoadedField = new DynamicFieldAccessor<bool>(context, IsLoadedGroup);
         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         if (!GetLoaded(context)) {
            SetLoaded(context);
            this.InitializeValueNext(context);
         }

         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         SetLoaded(context);
         this.SetValueNext(context, value);
      }

      public bool IsLoaded(IBehaviorContext context) {
         return GetLoaded(context) && this.IsLoadedNext(context);
      }

      private bool GetLoaded(IBehaviorContext context) {
         return _isLoadedField.GetWithDefault(context, false);
      }

      private void SetLoaded(IBehaviorContext context) {
         _isLoadedField.Set(context, true);
      }
   }
}
