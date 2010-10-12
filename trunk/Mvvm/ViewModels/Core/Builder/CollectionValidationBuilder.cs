namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class CollectionValidationBuilder<TItemVM> :
      ICollectionValidationBuilder<TItemVM>
      where TItemVM : ViewModel {

      private BehaviorConfigurationDictionary _configs;
      private IVMProperty<VMCollection<TItemVM>> _property;

      public CollectionValidationBuilder(BehaviorConfigurationDictionary configs, IVMProperty<VMCollection<TItemVM>> property) {
         _configs = configs;
         _property = property;
      }

      public void Custom(CollectionValidator<TItemVM> validation) {
         BehaviorConfiguration conf = _configs.GetConfiguration((VMProperty)_property); // HACK
         conf.Enable(VMBehaviorKey.CollectionValidator);
         conf.OverrideFactory(VMBehaviorKey.CollectionValidator, new CollectionValidationBehavior<TItemVM>());
         conf.Configure<CollectionValidationBehavior<TItemVM>>(VMBehaviorKey.CollectionValidator, c => {
            c.Add(validation);
         });
      }
   }
}
