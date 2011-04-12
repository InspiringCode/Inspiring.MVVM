namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class BehaviorFactory : IBehaviorFactory {
      private Dictionary<BehaviorKey, Func<IBehavior>> _behaviors
         = new Dictionary<BehaviorKey, Func<IBehavior>>();

      public IBehavior Create(BehaviorKey key) {
         Func<IBehavior> factoryFunction;

         if (!_behaviors.TryGetValue(key, out factoryFunction)) {
            throw new ArgumentException(ECommon
               .BehaviorForKeyCannotBeCreated
               .FormatWith(key)
            );
         }

         return factoryFunction();
      }

      public BehaviorFactory RegisterBehavior<T>(
         BehaviorKey key
      ) where T : IBehavior, new() {
         Contract.Requires(key != null);

         RegisterBehavior(key, () => new T());
         return this;
      }

      public BehaviorFactory RegisterBehavior(BehaviorKey key, Func<IBehavior> factoryFunction) {
         Contract.Requires(key != null);
         Contract.Requires(factoryFunction != null);

         _behaviors[key] = factoryFunction;
         return this;
      }
   }
}
