namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;

   public interface ISingleSelectionPropertyBuilder<TVM> {
      ISingleSelectionPropertyBuilder<TVM, TSourceItem> WithItems<TSourceItem>(
         Func<TVM, IEnumerable<TSourceItem>> itemSourceGetter,
         Func<TSourceItem, bool> currentlySelectablePredicate = null
      );
   }

   public interface ISingleSelectionPropertyBuilder<TVM, TSourceItem> {
      ISingleSelectionPropertyBuilder<TVM, TSourceItem> WithSelection(
         Expression<Func<TVM, TSourceItem>> selectedItemSelector
      );

      ISingleSelectionPropertyBuilder<TVM, TSourceItem> WithSelection(
         Func<TVM, TSourceItem> selectionGetter,
         Func<TVM, TSourceItem> selectionSetter
      );

      VMProperty<SingleSelectionVM<TSourceItem, TVM>> Of(
         Func<IVMPropertyFactory<TSourceItem>, VMDescriptor> descriptorFactory
      );
   }
}
