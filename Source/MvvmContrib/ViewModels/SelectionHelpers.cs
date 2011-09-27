namespace Inspiring.Mvvm.ViewModels {
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class SelectionHelpers {
      internal static IEnumerable<TItemSource> GetSelectableSourceItems<TItemSource>(
         IViewModel selectionVM
      ) {
         return GetItemProviderBehavior<TItemSource>(selectionVM)
            .GetSelectableItems(selectionVM.GetContext());
      }

      internal static bool IsItemContainedInAllSourceItems<TItemSource>(
         IViewModel selectionVM,
         TItemSource sourceItem
      ) {
         IEnumerable<TItemSource> allSourceItems = GetAllSourceItems<TItemSource>(selectionVM);
         return allSourceItems.Contains(sourceItem);
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

      private static ItemProviderBehavior<TItemSource> GetItemProviderBehavior<TItemSource>(IViewModel selectionVM) {
         return selectionVM
            .Descriptor
            .Behaviors
            .GetNextBehavior<ItemProviderBehavior<TItemSource>>();
      }
   }
}
