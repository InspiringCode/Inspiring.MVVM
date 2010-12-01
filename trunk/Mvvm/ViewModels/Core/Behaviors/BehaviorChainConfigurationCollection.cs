namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class BehaviorChainConfigurationCollection {
      public void RegisterProperty<TValue>(VMPropertyBase<TValue> property, BehaviorChainTemplateKey behavior) {

      }

      public BehaviorChainConfiguration this[VMPropertyBase forProperty] {
         get {
            throw new NotImplementedException();
         }
      }
   }
}
