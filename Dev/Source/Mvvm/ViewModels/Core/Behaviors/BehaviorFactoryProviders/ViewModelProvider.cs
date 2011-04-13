namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using Inspiring.Mvvm.Common;

   internal sealed class ViewModelProvider : IViewModelProvider {
      public BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
