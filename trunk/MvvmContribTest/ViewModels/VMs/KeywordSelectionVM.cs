namespace Inspiring.MvvmContribTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class KeywordSelectionVM : ViewModel<KeywordSelectionVMDescriptor>, ICanInitializeFrom<Document> {
      public static readonly KeywordSelectionVMDescriptor Descriptor = VMDescriptorBuilder
         .For<KeywordSelectionVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();
            var d = c.GetPropertyBuilder(x => x.Document);

            return new KeywordSelectionVMDescriptor {
               AllItems = vm.Collection.Wraps(x => x.FilteredItems).With<KeywordVM>(KeywordVM.Descriptor),
               SelectedItems = vm.Property.DelegatesTo(x => x.CreateSelectedItemsCollection()),
               SelectedSourceItems = d.Property.MapsTo(x => x.Keywords)
            };
         })
         .WithBehaviors((c) => {
            throw new NotImplementedException();
            //c.Disconnect(d.SelectedItems);
         })
         .Build();

      private IEnumerable<Keyword> _allSourceItems;

      public KeywordSelectionVM()
         : base() {
      }

      public Document Document { get; private set; }

      public void InitializeFrom(Document source) {
         Document = source;
      }

      public IEnumerable<Keyword> AllSourceItems {
         private get {
            if (_allSourceItems == null) {
               object injected = Kernel.ServiceLocator.TryGetInstance(typeof(IEnumerable<Keyword>));
               _allSourceItems = injected as IEnumerable<Keyword>;
            }
            return _allSourceItems;
         }
         set {
            _allSourceItems = value;
         }
      }

      public IVMCollection<KeywordVM> SelectedItems {
         get { return GetValue(Descriptor.SelectedItems); }
      }

      public IVMCollection<KeywordVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      private IEnumerable<Keyword> FilteredItems {
         get {
            return
               AllSourceItems.Where(i =>
                  IsItemSelectable(i) ||
                  SelectedItems.Any(x => x.Keyword == i)
               )
               .ToArray();
         }
      }

      private bool IsItemSelectable(Keyword keyword) {
         return keyword.IsActive();
      }

      private ICollection<Keyword> SelectedSourceItems {
         get { return GetValue(Descriptor.SelectedSourceItems); }
      }

      private VMCollection<KeywordVM> CreateSelectedItemsCollection() {
         throw new NotImplementedException();
         //var coll = new VMCollection<KeywordVM>(this, KeywordVM.Descriptor);

         IEnumerable<KeywordVM> selectedItemVMs = SelectedSourceItems
            .Select(i => AllItems.Single(x => x.Keyword == i));

         //coll.Popuplate(
         //   selectedItemVMs,
         //   new SelectedItemsSynchronizer(SelectedSourceItems)
         //);

         //return coll;
      }

      //private class SelectedItemsSynchronizer : ICollectionModificationController<KeywordVM> {
      //   private ICollection<Keyword> _selectedSourceItems;

      //   public SelectedItemsSynchronizer(ICollection<Keyword> selectedSourceItems) {
      //      _selectedSourceItems = selectedSourceItems;
      //   }
      //   public void Insert(KeywordVM item, int index) {
      //      _selectedSourceItems.Add(item.Keyword);
      //   }

      //   public void Remove(KeywordVM item) {
      //      _selectedSourceItems.Remove(item.Keyword);
      //   }

      //   public void SetItem(KeywordVM item, int index) {
      //      throw new NotSupportedException();
      //   }

      //   public void Clear() {
      //      _selectedSourceItems.Clear();
      //   }
      //}
   }

   internal sealed class KeywordSelectionVMDescriptor : VMDescriptor {
      public VMProperty<IVMCollection<KeywordVM>> AllItems { get; set; }
      public VMProperty<VMCollection<KeywordVM>> SelectedItems { get; set; }
      internal VMProperty<ICollection<Keyword>> SelectedSourceItems { get; set; }
   }
}
