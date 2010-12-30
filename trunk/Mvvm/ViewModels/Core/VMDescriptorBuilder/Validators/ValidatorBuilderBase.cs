namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public class ValidatorBuilderBase<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptorBase {

      internal ValidatorBuilderBase(ValidatorConfiguration configuration) {
         Contract.Requires(configuration != null);
         Configuration = configuration;
      }

      private ValidatorConfiguration Configuration {
         get;
         set;
      }

      /// <summary>
      ///   Selects the VM property for which a validator should be defined.
      /// </summary>
      public PropertyValidatorBuilder<TVM, TValue> Check<TValue>(
         Func<TDescriptor, IVMProperty<TValue>> propertySelector
      ) {
         var config = Configuration.SetTargetProperty(PropertySelector.Create(propertySelector));
         return new PropertyValidatorBuilder<TVM, TValue>(config);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="CheckVM"/> calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a collection VM property.
      /// </param>
      public ValidatorBuilderBase<ViewModel<TChildDescriptor>, TChildDescriptor> CheckVM<TChildDescriptor>(
         Func<TDescriptor, IVMProperty<ViewModel<TChildDescriptor>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         var config = Configuration.ExtendTargetPath(PropertySelector.Create(propertySelector));
         return new ValidatorBuilderBase<ViewModel<TChildDescriptor>, TChildDescriptor>(config);
      }

      /// <summary>
      ///   Selects a child collection for whose items a validator should be 
      ///   defined. <see cref="CheckVMs"/> may be freely intermixed with <see
      ///   cref="CheckVM"/> calls to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      public ValidatorBuilderBase<ViewModel<TChildDescriptor>, TChildDescriptor> CheckVMs<TChildDescriptor>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<ViewModel<TChildDescriptor>>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         var config = Configuration.ExtendTargetPath(PropertySelector.Create(propertySelector));
         return new ValidatorBuilderBase<ViewModel<TChildDescriptor>, TChildDescriptor>(config);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<TItemVM>>> collectionSelector
      ) where TItemVM : IViewModel {
         var config = Configuration.ExtendTargetPath(PropertySelector.Create(collectionSelector));
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
      public CollectionPropertyValidatorBuilder<TItemDescriptor, TItemValue> CheckCollection<TItemDescriptor, TItemValue>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<ViewModel<TItemDescriptor>>>> collectionSelector,
         Func<TItemDescriptor, IVMProperty<TItemValue>> itemPropertySelector
      ) where TItemDescriptor : VMDescriptorBase {
         var config = Configuration
            .ExtendTargetPath(PropertySelector.Create(collectionSelector))
            .SetTargetProperty(PropertySelector.Create(itemPropertySelector));

         return new CollectionPropertyValidatorBuilder<TItemDescriptor, TItemValue>(config);
      }
   }
}