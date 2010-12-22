namespace Inspiring.Mvvm.ViewModels.SingleSelection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class SingleSelectionVM<TItemSource, TItemVM> :
        ViewModel<SingleSelectionVMDescriptor<TItemSource, TItemVM>>
        where TItemVM : IViewModel {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal SingleSelectionVM(
         SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets or sets a filter that determines which items of the source items
      ///   should actually be returned by the <see cref="AllItems"/> property.
      ///   Items that were initially selected are always returned by the <see 
      ///   cref="AllItems"/> property.
      /// </summary>
      public Func<TItemSource, bool> ActiveItemFilter {
         get;
         set;
      }

      public IEnumerable<TItemSource> AllSourceItems {
         get { return GetValue(DescriptorBase.AllSourceItems); }
      }

      public TItemSource SelectedSourceItem {
         get { return GetValue(DescriptorBase.SelectedSourceItem); }
         set { SetValue(DescriptorBase.SelectedSourceItem, value); }
      }

      public IVMCollection<TItemVM> AllItems {
         get { return GetValue(DescriptorBase.AllItems); }
      }

      public TItemVM SelectedItem {
         get { return GetValue(DescriptorBase.SelectedItem); }
         set { SetValue(DescriptorBase.SelectedItem, value); }
      }

      /// <summary>
      ///   Returns all source items for which the <see cref="ActiveItemFilter"/>
      ///   returns true or that are currently contained by selected items collection
      ///   of the source object.
      /// </summary>
      internal IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = AllSourceItems;
         TItemSource selectedSourceItem = GetValue(DescriptorBase.SelectedSourceItem);

         if (ActiveItemFilter == null) {
            return allSourceItems;
         }

         return allSourceItems.Where(i =>
            ActiveItemFilter(i) ||
            Object.ReferenceEquals(selectedSourceItem, i)
         );
      }
   }



   public abstract class SingleSelectionVM<TItemSource> :
      SingleSelectionVM<TItemSource, SelectionItemVM<TItemSource>> {

      public SingleSelectionVM(
         SingleSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }
   }
}
