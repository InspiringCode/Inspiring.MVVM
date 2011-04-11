namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class RootValidatorBuilder<TOwner, TTarget, TDescriptor> :
      ValidatorBuilder<TOwner, TTarget, TDescriptor>
      where TOwner : IViewModel
      where TTarget : IViewModel
      where TDescriptor : VMDescriptorBase {

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
      ///   Allows ancestor VMs to define validators for this VM.
      /// </summary>
      public void EnableParentValidation<TValue>(Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         Contract.Requires<ArgumentNullException>(propertySelector != null);

         var op = OperationProvider.GetOperation();
         var property = propertySelector((TDescriptor)op.Descriptor);

         op.EnablePropertyValidationSourceBehavior(property);
         op.EnableViewModelValidationSourceBehavior();
      }

      public void Execute() {
         _operations.Perform();
      }
   }
}
