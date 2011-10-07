namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IDependencyActionBuilder<TRootVM, TRootDescriptor>
      where TRootVM : IViewModel
      where TRootDescriptor : IVMDescriptor {

      IRefreshTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor> Refresh { get; }
      IDependencyTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor> Revalidate { get; }
      void Execute(Action<TRootVM, ChangeArgs> changeAction);
   }
}