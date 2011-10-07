namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public interface IRefreshTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      IDependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      IDependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> AndExecuteRefreshDependencies { get; }
   }
}
