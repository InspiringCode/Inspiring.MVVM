﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public class ValidatorBuilder<TOwnerVM, TTarget, TDescriptor>
      where TOwnerVM : IViewModel
      where TTarget : IViewModel
      where TDescriptor : class, IVMDescriptor {

      private readonly TDescriptor _descriptor;

      internal ValidatorBuilder(
         IValidatorBuilderOperationProvider operationProvider,
         TDescriptor descriptor
      ) {
         OperationProvider = operationProvider;
         _descriptor = descriptor;
      }

      internal IValidatorBuilderOperationProvider OperationProvider { get; private set; }

      public ValidatorBuilder<TOwnerVM, TTarget, TDescriptor> When(
         Func<ValidatorConditionArgs<TOwnerVM, TTarget>, bool> predicate
      ) {
         var op = OperationProvider.GetOperation();

         // Depending on the position where 'When' is called, we have different 
         // target VMs:
         //   (a) b.When(x => true)
         //          .ValidateDescendant(x => x.Customer)
         //          .Check(x => x.Name)
         //          .HasValue();
         //   (b) b.ValidateDescendant(x => x.Customer)
         //          .When(x => true)
         //          .Check(x => x.Name)
         //          .HasValue();
         // In case (a) x.Target is the ParentVM and in case (b) it is the 
         // CustomerVM.
         int pathTargetIndex = op.Path.Length;

         var condition = new DelegateValidatorCondition<TOwnerVM, TTarget>(
            predicate,
            pathTargetIndex
         );

         op.PushConditionBuildAction(
            BuildActionOrder.UserConditions,
            condition
         );

         return new ValidatorBuilder<TOwnerVM, TTarget, TDescriptor>(op, _descriptor);
      }

      /// <summary>
      ///   Defines a custom validator that is executed every time the VM or 
      ///   any descendant VM has changed.
      /// </summary>
      public void CheckViewModel(Action<ViewModelValidationArgs<TOwnerVM, TTarget>> validatorAction) {
         var op = OperationProvider.GetOperation();

         if (!PathSelectsDescendant(op.Path)) {
            op.EnableViewModelValidationSourceBehavior();
         }

         op.PushValidatorBuildActions(
            DelegateValidator.For(validatorAction),
            ValidationStep.ViewModel
         );
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
         return new PropertyValidatorBuilder<TOwnerVM, TTarget, TValue>(op);
      }

      /// <summary>
      ///   Selects the VM collection for which a validator should be defined.
      /// </summary>
      public CollectionValidatorBuilder<TOwnerVM, TItemVM> CheckCollection<TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> collectionSelector
      ) where TItemVM : IViewModel {
         var op = OperationProvider.GetOperation();

         op.Path = op.Path.AppendCollection(collectionSelector);
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
      ) where TItemDescriptor : IVMDescriptor {
         var op = OperationProvider.GetOperation();

         op.Path = op
            .Path
            .AppendCollection(collectionSelector)
            .AppendCollectionProperty(itemPropertySelector);

         return new CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, TValue>(op);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined.
      ///   Calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a child VM property.
      /// </param>
      /// <typeparam name="C">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      public ValidatorBuilder<TOwnerVM, IViewModel<C>, C> ValidateDescendant<C>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<C>>> propertySelector
      ) where C : class, IVMDescriptor {
         var op = OperationProvider.GetOperation();
         op.Path = op.Path.Append(propertySelector);

         return new ValidatorBuilder<TOwnerVM, IViewModel<C>, C>(op, null);
      }

      /// <summary>
      ///   Selects the child VM for which a validator should be defined.
      ///   Calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="propertySelector">
      ///   The given function should return a collection VM property.
      /// </param>
      /// <typeparam name="C">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      public ValidatorBuilder<TOwnerVM, IViewModel<C>, C> ValidateDescendant<C>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<C>>>> propertySelector
      ) where C : class, IVMDescriptor {
         var op = OperationProvider.GetOperation();
         op.Path = op.Path.Append(propertySelector);

         return new ValidatorBuilder<TOwnerVM, IViewModel<C>, C>(op, null);
      }

      /// <summary>
      ///   If a descendant is validated, it has to enable the appropriate behaviors 
      ///   itself, because we can only modify our own descriptor (the user has to 
      ///   call <see cref="RootValidatorBuilder{TOwner, TTarget, TDescriptor}.EnableParentValidation"/> manually). 
      /// </summary>
      private static bool PathSelectsDescendant(PathDefinition descendantsPath) {
         return !descendantsPath.IsEmpty;
      }
   }
}
