namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   public sealed class RevalidationQueue {
      private readonly List<IViewModel> _queue = new List<IViewModel>();

      public void Add(IViewModel vm, bool addIfAlreadyContained = false) {
         if (!addIfAlreadyContained && _queue.Contains(vm)) {
            return;
         }

         _queue.Add(vm);
      }

      internal void Revalidate(ValidationContext validationContext, ValidationMode mode) {
         for (int i = 0; i < _queue.Count; i++) {
            IViewModel vm = _queue[i];
            vm.Kernel.Revalidate(validationContext, ValidationScope.SelfOnly, mode);
         }
      }
   }
}
