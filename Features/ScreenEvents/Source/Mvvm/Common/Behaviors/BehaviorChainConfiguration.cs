namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Holds the transient configuration of the <see cref="IBehavior"/> objects
   ///   the will finally consitute a <see cref="BehaviorChain"/> and provides
   ///   methods to manipulate the behavior objects.
   /// </summary>
   /// <remarks>
   ///   In contrast to a <see cref="BehaviorChainTemplate"/> which is the same
   ///   for all <see cref="BehaviorChain"/> instances of certain type, a 
   ///   <see cref="BehaviorChainConfiguration"/> is always bound to a single
   ///   <see cref="BehaviorChain"/> instance. Once the <see cref="BehaviorChain"/>
   ///   is created the <see cref="BehaviorChainConfiguration"/> is not needed
   ///   anymore and should be discarded.
   /// </remarks>
   public sealed class BehaviorChainConfiguration : SealableObject {
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
      ///   Specifies or overrides the default behavior instance provided by the
      ///   <see cref="BehaviorChainTemplate"/>. This parameter is required if
      ///   the template has defined the behavior with the <see 
      ///   cref="DefaultBehaviorState.DisabledWithoutFactory"/> option.
      /// </param>
      public void Enable(BehaviorKey key, IBehavior behaviorInstance = null) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentException>(Contains(key));
         RequireNotSealed();

         var item = GetItem(key);

         if (behaviorInstance != null) {
            item.Instance = behaviorInstance;
         } else {
            if (item.Instance == null) {
               throw new ArgumentException(
                  ExceptionTexts.CannotEnableBehavior.FormatWith(key)
               );
            }
         }

         item.IsEnabled = true;
      }

      /// <summary>
      ///   Removes the behavior defined by '<paramref name="key"/>' from the behavior
      ///   chain returned by <see cref="CreateChain"/>.
      /// </summary>
      public void Disable(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentException>(Contains(key));
         RequireNotSealed();

         var item = GetItem(key);
         item.IsEnabled = false;
      }

      /// <summary>
      ///   Calls the '<paramref name="configurationAction"/>' with the behavior
      ///   specified by '<paramref name="key"/>' that will be inserted in the 
      ///   behavior chain. This method implicitly calls <see 
      ///   cref="Enable(BehaviorKey)"/>.
      /// </summary>
      /// <typeparam name="T">
      ///   The type of the behavior to configure. This may be the concrete type
      ///   of the behavior or a base type/interface.
      /// </typeparam>
      public void ConfigureBehavior<T>(
         BehaviorKey key,
         Action<T> configurationAction
      ) where T : IBehavior {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentException>(Contains(key));
         Contract.Requires<ArgumentNullException>(configurationAction != null);
         RequireNotSealed();

         BehaviorChainItemConfiguration item = GetItem(key);
         Enable(key);

         T behavior = (T)item.Instance;
         configurationAction(behavior);
      }

      public TBehavior GetBehavior<TBehavior>(BehaviorKey withKey) where TBehavior : IBehavior {
         Contract.Requires<ArgumentNullException>(withKey != null);
         Contract.Requires<ArgumentException>(Contains(withKey));
         Contract.Ensures(Contract.Result<TBehavior>() != null);
         RequireNotSealed();

         BehaviorChainItemConfiguration item = GetItem(withKey);
         return (TBehavior)item.Instance;
      }

      internal static BehaviorChainConfiguration GetConfiguration(
         BehaviorChainTemplateKey templateKey,
         IBehaviorFactoryConfiguration factoryConfiguration
      ) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(templateKey);
         return template.CreateConfiguration(factoryConfiguration);
      }

      /// <summary>
      ///   Adds a disabled behavior configuration to the end of the chain.
      /// </summary>
      internal void Append(BehaviorKey key) {
         Contract.Requires(key != null);
         RequireNotSealed();

         _items.Add(new BehaviorChainItemConfiguration(key));
      }

      /// <summary>
      ///   Adds a disabled behavior configuration to the end of the chain.
      /// </summary>
      internal void Append(BehaviorKey key, IBehavior instance) {
         Contract.Requires(key != null);
         Contract.Requires(instance != null);
         RequireNotSealed();

         var item = new BehaviorChainItemConfiguration(key) { Instance = instance };
         _items.Add(item);
      }

      /// <summary>
      ///   Adds a disabled behavior configuration to the top of the chain.
      /// </summary>
      internal void Prepend(BehaviorKey key, IBehavior instance) {
         Contract.Requires(key != null);
         Contract.Requires(instance != null);
         RequireNotSealed();

         var item = new BehaviorChainItemConfiguration(key) { Instance = instance };
         _items.Insert(0, item);
      }

      /// <summary>
      ///   Creates a <see cref="BehaviorChain"/> with <see cref="IBehavior"/>s
      ///   as configured by this object.
      /// </summary>
      internal BehaviorChain CreateChain() {
         Contract.Ensures(Contract.Result<BehaviorChain>() != null);

         var chain = new BehaviorChain();
         IBehavior currentBehavior = chain;

         foreach (BehaviorChainItemConfiguration itemConfiguration in _items) {
            if (itemConfiguration.IsEnabled) {
               currentBehavior.Successor = itemConfiguration.Instance;
               currentBehavior = itemConfiguration.Instance;
            }
         }

         return chain;
      }

      private BehaviorChainItemConfiguration GetItem(BehaviorKey key) {
         var item = _items.Find(x => x.Key == key);

         if (item == null) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorKeyNotInConfiguration.FormatWith(key)
            );
         }

         return item;
      }

      private class BehaviorChainItemConfiguration {
         public BehaviorChainItemConfiguration(BehaviorKey key) {
            Key = key;
            IsEnabled = false;
         }

         public BehaviorKey Key { get; private set; }
         public bool IsEnabled { get; set; }
         public IBehavior Instance { get; set; }

         [ContractInvariantMethod]
         void ObjectInvariant() {
            Contract.Invariant(IsEnabled ? Instance != null : true);
         }

         public override string ToString() {
            return Key.ToString();
         }
      }
   }
}
