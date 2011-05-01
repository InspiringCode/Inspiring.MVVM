namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using System.Windows.Input;

   public interface ICustomPropertyFactory<TSourceObject> {
      IVMPropertyDescriptor<TValue> Property<TValue>(
         IValueAccessorBehavior<TValue> valueAccessor
      );

      IVMPropertyDescriptor<TValue> PropertyWithSource<TValue>(
         IValueAccessorBehavior<TValue> valueAccessor
      );

      IVMPropertyDescriptor<TChildVM> ViewModelProperty<TChildVM>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IBehavior sourceAccessor = null
      ) where TChildVM : IViewModel;

      IVMPropertyDescriptor<TChildVM> ViewModelPropertyWithSource<TChildVM, TChildSource>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IValueAccessorBehavior<TChildSource> sourceAccessor = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource>;

      IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionProperty<TChildVM>(
         VMDescriptorBase itemDescriptor,
         IValueAccessorBehavior<IVMCollection<TChildVM>> valueAccessor,
         IValueAccessorBehavior<IEnumerable<TChildVM>> sourceAccessor = null
      ) where TChildVM : IViewModel;

      IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionPropertyWithSource<TChildVM, TChildSource>(
         VMDescriptorBase itemDescriptor,
         IValueAccessorBehavior<IVMCollection<TChildVM>> valueAccessor,
         IValueAccessorBehavior<IEnumerable<TChildSource>> sourceAccessor = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource>;

      IVMPropertyDescriptor<ICommand> CommandProperty(
         IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
         IBehavior commandExecutor
      );

      IVMPropertyDescriptor<TValue> CustomProperty<TValue>(
         BehaviorChainTemplateKey templateKey,
         IBehaviorFactoryConfiguration factoryConfiguration,
         Action<BehaviorChainConfiguration> chainConfigurationAction
      );

      IVMPropertyDescriptor<TValue> CustomProperty<TValue>(
         BehaviorChainConfiguration behaviorChain
      );

      IValueAccessorBehavior<TValue> CreateDelegateAccessor<TValue>(
         Func<TSourceObject, TValue> getter,
         Action<TSourceObject, TValue> setter = null
      );

      IValueAccessorBehavior<TValue> CreateMappedAccessor<TValue>(
         Expression<Func<TSourceObject, TValue>> valueSelector
      );

      IValueAccessorBehavior<TSourceObject> CreateSourceObjectAccessor();
   }
}
