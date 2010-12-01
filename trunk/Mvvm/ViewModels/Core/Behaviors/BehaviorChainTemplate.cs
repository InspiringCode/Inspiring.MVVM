namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Diagnostics.Contracts;

   public sealed class BehaviorChainTemplate {
      internal BehaviorChainItemTemplate[] Items {
         get {
            Contract.Ensures(Contract.Result<BehaviorChainItemTemplate[]>() != null);
            throw new NotImplementedException();
         }
      }

      public void AppendBehaviorTemplate(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

      }

      public void AppendBehaviorTemplate(BehaviorKey key, IBehaviorFactory factory, bool isEnabledByDefault = true) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(factory != null);

      }
   }

   public sealed class BehaviorChainItemTemplate {
      public IBehaviorFactory Factory { get; private set; }
      public BehaviorKey Key { get; private set; }
      public bool IsEnabledByDefault { get; private set; }
   }
}
