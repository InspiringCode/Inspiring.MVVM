namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Diagnostics.Contracts;

   internal sealed class BehaviorChainConfiguration {
      private List<BehaviorChainItemConfiguration> _items = new List<BehaviorChainItemConfiguration>();

      private BehaviorChainConfiguration(List<BehaviorChainItemConfiguration> items) {
         _items = items;
      }

      public void Enable(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

         var item = GetItem(key);
         item.IsEnabled = true;
      }

      public void Enable(BehaviorKey key, IBehavior behaviorInstance) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(behaviorInstance != null);

         var item = GetItem(key);
         item.Instance = behaviorInstance;
         item.IsEnabled = true;         
      }

      public T GetBehaviorInstance<T>(BehaviorKey key) where T : IBehavior {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Ensures(Contract.Result<IBehavior>() != null);

         return (T)GetItem(key).Instance;
      }

      internal BehaviorChain CreateChain() {
         Contract.Ensures(Contract.Result<BehaviorChain>() != null);

         var chain = new BehaviorChain();
         IBehavior currentBehavior = chain;

         foreach (IBehavior instance in _items) {
            currentBehavior.Successor = instance;
            currentBehavior = instance;
         }

         return chain;
      }

      internal static BehaviorChainConfiguration FromTemplate<T>(BehaviorChainTemplate template) {
         Contract.Requires(template != null);
         Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);

         var items = new List<BehaviorChainItemConfiguration>();

         foreach (BehaviorChainItemTemplate itemTemplate in template.Items) {
            var item = new BehaviorChainItemConfiguration(itemTemplate.Key);

            if (itemTemplate.Factory != null) {
               item.Instance = itemTemplate.Factory.Create<T>();
            }

            item.IsEnabled = itemTemplate.IsEnabledByDefault;

            items.Add(item);
         }

         return new BehaviorChainConfiguration(items);
      }

      private BehaviorChainItemConfiguration GetItem(BehaviorKey key) {
         var item = _items.Find(x => x.Key == key);

         if (item != null) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorKeyNotInConfiguration.FormatWith(key)
            );
         }

         return item;
      }

      private class BehaviorChainItemConfiguration {
         public BehaviorChainItemConfiguration(BehaviorKey key) {
            Key = key;
         }

         public BehaviorKey Key { get; private set; }
         public bool IsEnabled { get; set; }
         public IBehavior Instance { get; set; }

         [ContractInvariantMethod]
         void ObjectInvariant() {
            Contract.Invariant(IsEnabled ? Instance != null : true);
         }
      }
   }
}
