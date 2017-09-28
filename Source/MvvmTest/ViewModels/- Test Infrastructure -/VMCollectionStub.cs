namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class VMCollectionStub<TItemVM> : Collection<TItemVM>, IVMCollection<TItemVM> where TItemVM : IViewModel {
      public VMCollectionStub(IViewModel ownerVM, IVMPropertyDescriptor ownerProperty) {
         OwnerVM = ownerVM;
         OwnerProperty = ownerProperty;
      }

      public virtual void ReplaceItems(IEnumerable<TItemVM> newItems, IChangeReason reason) {
         Clear();
         newItems.ForEach(Add);
      }

      public IViewModel OwnerVM {
         get;
         private set;
      }

      public IVMPropertyDescriptor OwnerProperty {
         get;
         private set;
      }
   }

   public class VMCollectionStub {
      public static VMCollectionStubBuilder<T> WithItems<T>(IEnumerable<T> items) where T : IViewModel {
         return new VMCollectionStubBuilder<T>().WithItems(items);
      }

      public static VMCollectionStubBuilder<T> WithItems<T>(params T[] items) where T : IViewModel {
         return new VMCollectionStubBuilder<T>().WithItems(items);
      }

      public static VMCollectionStubBuilder<ViewModelStub> WithOwner(IViewModel viewModel) {
         return new VMCollectionStubBuilder<ViewModelStub>().WithOwner(viewModel);
      }

      public static VMCollectionStubBuilder<ViewModelStub> WithOwnerProperty(IVMPropertyDescriptor property) {
         return new VMCollectionStubBuilder<ViewModelStub>().WithOwnerProperty(property);
      }

      public static VMCollectionStubBuilder<T> Of<T>() where T : IViewModel {
         return new VMCollectionStubBuilder<T>();
      }

      public static VMCollectionStub<ViewModelStub> Build() {
         return new VMCollectionStubBuilder<ViewModelStub>().Build();
      }

      public static VMCollectionStub<T> Build<T>() where T : IViewModel {
         return new VMCollectionStubBuilder<T>().Build();
      }
   }

   public class VMCollectionStubBuilder<T> where T : IViewModel {
      private IViewModel _ownerVM;
      private IVMPropertyDescriptor _ownerProperty;
      private IEnumerable<T> _items;

      public VMCollectionStubBuilder() {
         _ownerVM = ViewModelStub.Build();
         _ownerProperty = PropertyStub.Build();
         _items = Enumerable.Empty<T>();
      }

      public VMCollectionStubBuilder<T> WithItems(IEnumerable<T> items) {
         _items = items;
         return this;
      }

      public VMCollectionStubBuilder<T> WithOwner(IViewModel viewModel) {
         _ownerVM = viewModel;
         return this;
      }

      public VMCollectionStubBuilder<T> WithOwnerProperty(IVMPropertyDescriptor property) {
         _ownerProperty = property;
         return this;
      }

      public VMCollectionStub<T> Build() {
         var c = new VMCollectionStub<T>(_ownerVM, _ownerProperty);

         c.ReplaceItems(_items, null);

         _items
            .Cast<IViewModel>()
            .ForEach(x => x.Kernel.OwnerCollections.Add(c));

         return c;
      }
   }
}
