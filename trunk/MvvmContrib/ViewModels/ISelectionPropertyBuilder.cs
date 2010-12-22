//namespace Inspiring.Mvvm.ViewModels {
//   using System;
//   using System.Collections.Generic;
//   using System.Linq.Expressions;
//   using Inspiring.Mvvm.ViewModels.Core;
//   using Inspiring.Mvvm.ViewModels.Fluent;

//   public interface ISingleSelectionPropertyBuilder<TParentVM> where TParentVM : IViewModel {
//      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithItems<TSourceItem>(
//         Func<TParentVM, IEnumerable<TSourceItem>> itemSourceGetter,
//         Func<TSourceItem, bool> currentlySelectablePredicate = null
//      );
//   }

//   public interface ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> where TParentVM : IViewModel {
//      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
//         Expression<Func<TParentVM, TSourceItem>> selectedItemSelector
//      );

//      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
//         Func<TParentVM, TSourceItem> selectionGetter,
//         Action<TParentVM, TSourceItem> selectionSetter
//      );

//      SingleSelectionProperty<TSourceItem> Of(
//         Func<IVMPropertyBuilder<TSourceItem>, VMDescriptor> descriptorFactory,
//         Action<
//            SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>,
//            IValidationBuilder<SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>>
//         > validationConfigurator = null
//      );

//      SingleSelectionProperty<TSourceItem, TItemVM> Of<TItemVM>(
//         Func<_IVMPropertyFactory<SelectionItemVM<TSourceItem>, TSourceItem>, VMDescriptor> descriptorFactory
//      ) where TItemVM : SelectionItemVM<TSourceItem>;
//   }
//}
