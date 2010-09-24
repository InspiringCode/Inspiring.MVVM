namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CollectionPopulatorBehavior<TParentVM, TItemVM, TItemSource> :
      Behavior, IAccessPropertyBehavior<VMCollection<TItemVM>>
      where TItemVM : ViewModel, ICanInitializeFrom<TItemSource>
      where TParentVM : ViewModel {

      public VMCollection<TItemVM> GetValue(IBehaviorContext vm) {
         VMCollection<TItemVM> collection = this.GetNextValue(vm);
         IEnumerable<TItemSource> source = GetSourceCollection(vm);
         PopulateCollection(vm, collection, source);
         return collection;
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      private IEnumerable<TItemSource> GetSourceCollection(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<IEnumerable<TItemSource>>>().GetValue(vm);
      }

      private void PopulateCollection(
         IBehaviorContext behaviorContext,
         VMCollection<TItemVM> collection,
         IEnumerable<TItemSource> source
      ) {
         // TODO: Handling of collection that are not modifiable...
         var viewModelFactory = GetNextBehavior<IViewModelFactoryBehavior<TItemVM>>();

         var itemController = new ItemCreationController<TParentVM, TItemVM, TItemSource>(
            (TParentVM)behaviorContext,
            viewModelFactory,
            (ICollection<TItemSource>)source
         );

         var collectionController = new CollectionModificationController<TItemVM, TItemSource>(
            (ICollection<TItemSource>)source
         );

         IEnumerable<TItemVM> viewModels;

         if (source != null) {
            viewModels = source.Select(item => {
               TItemVM vm = viewModelFactory.CreateInstance(behaviorContext);
               vm.InitializeWithDescriptor(collection.ItemDescriptor);
               vm.InitializeFrom(item);
               return vm;
            });
         } else {
            viewModels = Enumerable.Empty<TItemVM>();
         }

         collection.Repopulate(viewModels, itemController, collectionController);

         //var viewModelFactory = GetNextBehavior<IViewModelFactoryBehavior<TItemVM>>();
         //collection.Clear();

         //foreach (TItemSource item in source) {
         //   TItemVM vm = viewModelFactory.CreateInstance(behaviorContext);
         //   vm.InitializeWithDescriptor(collection.ItemDescriptor);
         //   vm.InitializeFrom(item);
         //   collection.Add(vm);
         //}
      }
   }
}
