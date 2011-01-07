namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionValidationBuilder<TItemVM> :
      ICollectionValidationBuilder<TItemVM>
      where TItemVM : IViewModel {

      private BehaviorConfigurationDictionary _configs;
      private IVMProperty<VMCollection<TItemVM>> _property;

      public CollectionValidationBuilder(BehaviorConfigurationDictionary configs, IVMProperty<VMCollection<TItemVM>> property) {
         _configs = configs;
         _property = property;
      }

      public void Custom(CollectionValidator<TItemVM> validation) {
         BehaviorConfiguration conf = _configs.GetConfiguration((IVMProperty)_property); // HACK
         conf.Enable(VMBehaviorKey.CollectionValidator);
         conf.OverrideFactory(VMBehaviorKey.CollectionValidator, new CollectionValidationBehavior<TItemVM>());
         conf.Configure<CollectionValidationBehavior<TItemVM>>(VMBehaviorKey.CollectionValidator, c => {
            c.Add(validation);
         });
      }

      public ICollectionValidationBuilder<TItemVM, TValue> Check<TValue>(
         IVMProperty<TValue> itemProperty
      ) {
         return new CollectionValidationBuilder<TItemVM, TValue>(this, itemProperty);
      }
   }

   internal sealed class CollectionValidationBuilder<TItemVM, TItemValue> :
      ICollectionValidationBuilder<TItemVM, TItemValue>
      where TItemVM : IViewModel {

      private CollectionValidationBuilder<TItemVM> _parent;
      private IVMProperty<TItemValue> _itemProperty;

      public CollectionValidationBuilder(
         CollectionValidationBuilder<TItemVM> parent,
         IVMProperty<TItemValue> itemProperty
      ) {
         _parent = parent;
         _itemProperty = itemProperty;
      }

      public void Custom(Action<ValidationEventArgs<TItemVM, TItemValue>> validator) {
         throw new NotImplementedException();
         //_parent.Custom((item, items, args) => {
         //   if (args.Property == _itemProperty) {
         //      var validationItems = items.Select(vm =>
         //      new ValidationValue<TItemVM, TItemValue>(
         //         vm,
         //         _itemProperty.GetValue(vm)
         //      )
         //   );
         //      var a = new ValidationEventArgs<TItemVM, TItemValue>(
         //         validationItems, args
         //      );

         //      validator(a);
         //   }
         //});
      }
   }
}
