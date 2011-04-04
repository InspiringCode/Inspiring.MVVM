namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class ValueInitializerBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IIsLoadedIndicatorBehavior {

      private static readonly FieldDefinitionGroup IsLoadedGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<bool> _isLoadedField;
      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _isLoadedField = new DynamicFieldAccessor<bool>(context, IsLoadedGroup);
         _property = context.Property;

         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         if (!GetLoaded(context)) {
            Load(context);
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

      /// <summary>
      ///   This behavior calls <see cref="IValueInitializerBehavior.InitializeValue"/>
      ///   directly on property behavior chain (instead of on the rest of the chain
      ///   that follows this behavior) to resolve some tricky behavior ordering
      ///   issues (some behavior need the <see cref="IIsLoadedIndicatorBehavior"/> 
      ///   but also need to have <see cref="IValueInitializerBehavior.InitializeValue"/>
      ///   called.
      /// </summary>
      private void Load(IBehaviorContext context) {
         SetLoaded(context);

         _property
            .Behaviors
            .InitializeValueNext(context);
      }
   }
}
