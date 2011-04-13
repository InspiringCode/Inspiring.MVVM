namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using System;
   using Inspiring.Mvvm.Common;

   public class ViewModelPropertyProvider : IChildProvider {
      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         throw new NotImplementedException();
      }

      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         throw new NotImplementedException();
      }
   }
}
