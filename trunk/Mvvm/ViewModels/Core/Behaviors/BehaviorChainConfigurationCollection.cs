namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   Holds the <see cref="BehaviorChainConfiguration"/> for all VM properties
   ///   of a VM descriptor.
   /// </summary>
   public sealed class BehaviorChainConfigurationCollection {
      private readonly Dictionary<IVMProperty, BehaviorChainConfiguration> _propertyConfigurations
         = new Dictionary<IVMProperty, BehaviorChainConfiguration>();

      /// <summary>
      ///   Gets the <see cref="BehaviorChainConfiguration"/> for the given 
      ///   <paramref name="forProperty"/>.
      /// </summary>
      /// <exception cref="KeyNotFoundException">
      ///   The collection does not contain a configuration for the given property.
      ///   Make sure <see cref="RegisterProperty"/> was called.
      /// </exception>
      public BehaviorChainConfiguration this[IVMProperty forProperty] {
         get {
            Contract.Requires<ArgumentNullException>(forProperty != null);
            Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);

            return _propertyConfigurations[forProperty];
         }
      }

      /// <summary>
      ///   Registers a <see cref="BehaviorChainConfiguration"/> for a certain
      ///   <paramref name="property"/>.
      /// </summary>
      public void RegisterProperty<TValue>(
         VMPropertyBase<TValue> property,
         BehaviorChainConfiguration configuration
      ) {
         Contract.Requires<ArgumentNullException>(property != null);
         Contract.Requires<ArgumentNullException>(configuration != null);

         _propertyConfigurations.Add(property, configuration);
      }

      /// <summary>
      ///   Calls <see cref="BehaviorChainConfiguration.ConfigureBehavior"/> on
      ///   all configurations contained by this collection that contain the 
      ///   specified '<paramref name="key"/>'.
      /// </summary>
      public void ConfigureBehavior<T>(
         BehaviorKey key,
         Action<T> configurationAction
      ) where T : IBehavior {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(configurationAction != null);

         foreach (BehaviorChainConfiguration config in _propertyConfigurations.Values) {
            config.ConfigureBehavior(key, configurationAction);
         }
      }

      /// <summary>
      ///   Calls <see cref="BehaviorChainConfiguration.Enable(BehaviorKey)"/> on
      ///   all configurations contained by this collection that contain the 
      ///   specified '<paramref name="key"/>'.
      /// </summary>
      public void Enable(BehaviorKey key) {
         Contract.Requires<ArgumentNullException>(key != null);

         foreach (BehaviorChainConfiguration config in _propertyConfigurations.Values) {
            config.Enable(key);
         }
      }

      /// <summary>
      ///   Creates concrete <see cref="BehaviorChain"/> objects for each registered
      ///   <see cref="BehaviorChainConfiguration"/> and assigns it to the <see 
      ///   cref="VMPropertyBase.Behaviors"/> property of the <see cref="VMPropertyBase"/>
      ///   object for which the <see cref="BehaviorChainConfiguration"/> was
      ///   registered.
      /// </summary>
      internal void ApplyToProperties(VMDescriptorBase parentDescriptor) {
         foreach (var pair in _propertyConfigurations) {
            IVMProperty property = pair.Key;
            BehaviorChainConfiguration config = pair.Value;

            var chain = config.CreateChain();
            chain.Initialize(parentDescriptor, property);
            property.Behaviors = chain;
         }
      }
   }
}
