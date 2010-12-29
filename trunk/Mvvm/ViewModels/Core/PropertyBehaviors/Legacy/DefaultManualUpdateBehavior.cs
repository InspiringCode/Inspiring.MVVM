﻿using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DefaultManualUpdateBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IManualUpdateBehavior {

      private VMPropertyBase<TValue> _property;

      public void UpdatePropertyFromSource(IBehaviorContext vm) {
         throw new NotImplementedException("TODO2");
         //vm.RaisePropertyChanged(_property);
      }

      public void UpdatePropertySource(IBehaviorContext vm) {
         // Nothing to do
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = (VMPropertyBase<TValue>)context.Property;
         this.InitializeNext(context);
      }
   }
}
