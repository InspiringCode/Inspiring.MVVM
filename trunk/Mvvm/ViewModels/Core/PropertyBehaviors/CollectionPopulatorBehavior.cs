namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionPopulatorBehavior<TVM, TSourceItem> :
      Behavior, IAccessPropertyBehavior<VMCollection<TVM>>
      where TVM : ICanInitializeFrom<TSourceItem> {

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         VMCollection<TVM> collection = this.GetNextValue(vm);
         IEnumerable<TSourceItem> source = GetSourceCollection(vm);
         PopulateCollection(collection, source);
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

      private void PopulateCollection(VMCollection<TVM> collection, IEnumerable<TSourceItem> source) {
         collection.Clear();

         foreach (TSourceItem item in source) {
            TVM vm = ServiceLocator.Current.GetInstance<TVM>();
            vm.InitializeFrom(item);
            collection.Add(vm);
         }
      }
   }
}
