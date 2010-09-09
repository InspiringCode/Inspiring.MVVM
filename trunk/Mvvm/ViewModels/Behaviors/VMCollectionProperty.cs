namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Behaviors;

   public sealed class VMCollectionProperty<TVM> : VMPropertyBase<VMCollection<TVM>> {
      public VMCollectionProperty(IBehavior collectionPopulator) {
         IBehavior collectionValueCache = new CacheValueBehavior<VMCollection<TVM>>(
            BehaviorPosition.CollectionValueCache
         );

         IBehavior collectionInstanceCache = new CacheValueBehavior<VMCollection<TVM>>(
            BehaviorPosition.CollectionInstanceCache
         );

         IBehavior collectionFactory = new CollectionFactoryBehavior<TVM>();

         InsertBehavior(collectionValueCache);
         InsertBehavior(collectionPopulator);
         InsertBehavior(collectionInstanceCache);
         InsertBehavior(collectionFactory);
      }
   }
}
