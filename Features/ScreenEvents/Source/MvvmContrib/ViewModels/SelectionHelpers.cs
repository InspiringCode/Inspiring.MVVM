namespace Inspiring.Mvvm.ViewModels {
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class SelectionHelpers {
      internal static IEnumerable<TItemSource> GetSelectableSourceItems<TSourceObject, TItemSource>(
         IViewModel selectionVM
      ) {
         return GetItemProviderBehavior<TSourceObject, TItemSource>(selectionVM)
            .GetSelectableItems(selectionVM.GetContext());
      }

      internal static bool IsItemContainedInAllSourceItems<TItemSource>(
         IViewModel selectionVM,
         TItemSource sourceItem
      ) {
         IEnumerable<TItemSource> allSourceItems = GetAllSourceItems<TItemSource>(selectionVM);
         return allSourceItems.Contains(sourceItem, ReferenceEqualityComparer<TItemSource>.CreateSmartComparer());
      }

      internal static IEnumerable<TItemSource> GetAllSourceItems<TItemSource>(IViewModel selectionVM) {
         return (IEnumerable<TItemSource>)GetAllSourceItems(selectionVM);
      }

      internal static IEnumerable<TItemSource> GetSelectedSourceItems<TItemSource>(IViewModel selectionVM) {
         return (IEnumerable<TItemSource>)GetSelectedSourceItems(selectionVM);
      }

      internal static IEnumerable GetAllSourceItems(IViewModel selectionVM) {
         var typed = (ISelectionVM)selectionVM;
         return typed.AllSourceItems;
      }

      internal static IEnumerable GetSelectedSourceItems(IViewModel selectionVM) {
         var typed = (ISelectionVM)selectionVM;
         return typed.SelectedSourceItems;
      }

      private static ItemProviderBehavior<TSourceObject, TItemSource> GetItemProviderBehavior<TSourceObject, TItemSource>(IViewModel selectionVM) {
         return selectionVM
            .Descriptor
            .Behaviors
            .GetNextBehavior<ItemProviderBehavior<TSourceObject, TItemSource>>();
      }
   }
}
