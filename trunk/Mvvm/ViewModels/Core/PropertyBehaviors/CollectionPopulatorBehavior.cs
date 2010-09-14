namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionPopulatorBehavior<TVM, TSourceItem> :
      Behavior, IAccessPropertyBehavior<VMCollection<TVM>>
      where TVM : ViewModel, ICanInitializeFrom<TSourceItem> {

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         VMCollection<TVM> collection = this.GetNextValue(vm);
         IEnumerable<TSourceItem> source = GetSourceCollection(vm);
         PopulateCollection(vm, collection, source);
         return collection;
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      private IEnumerable<TSourceItem> GetSourceCollection(IBehaviorContext vm) {
         return GetNextBehavior<IAccessPropertyBehavior<IEnumerable<TSourceItem>>>().GetValue(vm);
      }

      private void PopulateCollection(IBehaviorContext behaviorContext, VMCollection<TVM> collection, IEnumerable<TSourceItem> source) {
         var viewModelFactory = GetNextBehavior<IViewModelFactoryBehavior<TVM>>();
         collection.Clear();

         foreach (TSourceItem item in source) {
            TVM vm = viewModelFactory.CreateInstance(behaviorContext);
            vm.InitializeFrom(item);
            collection.Add(vm);
         }
      }
   }
}
