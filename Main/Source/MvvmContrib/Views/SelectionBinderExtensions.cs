//namespace Inspiring.Mvvm.Views {
//   using System;
//   using System.Linq.Expressions;
//   using Inspiring.Mvvm.ViewModels;
//   using Inspiring.Mvvm.ViewModels.Core;

//   public static class SelectionBinderExtensions {
//      public static void SingleSelection<TDescriptor, TSourceItem, TItemVM>(
//         this IVMBinder<TDescriptor> binder,
//         Expression<Func<TDescriptor, SingleSelectionProperty<TSourceItem>>> propertySelector,
//         Action<IVMBinder<SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>>> viewModelBinder
//      ) where TItemVM : SelectionItemVM<TSourceItem> 
//      where TDescriptor : VMDescriptor {
//         //binder.VM<
//         var x = propertySelector.Compile()(null);

//         IBindableProperty<ViewModel<SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>>> y = x;

//         IBindableProperty<SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>> test1 = x;

//         IBindableProperty<ViewModel<SingleSelectionVMDescriptor<TSourceItem, TItemVM>>> test2 = x;


//         IBindableProperty<ViewModel<SingleSelectionVMDescriptor<TSourceItem, TItemVM>>> test = x;

//         //binder.VM(
//         //   propertySelector, viewModelBinder);

//         binder.VM<SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>>(
//            propertySelector,
//            viewModelBinder
//         );

//         binder.VM<SingleSelectionVMDescriptor<TSourceItem, TItemVM>>(
//            propertySelector,
//            viewModelBinder
//         );
//      }
//   }
//}
