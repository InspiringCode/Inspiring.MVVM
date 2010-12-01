namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class BehaviorChainTemplate {
      //internal BehaviorChainItemTemplate[] Items {
      //   get {
      //      Contract.Ensures(Contract.Result<BehaviorChainItemTemplate[]>() != null);
      //      throw new NotImplementedException();
      //   }
      //}

      public void AppendBehaviorTemplate(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

      }

      public void AppendBehaviorTemplate(BehaviorKey key, IBehaviorFactory factory, bool isEnabledByDefault = true) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(factory != null);

      }

      internal BehaviorChainConfiguration CreateConfiguration<T>() {
         throw new NotImplementedException();

         //   Contract.Requires(template != null);
         //   Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);

         //   var items = new List<BehaviorChainItemConfiguration>();

         //   foreach (BehaviorChainItemTemplate itemTemplate in template.Items) {
         //      var item = new BehaviorChainItemConfiguration(itemTemplate.Key);

         //      if (itemTemplate.Factory != null) {
         //         item.Instance = itemTemplate.Factory.Create<T>();
         //      }

         //      item.IsEnabled = itemTemplate.IsEnabledByDefault;

         //      items.Add(item);
         //   }

         //   return new BehaviorChainConfiguration(items);
      }

      private class BehaviorChainItemTemplate {
         public IBehaviorFactory Factory { get; private set; }
         public BehaviorKey Key { get; private set; }
         public bool IsEnabledByDefault { get; private set; }
      }
   }
}
