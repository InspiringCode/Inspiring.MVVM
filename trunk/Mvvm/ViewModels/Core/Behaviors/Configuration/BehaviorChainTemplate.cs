namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public enum DefaultBehaviorState {
      Disabled,
      Enabled,
      DisabledWithoutFactory
   }

   /// <summary>
   ///   A template that defines the ordering of <see cref="IBehavior"/> objects
   ///   in a <see cref="BehaviorChain"/>. It also defines how the behavior are
   ///   created and whether the are enabled by default.
   /// </summary>
   /// <remarks>
   ///   A template does not create a <see cref="BehaviorChain"/> directly. Create
   ///   <see cref="BehaviorChainConfiguration"/> by calling <see 
   ///   cref="CreateConfiguration"/> first to create a concrete chain.
   /// </remarks>
   public sealed class BehaviorChainTemplate {
      private readonly List<BehaviorChainItemTemplate> _itemTemplates;
      private readonly IBehaviorFactory _behaviorFactory;

      public BehaviorChainTemplate(IBehaviorFactory behaviorFactory)
         : this(behaviorFactory, new List<BehaviorChainItemTemplate>()) {
      }

      private BehaviorChainTemplate(
         IBehaviorFactory behaviorFactory,
         List<BehaviorChainItemTemplate> itemTemplates
      ) {
         _behaviorFactory = behaviorFactory;
         _itemTemplates = itemTemplates;
      }

      /// <summary>
      ///   Returns a new <see cref="BehaviorChainTemplate"/> with a behavior
      ///   chain item template added to the end of the item template list of 
      ///   this behavior chain template.
      /// </summary>
      /// <param name="disabled">
      ///   If true, the behavior is not enabled in a <see cref="BehaviorChainConfiguration"/> 
      ///   created from this template. This means it will not be included in the finally 
      ///   created <see cref="BehaviorChain"/> unless <see cref="BehaviorChainConfiguration.Enable"/>
      ///   is called.
      /// </param>
      public BehaviorChainTemplate Append(
         BehaviorKey key,
         DefaultBehaviorState state = DefaultBehaviorState.Enabled
      ) {
         Contract.Requires<ArgumentNullException>(key != null);

         var itemTemplatesClone = new List<BehaviorChainItemTemplate>(_itemTemplates);
         itemTemplatesClone.Add(new BehaviorChainItemTemplate(key, state));

         return new BehaviorChainTemplate(_behaviorFactory, itemTemplatesClone);
      }

      /// <summary>
      ///   Returns a new <see cref="BehaviorChainTemplate"/> with the same
      ///   behavior chain item templates but with a different <see 
      ///   cref="IBehaviorFactory"/>.
      /// </summary>
      public BehaviorChainTemplate OverrideFactory(IBehaviorFactory factory) {
         Contract.Requires<ArgumentNullException>(factory != null);

         // The _itemTemplates list can be shared because we do not modify it.
         return new BehaviorChainTemplate(factory, _itemTemplates);
      }

      /// <summary>
      ///   Creates a new <see cref="BehaviorChainConfiguration"/> from this
      ///   template.
      /// </summary>
      // TODO: Comment.
      internal BehaviorChainConfiguration CreateConfiguration(BehaviorFactoryInvoker factoryInvoker) {
         Contract.Requires(factoryInvoker != null);
         Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);

         var config = new BehaviorChainConfiguration();

         foreach (BehaviorChainItemTemplate itemTemplate in _itemTemplates) {
            BehaviorKey key = itemTemplate.Key;

            if (itemTemplate.State == DefaultBehaviorState.DisabledWithoutFactory) {
               config.Append(key);
            } else {
               IBehavior instance = factoryInvoker.Invoke(_behaviorFactory, key);
               config.Append(key, instance);

               if (itemTemplate.State == DefaultBehaviorState.Enabled) {
                  config.Enable(key);
               }
            }
         }

         return config;
      }

      private class BehaviorChainItemTemplate {
         private readonly BehaviorKey _key;
         private readonly DefaultBehaviorState _state;

         public BehaviorChainItemTemplate(BehaviorKey key, DefaultBehaviorState state) {
            _key = key;
            _state = state;
         }

         public BehaviorKey Key {
            get { return _key; }
         }

         public DefaultBehaviorState State {
            get { return _state; }
         }

         public override string ToString() {
            return Key.ToString();
         }
      }
   }
}
