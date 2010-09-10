namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A <see cref="IAccessPropertyBehavior"/> that uses the specified delegates
   ///   to implement get/set operation of a <see cref="VMProperty"/>.
   /// </summary>
   public sealed class CalculatedPropertyBehavior<TSource, TValue> : VMPropertyBehavior, IAccessPropertyBehavior<TValue> {
      private Func<TSource, TValue> _getter;
      private Action<TSource, TValue> _setter;
      private IAccessPropertyBehavior<TSource> _innerAccessor;

      public CalculatedPropertyBehavior(
         Func<TSource, TValue> getter,
         Action<TSource, TValue> setter = null,
         IAccessPropertyBehavior<TSource> innerAccessor = null
      ) {
         Contract.Requires<ArgumentNullException>(getter != null);

         _getter = getter;
         _setter = setter ?? delegate {
            throw new InvalidOperationException(ExceptionTexts.NoSetterDelegate);
         };
         _innerAccessor = innerAccessor;
      }

      public override BehaviorPosition Position {
         get { return BehaviorPosition.SourceValueAccessor; }
      }

      public TValue GetValue(IBehaviorContext vm) {
         return _getter(GetSourceValue(vm));
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         TSource sourceValue = GetSourceValue(vm);
         _setter(sourceValue, value);
      }

      private TSource GetSourceValue(IBehaviorContext vm) {
         return _innerAccessor != null ?
            _innerAccessor.GetValue(vm) :
            (TSource)(object)vm;
      }
   }
}
