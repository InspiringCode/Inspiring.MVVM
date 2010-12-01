namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public static class BehaviorChainTemplateRegistry {
      private static Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate> _templates
         = new Dictionary<BehaviorChainTemplateKey, BehaviorChainTemplate>();

      public static void RegisterTemplate(BehaviorChainTemplateKey key, BehaviorChainTemplate template) {
         throw new NotImplementedException();
      }

      public static BehaviorChainTemplate GetTemplate(BehaviorChainTemplateKey withKey) {
         throw new NotImplementedException();
      }
   }
}
