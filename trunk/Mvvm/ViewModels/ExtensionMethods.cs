namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ExtensionMethods {
      public static void InitializeNext(this Behavior behavior, BehaviorInitializationContext context) {
         IBehaviorInitializationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Initialize(context);
         }
      }

      public static TValue GetValueNext<TValue>(this Behavior behavior, IBehaviorContext context, ValueStage stage) {
         return behavior
            .GetNextBehavior<IValueAccessorBehavior<TValue>>()
            .GetValue(context, stage);
      }

      public static void SetValueNext<TValue>(this Behavior behavior, IBehaviorContext context, TValue value) {
         behavior
            .GetNextBehavior<IValueAccessorBehavior<TValue>>()
            .SetValue(context, value);
      }

      public static void OnSelfChangedNext(
         this Behavior behavior,
         IBehaviorContext context,
         ChangeArgs args
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnSelfChanged(context, args);
         }
      }

      public static void OnChildChangedNext(
         this Behavior behavior,
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedChildPath
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnChildChanged(context, args, changedChildPath);
         }
      }

      public static void OnChangedNext(
         this Behavior behavior,
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnChanged(context, args, changedPath);
         }
      }

      public static void OnValidatingNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationArgs args
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnValidating(context, args);
         }
      }

      public static void ItemInsertedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemInserted(context, collection, item, index);
         }
      }

      public static void ItemRemovedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemRemoved(context, collection, item, index);
         }
      }

      public static void ItemSetNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemSet(context, collection, previousItem, item, index);
         }
      }

      public static void ItemsClearedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemsCleared(context, collection, previousItems);
         }
      }

      public static void UpdateFromSourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManuelUpdateBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateFromSource(context);
         }
      }

      public static void UpdateSourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManuelUpdateBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateSource(context);
         }
      }

      public static void RevalidateNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationMode mode
      ) {
         IRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context, mode);
         }
      }
   }
}
