namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public abstract class MultiSelectionVM<TItemSource, TItemVM> :
      ViewModel<MultiSelectionVMDescriptor<TItemSource, TItemVM>>
      where TItemVM : IViewModel {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
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
         get { return GetValue(Descriptor.AllSourceItems); }
      }

      public ICollection<TItemSource> SelectedSourceItems {
         get { return GetValue(Descriptor.SelectedSourceItems); }
      }

      public IVMCollection<TItemVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      public IVMCollection<TItemVM> SelectedItems {
         get { return GetValue(Descriptor.SelectedItems); }
      }

      /// <summary>
      ///   Returns all source items for which the <see cref="ActiveItemFilter"/>
      ///   returns true or that are currently contained by selected items collection
      ///   of the source object.
      /// </summary>
      internal IEnumerable<TItemSource> GetActiveSourceItems() {
         IEnumerable<TItemSource> allSourceItems = GetValue(Descriptor.AllSourceItems);
         IEnumerable<TItemSource> selectedSourceItems = GetValue(Descriptor.SelectedSourceItems);

         if (ActiveItemFilter == null) {
            return allSourceItems;
         }

         return allSourceItems.Where(i =>
            ActiveItemFilter(i) ||
            selectedSourceItems.Contains(i)
         );
      }
   }



   public abstract class MultiSelectionVM<TItemSource> :
      MultiSelectionVM<TItemSource, SelectionItemVM<TItemSource>> {

      public MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }
   }

}
