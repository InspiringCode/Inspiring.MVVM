namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   internal class ValidatorBuilder<TOwner, TTarget, TDescriptor>
      where TOwner : IViewModel
      where TTarget : IViewModel
      where TDescriptor : VMDescriptorBase {

      private readonly TDescriptor _descriptor;

      public ValidatorBuilder(
         IValidatorBuilderOperationProvider operationProvider,
         TDescriptor descriptor
      ) {
         OperationProvider = operationProvider;
         _descriptor = descriptor;
      }

      internal IValidatorBuilderOperationProvider OperationProvider { get; private set; }

      /// <summary>
      ///   Defines a custom validator that is executed every time the VM or 
      ///   any descendant VM changes.
      /// </summary>
      public void CheckViewModel(Action<ViewModelValidationArgs<TOwner, TTarget>> validatorAction) {
         var op = OperationProvider.GetOperation();

         if (op.Path.IsEmpty) {
            // This is the first path step: A custom validation is defined for 
            // OUR view model (with the descriptor that we are building right 
            // now) and NOT for a descendant.
            // 
            // If a descendant is validated, it has to enable the appropriate
            // behaviors itself, because we can only modify our own descriptor
            // (the user has to call 'EnableParentValidation' manually).  
            op.EnableViewModelValidationSourceBehavior();
         }

         op.BuildActions.Push(() => {
            op.ActionArgs.Push(
               new ConditionalValidator(
                  new ValidationTargetCondition(op.Path),
                  new ConditionalValidator(
                     new ValidationStepCondition(ValidationStep.ViewModel),
                     DelegateValidator.Create(ViewModelValidationArgs<TOwner, TTarget>.Create, validatorAction)
                  )
               )
            );
         });

         throw new NotImplementedException();
      }

      /// <summary>
      ///   Selects the VM property for which a validator should be defined.
      /// </summary>
      public PropertyValidatorBuilder<TTarget, TValue> Check<TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) {
         var op = OperationProvider.GetOperation();

         if (op.Path.IsEmpty) {
            // This is the first path step: A custom validation is defined for 
            // a property of OUR view model (with the descriptor that we are 
            // building right now) and NOT for a descendant.
            // 
            // If a descendant is validated, it has to enable the appropriate
            // behaviors itself, because we can only modify our own descriptor
            // (the user has to call 'EnableParentValidation' manually).

            IVMPropertyDescriptor p = propertySelector(_descriptor);
            op.EnablePropertyValidationSourceBehavior(p);
         }

         op.Path = op.Path.Append(propertySelector);

         op.BuildActions.Push(() => {
            var inner = op.ActionArgs.Pop();

            op.ActionArgs.Push(
               new ConditionalValidator(new ValidationTargetCondition(op.Path), inner)
            );
         });

         throw new NotImplementedException();
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> collectionSelector
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
      public CollectionValidatorBuilder<TItemDescriptor, TItemValue> CheckCollection<TItemDescriptor, TItemValue>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>> collectionSelector,
         Func<TItemDescriptor, IVMPropertyDescriptor<TItemValue>> itemPropertySelector
      ) where TItemDescriptor : VMDescriptorBase {
         throw new NotImplementedException();
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="ValidateDescendant"/> calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a collection VM property.
      /// </param>
      public ValidatorBuilder<IViewModel<TChildDescriptor>, TChildDescriptor> ValidateDescendant<TChildDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         throw new NotImplementedException();
      }

      /// <summary>
      ///   Selects a child collection for whose items a validator should be 
      ///   defined. <see cref="ValidateDescendant"/> may be freely intermixed with <see
      ///   cref="ValidateDescendant"/> calls to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      public ValidatorBuilder<IViewModel<TChildDescriptor>, TChildDescriptor> ValidateDescendant<TChildDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModel<TChildDescriptor>>>> propertySelector
      ) where TChildDescriptor : VMDescriptorBase {
         throw new NotImplementedException();
      }
   }
}
