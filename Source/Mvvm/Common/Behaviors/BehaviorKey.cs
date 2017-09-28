namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class BehaviorKey {
      private readonly string _key;

      public BehaviorKey(string key) {
         Check.NotEmpty(key, nameof(key));

         _key = key;
      }

      public override string ToString() {
         return String.Format("{{BehaviorKey {0}}}", _key);
      }
   }
}
