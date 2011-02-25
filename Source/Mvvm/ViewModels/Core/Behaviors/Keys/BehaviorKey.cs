namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class BehaviorKey {
      private readonly string _key;

      public BehaviorKey(string key) {
         Contract.Requires(!String.IsNullOrEmpty(key));

         _key = key;
      }

      public override string ToString() {
         return String.Format("{{BehaviorKey {0}}}", _key);
      }
   }
}
