namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public static class BehaviorChainTemplateKeys {
      public static readonly BehaviorChainTemplateKey ViewModel = new BehaviorChainTemplateKey();
      public static readonly BehaviorChainTemplateKey Property = new BehaviorChainTemplateKey();
      public static readonly BehaviorChainTemplateKey CollectionProperty = new BehaviorChainTemplateKey();
      public static readonly BehaviorChainTemplateKey CommandProperty = new BehaviorChainTemplateKey();
   }
}
