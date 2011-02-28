namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

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
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) {
         var config = Configuration.SetTargetProperty(PropertySelector.CreateExactlyTyped(propertySelector));
         return new PropertyValidatorBuilder<TVM, TValue>(config);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="ValidateDescendant"/> calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a collection VM property.
      /// </param>
      public ValidatorBuilderBase<IViewModel<TChildDescriptor>, TChildDescriptor> ValidateDescendant<TChildDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         var config = Configuration.ExtendTargetPath(PropertySelector.Create(propertySelector));
         return new ValidatorBuilderBase<IViewModel<TChildDescriptor>, TChildDescriptor>(config);
      }

      /// <summary>
      ///   Selects a child collection for whose items a validator should be 
      ///   defined. <see cref="ValidateDescendant"/> may be freely intermixed with <see
      ///   cref="ValidateDescendant"/> calls to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      public ValidatorBuilderBase<IViewModel<TChildDescriptor>, TChildDescriptor> ValidateDescendant<TChildDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModel<TChildDescriptor>>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         var config = Configuration.ExtendTargetPath(PropertySelector.Create(propertySelector));
         return new ValidatorBuilderBase<IViewModel<TChildDescriptor>, TChildDescriptor>(config);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> collectionSelector
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
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>> collectionSelector,
         Func<TItemDescriptor, IVMPropertyDescriptor<TItemValue>> itemPropertySelector
      ) where TItemDescriptor : VMDescriptorBase {
         var config = Configuration
            .ExtendTargetPath(PropertySelector.Create(collectionSelector))
            .SetTargetProperty(PropertySelector.CreateExactlyTyped(itemPropertySelector));

         return new CollectionPropertyValidatorBuilder<TItemDescriptor, TItemValue>(config);
      }

      /// <summary>
      ///   Defines a custom validator that is executed every time the VM or 
      ///   any descendant VM changes.
      /// </summary>
      public void CheckViewModel(Action<TVM, ValidationArgs> validator) {
         Configuration.AddViewModelValidator(new DelegateValidator(validator));
      }

      private sealed class DelegateValidator : Validator {
         private Action<TVM, ValidationArgs> _validatorCallback;

         public DelegateValidator(Action<TVM, ValidationArgs> validatorCallback) {
            Contract.Requires(validatorCallback != null);
            _validatorCallback = validatorCallback;
         }

         public override void ValidateCore(ValidationArgs args) {
            Contract.Assert(args.TargetProperty == null);

            _validatorCallback((TVM)args.TargetVM, args);
         }

         public override string ToString() {
            return String.Format(
               "{{DelegateValidator: {0}}}",
               DelegateUtils.GetFriendlyName(_validatorCallback)
            );
         }
      }
   }
}