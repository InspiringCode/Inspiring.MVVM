namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public static class LegacyPropertyFactoryMethodExtensions {
      // TODO: Document me
      public static IVMCollectionPropertyFactoryExpression<TParentVM, TItem> MappedCollection<TParentVM, TItem, TSource>(
         this IVMPropertyFactory<TParentVM, TSource> factory,
         Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector
      ) where TParentVM : IViewModel {
         throw new NotImplementedException();
      }

      public static IVMViewModelPropertyFactoryExpression<TVMSource> MappedVM<TParentVM, TSource, TVMSource>(
         this IVMPropertyFactory<TParentVM, TSource> factory,
         Expression<Func<TSource, TVMSource>> viewModelSourceSelector
      ) where TParentVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
