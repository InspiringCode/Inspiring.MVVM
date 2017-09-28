namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class BehaviorChainTemplateKey {
      private readonly string _key;

      public BehaviorChainTemplateKey(string key) {
         Check.NotEmpty(key, nameof(key));

         _key = key;
      }

      public override string ToString() {
         return String.Format("{{BehaviorKey {0}}}", _key);
      }
   }
}
