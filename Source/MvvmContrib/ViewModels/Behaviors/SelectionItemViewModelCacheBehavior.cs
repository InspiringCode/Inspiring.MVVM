namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class SelectionItemViewModelCacheBehavior<TSource, TVM> :
      Behavior,
      IBehaviorInitializationBehavior
      where TVM : IViewModel, IHasSourceObject<TSource> {

      private readonly IVMDescriptor _itemDescriptor;
      private FieldDefinition<Dictionary<TSource, TVM>> _cache;

      public SelectionItemViewModelCacheBehavior(IVMDescriptor itemDescriptor) {
         _itemDescriptor = itemDescriptor;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _cache = context
            .Fields
            .DefineField<Dictionary<TSource, TVM>>(ViewModel.GeneralFieldGroup);

         this.InitializeNext(context);
      }

      public TVM GetVMForSource(IBehaviorContext context, TSource source) {
         var cache = GetCache(context);

         TVM cachedInstance;
         if (!cache.TryGetValue(source, out cachedInstance)) {
            cachedInstance = context.ServiceLocator.GetInstance<TVM>();
            cachedInstance.Descriptor = _itemDescriptor;
            cachedInstance.Source = source;

            cache.Add(source, cachedInstance);
         }

         return cachedInstance;
      }

      private Dictionary<TSource, TVM> GetCache(IBehaviorContext context) {
         Dictionary<TSource, TVM> cache;

         if (!context.FieldValues.TryGetValue(_cache, out cache)) {
            cache = new Dictionary<TSource, TVM>();
            context.FieldValues.SetValue(_cache, cache);
         }

         return cache;
      }
   }
}
