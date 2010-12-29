namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class KeywordSelectionVM : ViewModel<KeywordSelectionVMDescriptor>, ICanInitializeFrom<User> {
      public static readonly KeywordSelectionVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<KeywordSelectionVMDescriptor>()
         .For<KeywordSelectionVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var doc = c.GetPropertyBuilder(x => x.Document);

            d.AllItems = vm.Collection.Wraps(x => x.FilteredItems).With<GroupVM>(GroupVM.ClassDescriptor);
            d.SelectedItems = vm.Property.DelegatesTo(x => x.CreateSelectedItemsCollection());
            d.SelectedSourceItems = doc.Property.MapsTo(x => x.Groups);
         })
         .WithBehaviors((c) => {
            throw new NotImplementedException();
            //c.Disconnect(d.SelectedItems);
         })
         .Build();

      private IEnumerable<Group> _allSourceItems;

      public KeywordSelectionVM()
         : base() {
      }

      public User Document { get; private set; }

      public void InitializeFrom(User source) {
         Document = source;
      }

      public IEnumerable<Group> AllSourceItems {
         private get {
            if (_allSourceItems == null) {
               object injected = Kernel.ServiceLocator.TryGetInstance(typeof(IEnumerable<Group>));
               _allSourceItems = injected as IEnumerable<Group>;
            }
            return _allSourceItems;
         }
         set {
            _allSourceItems = value;
         }
      }

      public IVMCollection<GroupVM> SelectedItems {
         get { return GetValue(Descriptor.SelectedItems); }
      }

      public IVMCollection<GroupVM> AllItems {
         get { return GetValue(Descriptor.AllItems); }
      }

      private IEnumerable<Group> FilteredItems {
         get {
            return
               AllSourceItems.Where(i =>
                  IsItemSelectable(i) ||
                  SelectedItems.Any(x => x.GroupSource == i)
               )
               .ToArray();
         }
      }

      private bool IsItemSelectable(Group keyword) {
         return keyword.IsActive;
      }

      private ICollection<Group> SelectedSourceItems {
         get { return GetValue(Descriptor.SelectedSourceItems); }
      }

      private VMCollection<GroupVM> CreateSelectedItemsCollection() {
         throw new NotImplementedException();
         //var coll = new VMCollection<KeywordVM>(this, KeywordVM.Descriptor);

         IEnumerable<GroupVM> selectedItemVMs = SelectedSourceItems
            .Select(i => AllItems.Single(x => x.GroupSource == i));

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
      public VMProperty<IVMCollection<GroupVM>> AllItems { get; set; }
      public VMProperty<VMCollection<GroupVM>> SelectedItems { get; set; }
      internal VMProperty<ICollection<Group>> SelectedSourceItems { get; set; }
   }
}
