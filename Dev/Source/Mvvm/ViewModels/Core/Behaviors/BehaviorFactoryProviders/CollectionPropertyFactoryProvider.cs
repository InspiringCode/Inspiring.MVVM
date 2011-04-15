﻿namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class CollectionPropertyFactoryProvider : PropertyProvider, IChildProvider {
      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel {

         return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
      }

      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
         where TOwnerVM : IViewModel
         where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

         return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>()
            .RegisterBehavior<SynchronizerCollectionBehavior<TChildVM, TChildSource>>(CollectionPropertyBehaviorKeys.Synchronizer)
            .RegisterBehavior<ServiceLocatorValueFactoryBehavior<TChildVM>>(PropertyBehaviorKeys.ValueFactory);
      }

      protected virtual new BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>()
         where TChildVM : IViewModel {

         return base
            .GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>()
            .RegisterBehavior<LazyRefreshBehavior>(PropertyBehaviorKeys.LazyRefresh)
            .RegisterBehavior<UndoCollectionModifcationBehavior<TChildVM>>(PropertyBehaviorKeys.Undo)
            .RegisterBehavior<CollectionValidationSourceBehavior<TChildVM>>(PropertyBehaviorKeys.ValueValidationSource)
            .RegisterBehavior<ChangeNotifierCollectionBehavior<TChildVM>>(PropertyBehaviorKeys.ChangeNotifier);
      }
   }
}
