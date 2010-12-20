namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class VMBehaviorBuilder<TDescriptor> :
      ConfigurationProvider,
      IVMBehaviorBuilder<TDescriptor> {

      private TDescriptor _descriptor;

      public VMBehaviorBuilder(
         VMDescriptorConfiguration configuration,
         TDescriptor descriptor
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
         _descriptor = descriptor;
      }

      public ISinglePropertyBehaviorBuilder<TValue> For<TValue>(Func<TDescriptor, IVMProperty<TValue>> propertySelector) {
         var property = propertySelector(_descriptor);
         var propertyConfiguration = Configuration.PropertyConfigurations[property];
         return new SinglePropertyBehaviorBuilder<TValue>(propertyConfiguration);
      }

      /// <inheritdoc />
      private class SinglePropertyBehaviorBuilder<TValue> : ISinglePropertyBehaviorBuilder<TValue> {
         private BehaviorChainConfiguration _propertyConfiguration;

         public SinglePropertyBehaviorBuilder(BehaviorChainConfiguration propertyConfiguration) {
            Contract.Requires(propertyConfiguration != null);
            _propertyConfiguration = propertyConfiguration;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TValue> ISinglePropertyBehaviorBuilder<TValue>.Enable(
            BehaviorKey key,
            IBehavior behaviorInstance
         ) {
            _propertyConfiguration.Enable(key, behaviorInstance);
            return this;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TValue> ISinglePropertyBehaviorBuilder<TValue>.Configure<TBehavior>(
            BehaviorKey key,
            Action<TBehavior> configurationAction
         ) {
            _propertyConfiguration.ConfigureBehavior<TBehavior>(key, configurationAction);
            return this;
         }
      }
   }
}
