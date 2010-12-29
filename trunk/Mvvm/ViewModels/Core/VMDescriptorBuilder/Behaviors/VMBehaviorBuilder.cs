namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

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

      public ISinglePropertyBehaviorBuilder<TVM, TValue> For<TValue>(Func<TDescriptor, IVMProperty<TValue>> propertySelector) {
         var property = propertySelector(_descriptor);
         var propertyConfiguration = Configuration.PropertyConfigurations[property];
         return new SinglePropertyBehaviorBuilder<TValue>(Configuration, propertyConfiguration);
      }

      /// <inheritdoc />
      private class SinglePropertyBehaviorBuilder<TValue> : ConfigurationProvider, ISinglePropertyBehaviorBuilder<TVM, TValue> {
         private BehaviorChainConfiguration _propertyConfiguration;

         public SinglePropertyBehaviorBuilder(VMDescriptorConfiguration configuration, BehaviorChainConfiguration propertyConfiguration)
            : base(configuration) {
            Contract.Requires(propertyConfiguration != null);
            _propertyConfiguration = propertyConfiguration;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TValue> ISinglePropertyBehaviorBuilder<TVM, TValue>.CollectionBehaviors {
            get {
               // TODO: Refactor this?
               BehaviorChainConfiguration collectionConfiguration = _propertyConfiguration
                  .GetBehavior<ICollectionBehaviorConfigurationBehavior>(BehaviorKeys.CollectionFactory)
                  .CollectionBehaviorConfiguration;

               return new SinglePropertyBehaviorBuilder<TValue>(Configuration, collectionConfiguration);
            }
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TValue> ISinglePropertyBehaviorBuilder<TVM, TValue>.Enable(
            BehaviorKey key,
            IBehavior behaviorInstance
         ) {
            _propertyConfiguration.Enable(key, behaviorInstance);
            return this;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TValue> ISinglePropertyBehaviorBuilder<TVM, TValue>.Configure<TBehavior>(
            BehaviorKey key,
            Action<TBehavior> configurationAction
         ) {
            _propertyConfiguration.ConfigureBehavior<TBehavior>(key, configurationAction);
            return this;
         }

         /// <inheritdoc />
         ISinglePropertyBehaviorBuilder<TVM, TValue> ISinglePropertyBehaviorBuilder<TVM, TValue>.AddChangeHandler(Action<TVM, ChangeArgs, InstancePath> changeHandler) {
            // TODO: Make this more official...
            var key = new BehaviorKey("ChangeListener");
            Configuration.ViewModelConfiguration.Append(key);
            Configuration.ViewModelConfiguration.Enable(
               key,
               new ChangeListenerBehavior<TVM>(changeHandler)
            );

            return this;
         }
      }
   }
}
