namespace Inspiring.Mvvm.ViewModels.Core {
   // TODO: Reintroduce AddNew capability!

   //using System.Collections.Generic;

   //internal sealed class ItemCreationController<TParentVM, TItemVM, TItemSource> :
   //   IItemCreationController<TItemVM>
   //   where TParenTVM : IViewModel
   //   where TItemVM : IViewModel {

   //   private TParentVM _parent;
   //   private IViewModelFactoryBehavior<TItemVM> _viewModelFactory;
   //   private ICollection<TItemSource> _sourceCollection;
   //   private ItemCreationArguments<TItemSource> _lastArguments = null; // TODO: Add some assertions.

   //   public ItemCreationController(
   //      TParentVM parent,
   //      IViewModelFactoryBehavior<TItemVM> viewModelFactory,
   //      ICollection<TItemSource> collectionSource
   //   ) {
   //      _parent = parent;
   //      _viewModelFactory = viewModelFactory;
   //      _sourceCollection = collectionSource;
   //   }

   //   public TItemVM AddNew() {
   //      TItemVM vm = _viewModelFactory.CreateInstance(_parent);

   //      _lastArguments = new ItemCreationArguments<TItemSource>();
   //      _lastArguments.IsStartNewItem = true;

   //      // TODO: Get rid of cast
   //      ((ICreatableItem<TParentVM, TItemSource>)vm).OnNewItem(_lastArguments, _parent);

   //      if (_lastArguments.AddToSourceCollection == AddItemTime.AfterStartNew) {
   //         _sourceCollection.Add(_lastArguments.NewSoureObject);
   //      }

   //      return vm;
   //   }

   //   public void EndNew(TItemVM transientItem) {
   //      _lastArguments.IsCancelNewItem = false;
   //      _lastArguments.IsStartNewItem = false;
   //      _lastArguments.IsEndNewItem = true;
   //      _lastArguments.IsEndedNewItem = false;

   //      // TODO: Get rid of cast
   //      ((ICreatableItem<TParentVM, TItemSource>)transientItem).OnNewItem(_lastArguments, _parent);

   //      if (_lastArguments.AddToSourceCollection == AddItemTime.AfterEndNew) {
   //         _sourceCollection.Add(_lastArguments.NewSoureObject);
   //      }

   //      _lastArguments.IsCancelNewItem = false;
   //      _lastArguments.IsStartNewItem = false;
   //      _lastArguments.IsEndNewItem = false;
   //      _lastArguments.IsEndedNewItem = true;

   //      // TODO: Get rid of cast
   //      ((ICreatableItem<TParentVM, TItemSource>)transientItem).OnNewItem(_lastArguments, _parent);

   //      _lastArguments = null;
   //   }

   //   public void CancelNew(TItemVM transientItem) {
   //      _lastArguments.IsCancelNewItem = true;
   //      _lastArguments.IsStartNewItem = false;
   //      _lastArguments.IsEndNewItem = false;
   //      _lastArguments.IsEndedNewItem = false;

   //      // TODO: Get rid of cast
   //      ((ICreatableItem<TParentVM, TItemSource>)transientItem).OnNewItem(_lastArguments, _parent);

   //      if (_lastArguments.AddToSourceCollection == AddItemTime.AfterStartNew) {
   //         _sourceCollection.Remove(_lastArguments.NewSoureObject);
   //      }

   //      _lastArguments = null;
   //   }
   //}
}
