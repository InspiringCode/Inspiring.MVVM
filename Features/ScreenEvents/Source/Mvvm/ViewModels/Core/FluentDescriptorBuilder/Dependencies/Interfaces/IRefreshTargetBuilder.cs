namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IRefreshTargetBuilder<TSourceDescriptor> :
      IPathDefinitionBuilder<TSourceDescriptor>
      where TSourceDescriptor : IVMDescriptor {

      IRefreshTargetBuilder<TSourceDescriptor> AndExecuteRefreshDependencies { get; }

      void Self();
   }
}
