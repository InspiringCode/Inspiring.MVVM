namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Resources;

   public interface IMultiSelectionVM {
      IList AllItems { get; }
      IList SelectedItems { get; }
      Type ItemSourceType { get; }
      Type ItemVMType { get; }
   }

   public abstract class MultiSelectionBaseVM<TItemSource, TItemVM> :
      SelectionVM<MultiSelectionVMDescriptor<TItemSource, TItemVM>, TItemSource, TItemVM>,
      IMultiSelectionVM,
      ISelectionVM
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionBaseVM(
         MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {

         //NonExistingSelectedSourceItems = new List<TItemSource>();
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
         set {
            var selectedSourceItems = GetValue(Descriptor.SelectedSourceItems);
            selectedSourceItems.Clear();

            if (value != null) {
               var newItemsContainedByAllSourceItems = value
                  .All(x => AllSourceItems.Contains(x));

               if (!newItemsContainedByAllSourceItems) {
                  throw new ArgumentException(ExceptionTexts.SourceItemNotContainedByAllSourceItems);
               }

               foreach (var item in value) {
                  selectedSourceItems.Add(item);
               }
            }

            Kernel.Refresh(Descriptor.AllItems);
            Kernel.Refresh(Descriptor.SelectedItems);
         }
      }

      public IVMCollection<TItemVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      public IVMCollection<TItemVM> SelectedItems {
         get { return GetValue(Descriptor.SelectedItems); }
      }

      ///// <summary>
      /////   May contain items that are selected, but not in the AllSourceItems collection.
      ///// </summary>
      //internal ICollection<TItemSource> NonExistingSelectedSourceItems { get; private set; }

      IList IMultiSelectionVM.AllItems {
         get { return AllItems; }
      }

      IList IMultiSelectionVM.SelectedItems {
         get { return SelectedItems; }
      }

      Type IMultiSelectionVM.ItemSourceType {
         get { return typeof(TItemSource); }
      }

      Type IMultiSelectionVM.ItemVMType {
         get { return typeof(TItemVM); }
      }

      public object SelectedItemsDisplayValue {
         get { return GetDisplayValue(Descriptor.SelectedItems); }
         set { SetDisplayValue(Descriptor.SelectedItems, value); }
      }

      internal void RaisePropertyChangedForSelectedItems() {
         OnPropertyChanged("SelectedItems");
      }

      //SourceItemCollections<TItemSource> IHasSourceItems<TItemSource>.SourceItems {
      //   get {
      //      return Descriptor
      //         .AllItems
      //         .Behaviors
      //         .GetNextBehavior<ItemProviderBehavior<TItemSource>>()
      //         .GetSelectableItems(GetContext());
      //   }
      //}

      ///// <summary>
      /////   Returns all source items for which the <see cref="ActiveItemFilter"/>
      /////   returns true or that are currently contained by selected items collection
      /////   of the source object.
      /////   All selected items are always contained, even if they are not in the collection of
      /////   all items.
      ///// </summary>
      //internal IEnumerable<TItemSource> GetActiveSourceItems() {
      //   IEnumerable<TItemSource> allSourceItems = GetValue(Descriptor.AllSourceItems);
      //   IEnumerable<TItemSource> selectedSourceItems = GetValue(Descriptor.SelectedSourceItems);
      //   IEnumerable<TItemSource> activeSourceItems = null;

      //   if (allSourceItems == null) {
      //      activeSourceItems = new TItemSource[0];
      //   } else if (ActiveItemFilter == null) {
      //      activeSourceItems = allSourceItems;
      //   } else {
      //      activeSourceItems = allSourceItems
      //         .Where(i =>
      //            ActiveItemFilter(i) ||
      //            selectedSourceItems.Contains(i)
      //         )
      //         .ToArray();
      //   }

      //   NonExistingSelectedSourceItems = selectedSourceItems
      //      .Except(activeSourceItems)
      //      .ToArray();

      //   if (NonExistingSelectedSourceItems.Any()) {
      //      activeSourceItems = activeSourceItems
      //         .Concat(NonExistingSelectedSourceItems)
      //         .ToArray();
      //   }

      //   return activeSourceItems;
      //}

      protected override string ProvideErrorMessage(string propertyName) {
         if (propertyName == Descriptor.SelectedItems.PropertyName) {

            var groupedErrors = ValidationResult
               .Errors
               .Select(e => {
                  IViewModel targetVM = e.Target.VM;

                  string itemCaption = targetVM != null && targetVM != this ?
                     targetVM.ToString() :
                     null;

                  return new {
                     ItemCaption = itemCaption,
                     ErrorMessage = e.Message
                  };
               })
               .GroupBy(x => x.ItemCaption)
               .Select(x => new {
                  ItemCaption = x.Key,
                  Messages = String.Join(
                     Localized.MultiSelectionCompositeValidationErrorSeparator,
                     x.Select(entry => entry.ErrorMessage)
                  )
               })
               .ToArray();

            if (groupedErrors.Count() == 0) {
               return null;
            }

            IEnumerable<string> propertyErrorLines = groupedErrors
               .Where(x => x.ItemCaption != null)
               .Select(x => {
                  return Localized
                    .MultiSelectionCompositeValidationErrorPropertyLine
                    .FormatWith(x.ItemCaption, x.Messages);
               });

            IEnumerable<string> viewModelErrorLine = groupedErrors
               .Where(x => x.ItemCaption == null)
               .Select(x => {
                  return Localized
                     .MultiSelectionCompositeValidationErrorViewModelLine
                     .FormatWith(x.Messages);
               });

            IEnumerable<string> errorLines = propertyErrorLines.Concat(viewModelErrorLine);
            IEnumerable<string> header = new[] { Localized.MultiSelectionCompositeValidationError };

            return String.Join(
               Environment.NewLine,
               header.Concat(errorLines)
            );
         }

         return base.ProvideErrorMessage(propertyName);
      }

      IEnumerable ISelectionVM.AllSourceItems {
         get { return AllSourceItems; }
      }

      IEnumerable ISelectionVM.SelectedSourceItems {
         get { return SelectedSourceItems; }
      }
   }

   public abstract class MultiSelectionVM<TItemSource> :
      MultiSelectionBaseVM<TItemSource, SelectionItemVM<TItemSource>> {

      public MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }
   }

   public abstract class MultiSelectionVM<TItemSource, TItemVM> :
      MultiSelectionBaseVM<TItemSource, SelectableItemVM<TItemSource, TItemVM>>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      public MultiSelectionVM(
         MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {

      }
   }
}
