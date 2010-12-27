﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class PropertyChangedBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private IVMProperty _property;

      public TValue GetValue(IBehaviorContext vm, ValueStage stage) {
         RequireInitialized();
         return GetNextBehavior<IValueAccessorBehavior<TValue>>().GetValue(vm, stage);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         TValue oldValue = GetValue(context, ValueStage.PostValidation);
         GetNextBehavior<IValueAccessorBehavior<TValue>>().SetValue(context, value);

         if (!Object.Equals(value, oldValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            context.NotifyChange(args);
         }
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
         SetInitialized();
      }
   }
}
