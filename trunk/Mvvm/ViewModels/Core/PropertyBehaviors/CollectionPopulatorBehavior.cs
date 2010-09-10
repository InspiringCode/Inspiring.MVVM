namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionPopulatorBehavior<TVM, TSourceItem> :
      VMPropertyBehavior, IAccessPropertyBehavior<VMCollection<TVM>>
      where TVM : ICanInitializeFrom<TSourceItem> {

      private IAccessPropertyBehavior<IEnumerable<TSourceItem>> _sourceAccessor;

      public CollectionPopulatorBehavior(
         IAccessPropertyBehavior<IEnumerable<TSourceItem>> sourceAccessor
      ) {
         _sourceAccessor = sourceAccessor;
      }

      public override BehaviorPosition Position {
         get { return BehaviorPosition.CollectionPopulator; }
      }

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         VMCollection<TVM> collection = this.GetNextValue(vm);
         IEnumerable<TSourceItem> source = _sourceAccessor.GetValue(vm);
         PopulateCollection(collection, source);
         return collection;
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
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
