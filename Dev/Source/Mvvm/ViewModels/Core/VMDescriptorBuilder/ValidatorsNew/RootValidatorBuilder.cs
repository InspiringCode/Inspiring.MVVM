namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {

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
   }
}
