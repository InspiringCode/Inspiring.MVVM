namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A <see cref="IAccessPropertyBehavior"/> that uses the specified delegates
   ///   to implement get/set operation of a <see cref="VMProperty"/>.
   /// </summary>
   public sealed class CalculatedPropertyBehavior<TSource, TValue> : Behavior, IAccessPropertyBehavior<TValue> {
      private Func<TSource, TValue> _getter;
      private Action<TSource, TValue> _setter;

      public CalculatedPropertyBehavior(
         Func<TSource, TValue> getter,
         Action<TSource, TValue> setter = null
      ) {
         Contract.Requires<ArgumentNullException>(getter != null);

         _getter = getter;
         _setter = setter ?? delegate {
            throw new InvalidOperationException(ExceptionTexts.NoSetterDelegate);
         };
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
   }
}
