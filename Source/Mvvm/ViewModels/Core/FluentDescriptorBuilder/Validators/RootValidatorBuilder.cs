namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public sealed class RootValidatorBuilder<TOwner, TTarget, TDescriptor> :
      ValidatorBuilder<TOwner, TTarget, TDescriptor>
      where TOwner : IViewModel
      where TTarget : IViewModel
      where TDescriptor : class, IVMDescriptor {

      private ValidatorBuilderOperationCollection _operations;

      public RootValidatorBuilder(VMDescriptorConfiguration config, TDescriptor descriptor)
         : this(new ValidatorBuilderOperationCollection(descriptor, config), descriptor) {
      }

      private RootValidatorBuilder(
         ValidatorBuilderOperationCollection operations,
         TDescriptor descriptor
      )
         : base(operations, descriptor) {
         _operations = operations;
      }

      /// <summary>
      ///   Allows ancestor VMs to define property validators for this VM.
      /// </summary>
      public void EnableParentValidation<TValue>(Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         Inspiring.Mvvm.Check.NotNull(propertySelector, nameof(propertySelector));

         var op = OperationProvider.GetOperation();
         var property = propertySelector((TDescriptor)op.Descriptor);

         op.EnablePropertyValidationSourceBehavior(property);
         op.EnableViewModelValidationSourceBehavior();
      }

      /// <summary>
      ///   Allows ancestor VMs to define view model-level validators for this VM.
      /// </summary>
      public void EnableParentViewModelValidation() {
         var op = OperationProvider.GetOperation();
         op.EnableViewModelValidationSourceBehavior();
      }

      public void Execute() {
         _operations.Perform();
      }
   }
}
