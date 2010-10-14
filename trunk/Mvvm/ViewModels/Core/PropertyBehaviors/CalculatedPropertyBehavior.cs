namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A <see cref="IAccessPropertyBehavior"/> that uses the specified delegates
   ///   to implement get/set operation of a <see cref="VMProperty"/>.
   /// </summary>
   public sealed class CalculatedPropertyBehavior<TSource, TValue> :
      Behavior,
      IAccessPropertyBehavior<TValue>,
      IMutabilityCheckerBehavior,
      IManuelUpdateBehavior {

      private static readonly Action<TSource, TValue> _throwingSetter = delegate {
         throw new InvalidOperationException(ExceptionTexts.NoSetterDelegate);
      };

      private VMPropertyBase<TValue> _property;
      private Func<TSource, TValue> _getter;
      private Action<TSource, TValue> _setter;

      public CalculatedPropertyBehavior(
         Func<TSource, TValue> getter,
         Action<TSource, TValue> setter = null
      ) {
         Contract.Requires<ArgumentNullException>(getter != null);

         _getter = getter;
         _setter = setter ?? _throwingSetter;
      }

      public TValue GetValue(IBehaviorContext vm) {
         return _getter(GetSourceValue(vm));
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         TSource sourceValue = GetSourceValue(vm);
         _setter(sourceValue, value);
      }

      private TSource GetSourceValue(IBehaviorContext vm) {
         IAccessPropertyBehavior<TSource> innerAccessor;

         return TryGetBehavior(out innerAccessor) ?
            innerAccessor.GetValue(vm) :
            (TSource)vm;
      }

      public bool IsMutable(IBehaviorContext vm) {
         return _setter != _throwingSetter;
      }

      public void UpdateFromSource(IBehaviorContext vm) {
         vm.RaisePropertyChanged(_property);

         // TODO: Call next...
      }

      public void UpdateSource(IBehaviorContext vm) {
         // Nothing to do
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = (VMPropertyBase<TValue>)context.Property;
      }
   }
}
