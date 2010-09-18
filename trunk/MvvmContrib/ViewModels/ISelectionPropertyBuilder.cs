namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;

   public interface ISingleSelectionPropertyBuilder<TParentVM> where TParentVM : ViewModel {
      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithItems<TSourceItem>(
         Func<TParentVM, IEnumerable<TSourceItem>> itemSourceGetter,
         Func<TSourceItem, bool> currentlySelectablePredicate = null
      );
   }

   public interface ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> where TParentVM : ViewModel {
      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
         Expression<Func<TParentVM, TSourceItem>> selectedItemSelector
      );

      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
         Func<TParentVM, TSourceItem> selectionGetter,
         Action<TParentVM, TSourceItem> selectionSetter
      );

      SingleSelectionProperty<TSourceItem> Of(
         Func<IVMPropertyFactory<TSourceItem>, VMDescriptor> descriptorFactory
      );

      SingleSelectionProperty<TSourceItem, TItemVM> Of<TItemVM>(
         Func<IVMPropertyFactory<TSourceItem>, VMDescriptor> descriptorFactory
      ) where TItemVM : SelectionItemVM<TSourceItem>;
   }
}
