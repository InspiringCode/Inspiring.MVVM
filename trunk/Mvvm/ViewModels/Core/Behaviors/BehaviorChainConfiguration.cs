namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class BehaviorChainConfiguration {
      private List<BehaviorChainItemConfiguration> _items = new List<BehaviorChainItemConfiguration>();

      [Pure]
      public bool Contains(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

         return _items.Find(x => x.Key == key) != null;
      }

      /// <summary>
      ///   Makes sure that the behavior defined by '<paramref name="key"/>' gets
      ///   included in the behavior chain returned by <see cref="CreateChain"/>.
      /// </summary>
      /// <param name="behaviorInstance">
      ///   If the <see cref="BehaviorChainTemplate"/> does not specify a <see 
      ///   cref="IBehaviorFactory"/> the behavior instance that should be used
      ///   in the behavior chain must be set with this parameter (an <see 
      ///   cref="ArgumentException"/> is thrown otherwise).
      /// </param>
      public void Enable(BehaviorKey key, IBehavior behaviorInstance = null) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentException>(Contains(key));

         var item = GetItem(key);

         if (behaviorInstance != null) {
            item.Instance = behaviorInstance;
         } else {
            if (item.Instance != null) {
               throw new ArgumentException(
                  ExceptionTexts.CannotEnableBehavior.FormatWith(key)
               );
            }
         }

         item.Instance = behaviorInstance;
         item.IsEnabled = true;
      }

      /// <summary>
      ///   Calls the '<paramref name="configurationAction"/>' with the behavior
      ///   specified by '<paramref name="key"/>' that will be inserted in the 
      ///   behavior chain. This method implicitly calls <see 
      ///   cref="Enable(BehaviorKey)"/>.
      /// </summary>
      /// <typeparam name="T">
      ///   The type of the behavior to configure. This may be the concrete type
      ///   of the behavior of a base type/interface.
      /// </typeparam>
      public void ConfigureBehavior<T>(
         BehaviorKey key,
         Action<T> configurationAction
      ) where T : IBehavior {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentException>(Contains(key));
         Contract.Requires<ArgumentNullException>(configurationAction != null);

         BehaviorChainItemConfiguration item = GetItem(key);
         Enable(key);

         T behavior = (T)item.Instance;
         configurationAction(behavior);
      }

      /// <summary>
      ///   Adds a disabled behavior configuration to the end of the chain.
      /// </summary>
      internal void Append(BehaviorKey key) {
         Contract.Requires(key != null);

         _items.Add(new BehaviorChainItemConfiguration(key));
      }

      /// <summary>
      ///   Adds a behavior configuration to the end of the chain.
      /// </summary>
      internal void Append(BehaviorKey key, IBehavior instance, bool isEnabled = true) {
         Contract.Requires(key != null);
         Contract.Requires(instance != null);

         var item = new BehaviorChainItemConfiguration(key);
         item.Instance = instance;
         item.IsEnabled = isEnabled;

         _items.Add(item);
      }

      /// <summary>
      ///   Creates a <see cref="BehaviorChain"/> with <see cref="IBehavior"/>s
      ///   as configured by this object.
      /// </summary>
      /// <returns></returns>
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
