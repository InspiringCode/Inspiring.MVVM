namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   /// <summary>
   ///   Holds the <see cref="BehaviorChainConfiguration"/> for all VM properties
   ///   of a VM descriptor.
   /// </summary>
   public sealed class BehaviorChainConfigurationCollection {
      private readonly Dictionary<IVMPropertyDescriptor, BehaviorChainConfiguration> _propertyConfigurations
         = new Dictionary<IVMPropertyDescriptor, BehaviorChainConfiguration>();

      /// <summary>
      ///   Gets the <see cref="BehaviorChainConfiguration"/> for the given 
      ///   <paramref name="forProperty"/>.
      /// </summary>
      /// <exception cref="KeyNotFoundException">
      ///   The collection does not contain a configuration for the given property.
      ///   Make sure <see cref="RegisterProperty"/> was called.
      /// </exception>
      public BehaviorChainConfiguration this[IVMPropertyDescriptor forProperty] {
         get {
            Check.NotNull(forProperty, nameof(forProperty));

            return _propertyConfigurations[forProperty];
         }
      }

      public IEnumerable<IVMPropertyDescriptor> ConfiguredProperties {
         get { return _propertyConfigurations.Keys; }
      }

      /// <summary>
      ///   Registers a <see cref="BehaviorChainConfiguration"/> for a certain
      ///   <paramref name="property"/>.
      /// </summary>
      public void RegisterProperty<TValue>(
         IVMPropertyDescriptor<TValue> property,
         BehaviorChainConfiguration configuration
      ) {
         Check.NotNull(property, nameof(property));
         Check.NotNull(configuration, nameof(configuration));

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
         Check.NotNull(key, nameof(key));
         Check.NotNull(configurationAction, nameof(configurationAction));

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
         Check.NotNull(key, nameof(key));

         foreach (BehaviorChainConfiguration config in _propertyConfigurations.Values) {
            config.Enable(key);
         }
      }

      /// <summary>
      ///   Creates concrete <see cref="BehaviorChain"/> objects for each registered
      ///   <see cref="BehaviorChainConfiguration"/> and assigns it to the <see 
      ///   cref="IVMPropertyDescriptor.Behaviors"/> property of the <see cref="IVMPropertyDescriptor"/>
      ///   object for which the <see cref="BehaviorChainConfiguration"/> was
      ///   registered.
      /// </summary>
      internal void ApplyToProperties(IVMDescriptor parentDescriptor) {
         foreach (var pair in _propertyConfigurations) {
            IVMPropertyDescriptor property = pair.Key;
            BehaviorChainConfiguration config = pair.Value;

            var chain = config.CreateChain();
            chain.Initialize(parentDescriptor, property);
            property.Behaviors = chain;
         }
      }
   }
}
