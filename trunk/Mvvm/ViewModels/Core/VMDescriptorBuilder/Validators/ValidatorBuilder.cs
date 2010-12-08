namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   public sealed class ValidatorBuilder<TVM, TDescriptor> :
      ConfigurationProvider
      where TVM : IViewModel
      where TDescriptor : VMDescriptorBase {

      internal ValidatorBuilder(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      /// <summary>
      ///   Selects the VM property for which a validator should be defined.
      /// </summary>
      public PropertyValidatorBuilder<TVM, TValue> Check<TValue>(
         Func<TDescriptor, IVMProperty<TValue>> propertySelector
      ) {
         return new PropertyValidatorBuilder<TVM, TValue>(Configuration);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="CheckVM"/> calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      public ValidatorBuilder<ViewModel<TNextDescriptor>, TNextDescriptor> CheckVM<TNextDescriptor>(
         Func<TDescriptor, IVMProperty<ViewModel<TNextDescriptor>>> propertySelector
      ) where TNextDescriptor : VMDescriptorBase {
         throw new NotImplementedException();
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMProperty<IVMCollectionExpression<TItemVM>>> collectionSelector
      ) where TItemVM : IViewModel {
         throw new NotImplementedException();
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
         throw new NotImplementedException();
      }
   }
}
