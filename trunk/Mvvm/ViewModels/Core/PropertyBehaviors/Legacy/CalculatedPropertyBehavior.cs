namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="IDisplayValueAccessorBehavior"/> that uses the specified delegates
   ///   to implement get/set operation of a <see cref="VMPropertyBase"/>.
   /// </summary>
   internal sealed class CalculatedPropertyAccessor<TVM, TSource, TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IMutabilityCheckerBehavior,
      IManualUpdateBehavior {

      private static readonly Action<TSource, TValue> ThrowingSetter = delegate {
         throw new InvalidOperationException(ExceptionTexts.NoSetterDelegate);
      };

      private PropertyPath<TVM, TSource> _sourceObjectPath;
      private Func<TSource, TValue> _getter;
      private Action<TSource, TValue> _setter;
      private IVMProperty _property;

      public CalculatedPropertyAccessor(
         PropertyPath<TVM, TSource> sourceObjectPath,
         Func<TSource, TValue> getter,
         Action<TSource, TValue> setter = null
      ) {
         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(getter != null);

         _sourceObjectPath = sourceObjectPath;
         _getter = getter;
         _setter = setter ?? ThrowingSetter;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context, ValueStage stage) {
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

      public void UpdateFromSource(IBehaviorContext context) {
         var args = new ChangeArgs(
            ChangeType.PropertyChanged,
            changedVM: context.VM,
            changedProperty: _property
         );

         context.NotifyChange(args);

         this.UpdateFromSourceNext(context);
      }

      public void UpdateSource(IBehaviorContext vm) {
         // Nothing to do
      }
   }
}
