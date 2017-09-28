namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   // TODO: Null handling

   /// <summary>
   ///   A <see cref="IDisplayValueAccessorBehavior"/> that uses the specified delegates
   ///   to implement get/set operation of a <see cref="IVMPropertyDescriptor"/>.
   /// </summary>
   internal sealed class DelegateValueAccessor<TVM, TSource, TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>,
      IMutabilityCheckerBehavior {

      private static readonly Action<TSource, TValue> ThrowingSetter = delegate {
         throw new InvalidOperationException(ExceptionTexts.NoSetterDelegate);
      };

      private PropertyPath<TVM, TSource> _sourceObjectPath;
      private Func<TSource, TValue> _getter;
      private Action<TSource, TValue> _setter;

      public DelegateValueAccessor(
         PropertyPath<TVM, TSource> sourceObjectPath,
         Func<TSource, TValue> getter,
         Action<TSource, TValue> setter = null
      ) {
         Check.NotNull(sourceObjectPath, nameof(sourceObjectPath));
         Check.NotNull(getter, nameof(getter));

         _sourceObjectPath = sourceObjectPath;
         _getter = getter;
         _setter = setter ?? ThrowingSetter;
      }

      public TValue GetValue(IBehaviorContext context) {
         var sourceObject = _sourceObjectPath.GetValue((TVM)context.VM);
         return _getter(sourceObject);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         var sourceObject = _sourceObjectPath.GetValue((TVM)context.VM);
         _setter(sourceObject, value);
      }

      public bool IsMutable(IBehaviorContext context) {
         return _setter != ThrowingSetter;
      }
   }
}
