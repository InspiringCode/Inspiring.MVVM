namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public static class LegacyPropertyFactoryMethodExtensions {
      // TODO: Document me
      public static IVMCollectionPropertyFactoryExpression<TParentVM, TItem> MappedCollection<TParentVM, TItem, TSource>(
         this IVMPropertyFactory<TSource> factory,
         Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector
      ) {
         throw new NotImplementedException();
      }

      public static IVMViewModelPropertyFactoryExpression<TVMSource> MappedVM<TParentVM, TSource, TVMSource>(
         this IVMPropertyFactory<TSource> factory,
         Expression<Func<TSource, TVMSource>> viewModelSourceSelector
      ) {
         throw new NotImplementedException();
      }
   }
}
