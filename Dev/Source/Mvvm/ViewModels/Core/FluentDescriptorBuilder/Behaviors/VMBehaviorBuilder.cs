namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal sealed class VMBehaviorBuilder<TVM, TDescriptor> :
      ConfigurationProvider,
      IVMBehaviorBuilder<TVM, TDescriptor> {

      private TDescriptor _descriptor;

      public VMBehaviorBuilder(
         VMDescriptorConfiguration configuration,
         TDescriptor descriptor
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
         _descriptor = descriptor;
      }

      public ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> Property<TValue>(Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         var property = propertySelector(_descriptor);
         var propertyConfiguration = Configuration.PropertyConfigurations[property];
         return new SinglePropertyBehaviorBuilder<TValue>(Configuration, propertyConfiguration, _descriptor);
      }

      public void AddBehavior(IBehavior behaviorInstance, BehaviorKey key = null) {
         string keyString = String.Format(
              "{0} (manually configured)",
              TypeService.GetFriendlyTypeName(behaviorInstance)
           );

         key = key ?? new BehaviorKey(keyString);

         Configuration.ViewModelConfiguration.Append(key, behaviorInstance);
         Configuration.ViewModelConfiguration.Enable(key, behaviorInstance);
      }

      /// <inheritdoc />
      private class SinglePropertyBehaviorBuilder<TValue> : ConfigurationProvider, ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> {
         private readonly BehaviorChainConfiguration _propertyConfiguration;
         private readonly TDescriptor _descriptor;

         public SinglePropertyBehaviorBuilder(VMDescriptorConfiguration configuration, BehaviorChainConfiguration propertyConfiguration, TDescriptor descriptor)
            : base(configuration) {
            Contract.Requires(propertyConfiguration != null);
            _propertyConfiguration = propertyConfiguration;
            _descriptor = descriptor;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue>.Enable(
            BehaviorKey key,
            IBehavior behaviorInstance
         ) {
            _propertyConfiguration.Enable(key, behaviorInstance);
            return this;
         }

         ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue>.AddBehavior(
            IBehavior behaviorInstance,
            BehaviorKey key = null
         ) {
            string keyString = String.Format(
               "{0} (manually configured)",
               TypeService.GetFriendlyTypeName(behaviorInstance)
            );

            key = key ?? new BehaviorKey(keyString);

            _propertyConfiguration.Append(key, behaviorInstance);
            _propertyConfiguration.Enable(key, behaviorInstance);
            return this;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue>.Configure<TBehavior>(
            BehaviorKey key,
            Action<TBehavior> configurationAction
         ) {
            _propertyConfiguration.ConfigureBehavior<TBehavior>(key, configurationAction);
            return this;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue>.AddChangeHandler(Action<TVM, ChangeArgs> changeHandler) {
            // TODO: Make this more official...
            var key = new BehaviorKey("ChangeListener");
            Configuration.ViewModelConfiguration.Append(key);
            Configuration.ViewModelConfiguration.Enable(
               key,
               new ChangeListenerBehavior<TVM>(changeHandler)
            );

            return this;
         }

         ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue>.RequiresLoadedProperty(
            Func<TDescriptor, IVMPropertyDescriptor> requiredPropertySelector
         ) {
            IVMPropertyDescriptor requiredProperty = requiredPropertySelector(_descriptor);

            _propertyConfiguration
               .ConfigureBehavior<PropertyPreloaderBehavior<TValue>>(
                  PropertyBehaviorKeys.PropertyPreloader,
                  x => x.PreloadedProperties.Add(requiredProperty)
               );

            return this;
         }
      }
   }
}
