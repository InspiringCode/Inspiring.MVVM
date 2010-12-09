namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   public abstract class ValidatorBuilderBase<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptorBase {

      /// <summary>
      ///   Selects the VM property for which a validator should be defined.
      /// </summary>
      public PropertyValidatorBuilder<TVM, TValue> Check<TValue>(
         Func<TDescriptor, IVMProperty<TValue>> propertySelector
      ) {
         var config = BeginConfiguration(propertySelector);
         return new PropertyValidatorBuilder<TVM, TValue>(config);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="CheckVM"/> calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      public ValidatorBuilderBase<ViewModel<TChildDescriptor>, TChildDescriptor> CheckVM<TChildDescriptor>(
         Func<TDescriptor, IVMProperty<ViewModel<TChildDescriptor>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         var config = BeginConfiguration(propertySelector);
         return new ChildValidatorBuilder<ViewModel<TChildDescriptor>, TChildDescriptor>(config);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<TItemVM>>> collectionSelector
      ) where TItemVM : IViewModel {
         var config = BeginConfiguration(collectionSelector);
         return new CollectionValidatorBuilder<TItemVM>(config);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      /// <param name="itemPropertySelector">
      ///   <para>The given function should return a VM property of a collection 
      ///      item.</para>
      ///   <para>This overrload is used to validate a single property of the
      ///      items of a collection (for example that the 'Name' property of 
      ///      the collection is unique).</para>
      /// </param>
      public CollectionPropertyValidatorBuilder<TItemValue> CheckCollection<TItemDescriptor, TItemValue>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<ViewModel<TItemDescriptor>>>> collectionSelector,
         Func<TItemDescriptor, IVMProperty<TItemValue>> itemPropertySelector
      ) where TItemDescriptor : VMDescriptorBase {
         var config = BeginConfiguration(collectionSelector);
         return new CollectionPropertyValidatorBuilder<TItemValue>(config);
      }

      protected abstract ViewModelValidationBehavior BeginConfiguration(
         Func<TDescriptor, IVMProperty> propertySelector
      );
   }

   public sealed class ValidatorBuilder<TVM, TDescriptor> :
      ValidatorBuilderBase<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptorBase {

      private TDescriptor _descriptor;
      private VMDescriptorConfiguration _configuration;

      internal ValidatorBuilder(VMDescriptorConfiguration configuration, TDescriptor descriptor) {
         Contract.Requires(configuration != null);
         Contract.Requires(descriptor != null);

         _configuration = configuration;
         _descriptor = descriptor;
      }

      /// <summary>
      ///   <para>Enables validation for ALL properties of this VM descriptor.</para>
      ///   <para>You ONLY need to call this method if you want to define validators 
      ///      for some properties of this descriptor in an ancestor VM and you
      ///      DO NOT define any validator for that property in the current VM 
      ///      descriptor.</para>
      /// </summary>
      public void EnableParentValidation() {
         _descriptor
            .Properties
            .Where(p => p != null)
            .ForEach(EnableValidation);
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

         var property = propertySelector(_descriptor);
         Contract.Assert(property != null);

         EnableValidation(property);
      }

      /// <inheritdoc />
      protected override ViewModelValidationBehavior BeginConfiguration(
         Func<TDescriptor, IVMProperty> propertySelector
      ) {
         IVMProperty property = propertySelector(_descriptor);

         EnableValidation(property);
         
         return _configuration
            .ViewModelConfiguration
            .GetBehavior<ViewModelValidationBehavior>(BehaviorKeys.Validator);
      }

      /// <summary>
      ///   Enables the validation behaviors for the given property and the
      ///   VM descriptor.
      /// </summary>
      private void EnableValidation(IVMProperty property) {
         _configuration
            .PropertyConfigurations[property]
            .Enable(BehaviorKeys.Validator);
         _configuration
            .ViewModelConfiguration
            .Enable(BehaviorKeys.Validator);
      }
   }

   internal sealed class ChildValidatorBuilder<TVM, TChildDescriptor> :
      ValidatorBuilderBase<TVM, TChildDescriptor>
      where TVM : IViewModel
      where TChildDescriptor : VMDescriptorBase {

      private ViewModelValidationBehavior _validationBehavior;

      public ChildValidatorBuilder(ViewModelValidationBehavior validationBehavior) {
         Contract.Requires(validationBehavior != null);
         _validationBehavior = validationBehavior;
      }

      protected override ViewModelValidationBehavior BeginConfiguration(Func<TChildDescriptor, IVMProperty> propertySelector) {
         return _validationBehavior;
      }
   }
}
