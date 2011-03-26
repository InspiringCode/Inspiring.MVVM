namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class RootValidatorBuilder<TOwner, TTarget, TDescriptor> :
      ValidatorBuilder<TOwner, TTarget, TDescriptor>
      where TOwner : IViewModel
      where TTarget : IViewModel
      where TDescriptor : VMDescriptorBase {

      public RootValidatorBuilder(
         IValidatorBuilderOperationProvider operationProvider,
         TDescriptor descriptor
      )
         : base(operationProvider, descriptor) {
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
   }
}
