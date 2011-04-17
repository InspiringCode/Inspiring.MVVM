namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class ValidatorBuilder<TOwnerVM, TTarget, TDescriptor>
      where TOwnerVM : IViewModel
      where TTarget : IViewModel
      where TDescriptor : VMDescriptorBase {

      private readonly TDescriptor _descriptor;

      internal ValidatorBuilder(
         IValidatorBuilderOperationProvider operationProvider,
         TDescriptor descriptor
      ) {
         OperationProvider = operationProvider;
         _descriptor = descriptor;
      }

      internal IValidatorBuilderOperationProvider OperationProvider { get; private set; }

      /// <summary>
      ///   Defines a custom validator that is executed every time the VM or 
      ///   any descendant VM has changed.
      /// </summary>
      public void CheckViewModel(Action<ViewModelValidationArgs<TOwnerVM, TTarget>> validatorAction) {
         var op = OperationProvider.GetOperation();

         if (!PathSelectsDescendant(op.Path)) {
            op.EnableViewModelValidationSourceBehavior();
         }

         var val = new ConditionalValidator(
            new ValidationTargetCondition(op.Path),
            new ConditionalValidator(
               new ValidationStepCondition(ValidationStep.ViewModel),
               DelegateValidator.For(validatorAction)
            )
         );

         op.BuildActions.Push(() => {
            op.ActionArgs.Push(val);
         });
      }

      /// <summary>
      ///   Selects the VM property for which a validator should be defined.
      /// </summary>
      public PropertyValidatorBuilder<TOwnerVM, TTarget, TValue> Check<TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) {
         var op = OperationProvider.GetOperation();

         if (!PathSelectsDescendant(op.Path)) {
            IVMPropertyDescriptor p = propertySelector(_descriptor);
            op.EnablePropertyValidationSourceBehavior(p);
         }

         op.Path = op.Path.Append(propertySelector);

         var targetPathCondition = new ValidationTargetCondition(op.Path);

         op.BuildActions.Push(() => {
            var inner = op.ActionArgs.Pop();

            op.ActionArgs.Push(
               new ConditionalValidator(targetPathCondition, inner)
            );
         });

         return new PropertyValidatorBuilder<TOwnerVM, TTarget, TValue>(op);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TOwnerVM, TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> collectionSelector
      ) where TItemVM : IViewModel {
         var op = OperationProvider.GetOperation();

         op.Path = op.Path.Append(collectionSelector);

         var targetPathCondition = new ValidationTargetCondition(op.Path);

         op.BuildActions.Push(() => {
            var inner = op.ActionArgs.Pop();

            op.ActionArgs.Push(
               new ConditionalValidator(targetPathCondition, inner)
            );
         });

         return new CollectionValidatorBuilder<TOwnerVM, TItemVM>(op);
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
      public CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, TValue> CheckCollection<TItemDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>> collectionSelector,
         Func<TItemDescriptor, IVMPropertyDescriptor<TValue>> itemPropertySelector
      ) where TItemDescriptor : VMDescriptorBase {
         var op = OperationProvider.GetOperation();

         op.Path = op
            .Path
            .Append(collectionSelector)
            .AppendCollectionProperty(itemPropertySelector);

         var targetPathCondition = new ValidationTargetCondition(op.Path);

         op.BuildActions.Push(() => {
            var inner = op.ActionArgs.Pop();

            op.ActionArgs.Push(
               new ConditionalValidator(targetPathCondition, inner)
            );
         });

         return new CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, TValue>(op);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="ValidateDescendant"/> calls may be chained to select any 
      ///   descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      /// <typeparam name="C">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      public ValidatorBuilder<TOwnerVM, IViewModel<C>, C> ValidateDescendant<C>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<C>>> propertySelector
      ) where C : VMDescriptorBase {
         var op = OperationProvider.GetOperation();
         op.Path = op.Path.Append(propertySelector);

         return new ValidatorBuilder<TOwnerVM, IViewModel<C>, C>(op, null);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined. <see 
      ///   cref="ValidateDescendant"/> calls may be chained to select any 
      ///   descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a collection VM property.
      /// </param>
      /// <typeparam name="C">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      public ValidatorBuilder<TOwnerVM, IViewModel<C>, C> ValidateDescendant<C>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<C>>>> propertySelector
      ) where C : VMDescriptorBase {
         var op = OperationProvider.GetOperation();
         op.Path = op.Path.Append(propertySelector);

         return new ValidatorBuilder<TOwnerVM, IViewModel<C>, C>(op, null);
      }

      /// <summary>
      ///   If a descendant is validated, it has to enable the appropriate behaviors 
      ///   itself, because we can only modify our own descriptor (the user has to 
      ///   call <see cref="EnableParentValidation"/> manually). 
      /// </summary>
      private static bool PathSelectsDescendant(PathDefinition descendantsPath) {
         return !descendantsPath.IsEmpty;
      }
   }
}
