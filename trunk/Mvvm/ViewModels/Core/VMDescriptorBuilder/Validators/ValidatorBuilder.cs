namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class ValidatorBuilder<TVM, TDescriptor> :
      ValidatorBuilderBase<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptorBase {

      internal ValidatorBuilder(
         VMDescriptorConfiguration descriptorConfiguration,
         TDescriptor descriptor
      )
         : this(new RootValidatorConfiguration(descriptorConfiguration, descriptor)) {

         Contract.Requires(descriptorConfiguration != null);
         Contract.Requires(descriptor != null);
      }

      private ValidatorBuilder(RootValidatorConfiguration configuration)
         : base(configuration) {
         Configuration = configuration;
      }

      private RootValidatorConfiguration Configuration {
         get;
         set;
      }

      /// <summary>
      ///   <para>Enables validation for ALL properties of this VM descriptor.</para>
      ///   <para>You ONLY need to call this method if you want to define validators 
      ///      for some properties of this descriptor in an ancestor VM and you
      ///      DO NOT define any validator for that property in the current VM 
      ///      descriptor.</para>
      /// </summary>
      public void EnableParentValidation() {
         Configuration.EnableValidationForAllProperties();
      }

      /// <summary>
      ///   <para>Enables validation for the selected property even if no validator
      ///      is defined for it.</para>
      ///   <para>You ONLY need to call this method if you want to define validators 
      ///      for the selected property in an ancestor VM and you DO NOT define 
      ///      any validator for that property in the current VM descriptor.</para>
      /// </summary>
      public void EnableParentValidation(Func<TDescriptor, IVMProperty> propertySelector) {
         Contract.Requires<ArgumentNullException>(propertySelector != null);
         Configuration.EnableValidation(PropertySelector.Create(propertySelector));
      }

      private class RootValidatorConfiguration : ValidatorConfiguration {
         private readonly VMDescriptorConfiguration _descriptorConfiguration;
         private readonly VMDescriptorBase _descriptor;

         internal RootValidatorConfiguration(
            VMDescriptorConfiguration descriptorConfiguration,
            VMDescriptorBase descriptor
         ) {
            Contract.Requires(descriptorConfiguration != null);
            Contract.Requires(descriptor != null);

            _descriptorConfiguration = descriptorConfiguration;
            _descriptor = descriptor;
         }

         protected override ViewModelValidationBehavior ValidationBehavior {
            get {
               return _descriptorConfiguration
                  .ViewModelConfiguration
                  .GetBehavior<ViewModelValidationBehavior>(BehaviorKeys.Validator);
            }
         }

         public override ValidatorConfiguration SetTargetProperty(PropertySelector selector) {
            EnableValidation(selector);
            return base.SetTargetProperty(selector);
         }

         public override ValidatorConfiguration ExtendTargetPath(PropertySelector selector) {
            EnableValidation(selector);
            return base.ExtendTargetPath(selector);
         }

         public void EnableValidationForAllProperties() {
            _descriptor
               .Properties
               .ForEach(EnableValidation);
         }

         public void EnableValidation(PropertySelector forProperty) {
            Contract.Requires(forProperty != null);

            IVMProperty p = forProperty.GetProperty(_descriptor);
            Contract.Assert(p != null);

            EnableValidation(p);
         }

         /// <summary>
         ///   Enables the validation behaviors for the given property and the
         ///   VM descriptor.
         /// </summary>
         private void EnableValidation(IVMProperty forProperty) {
            _descriptorConfiguration
               .ViewModelConfiguration
               .Enable(BehaviorKeys.Validator);

            _descriptorConfiguration
               .PropertyConfigurations[forProperty]
               .Enable(BehaviorKeys.PreValidationValueCache);

            _descriptorConfiguration
               .PropertyConfigurations[forProperty]
               .Enable(BehaviorKeys.Validator);
         }
      }
   }
}
