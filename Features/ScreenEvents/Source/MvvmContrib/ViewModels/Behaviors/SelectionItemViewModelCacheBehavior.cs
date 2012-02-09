namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class SelectionItemViewModelCacheBehavior<TItemSource, TItemVM> :
      Behavior,
      IBehaviorInitializationBehavior,
      IRefreshControllerBehavior
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      private readonly IVMDescriptor _itemDescriptor;
      private FieldDefinition<Dictionary<TItemSource, TItemVM>> _cache;

      public SelectionItemViewModelCacheBehavior(
         IVMDescriptor itemDescriptor
      ) {
         _itemDescriptor = itemDescriptor;
      }

      public void Initialize(BehaviorInitializationContext context) {
         _cache = context
            .Fields
            .DefineField<Dictionary<TItemSource, TItemVM>>(ViewModel.GeneralFieldGroup);

         this.InitializeNext(context);
      }

      public TItemVM GetVMForSource(IBehaviorContext context, TItemSource source) {
         Dictionary<TItemSource, TItemVM> cache = GetCache(context);

         TItemVM cachedInstance;
         if (!cache.TryGetValue(source, out cachedInstance)) {
            cachedInstance = context.ServiceLocator.GetInstance<TItemVM>();
            cachedInstance.Descriptor = _itemDescriptor;
            cachedInstance.Source = source;

            cache.Add(source, cachedInstance);
         }

         return cachedInstance;
      }

      private Dictionary<TItemSource, TItemVM> GetCache(IBehaviorContext context) {
         Dictionary<TItemSource, TItemVM> cache;

         if (!context.FieldValues.TryGetValue(_cache, out cache)) {
            // Reference equality because:
            //   (1) If the source objects are reloaded in a new NHibernate session,
            //       the are equal to the old object and thus item VMs would be mistakenly
            //       reused.
            //   (2) If two items of the all source items collection are equal, only one 
            //       view model would be created for both.
            cache = new Dictionary<TItemSource, TItemVM>(ReferenceEqualityComparer<TItemSource>.CreateSmartComparer());
            context.FieldValues.SetValue(_cache, cache);
         }

         return cache;
      }

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         this.ViewModelRefreshNext(context, executeRefreshDependencies);
      }

      public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property, RefreshOptions options) {
         var descriptor = (SelectionVMDescriptor<TItemSource, TItemVM>)context
            .VM
            .Descriptor;

         if (property == descriptor.AllItems) {
            DiscardUnusedEntries(context);
         }

         this.ViewModelRefreshNext(context, property, options);
      }

      private void DiscardUnusedEntries(IBehaviorContext context) {
         var activeSourceItems = new HashSet<TItemSource>(
            SelectionHelpers.GetAllSourceItems<TItemSource>(context.VM)
         );

         Dictionary<TItemSource, TItemVM> cache = GetCache(context);

         foreach (TItemSource entrySource in cache.Keys.ToArray()) {
            if (!activeSourceItems.Contains(entrySource)) {
               cache.Remove(entrySource);
            }
         }
      }
   }
}
