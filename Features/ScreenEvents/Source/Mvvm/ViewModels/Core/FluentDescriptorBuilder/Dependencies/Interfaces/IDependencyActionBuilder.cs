namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IDependencyActionBuilder<TRootVM, TRootDescriptor>
      where TRootVM : IViewModel
      where TRootDescriptor : IVMDescriptor {

      IRefreshTargetBuilder<TRootDescriptor> Refresh { get; }
      IPathDefinitionBuilder<TRootDescriptor> Revalidate { get; }
      void Execute(Action<TRootVM, ChangeArgs> changeAction);
   }
}