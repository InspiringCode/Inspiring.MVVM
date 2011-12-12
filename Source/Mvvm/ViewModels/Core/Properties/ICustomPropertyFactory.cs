namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using System.Windows.Input;

   public interface ICustomPropertyFactory<TSourceObject> {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TValue}"/> object.
      /// </param>
      IVMPropertyDescriptor<TValue> Property<TValue>(
         IBehavior valueAccessor,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      );

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TValue}"/> object.
      /// </param>
      IVMPropertyDescriptor<TValue> PropertyWithSource<TValue>(
         IBehavior valueAccessor,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      );

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TChildVM}"/> object.
      /// </param>
      IVMPropertyDescriptor<TChildVM> ViewModelProperty<TChildVM>(
         IBehavior valueAccessor,
         IBehavior sourceAccessor = null,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      ) where TChildVM : IViewModel;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TChildVM}"/> object.
      /// </param>
      /// <param name="sourceAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TChildSource}"/> object.
      /// </param>
      IVMPropertyDescriptor<TChildVM> ViewModelPropertyWithSource<TChildVM, TChildSource>(
         IBehavior valueAccessor,
         IBehavior sourceAccessor = null,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource>;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{T}"/> (T is <see cref="IVMCollection{TChildVM}"/>) object.
      /// </param>
      /// <param name="sourceAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{T}"/> (T is <see cref="IEnumerable{TChildVM}"/>) object.
      /// </param>
      IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionProperty<TChildVM>(
         IVMDescriptor itemDescriptor,
         IBehavior valueAccessor,
         IBehavior sourceAccessor = null,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      ) where TChildVM : IViewModel;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{T}"/> (T is <see cref="IVMCollection{TChildVM}"/>) object.
      /// </param>
      /// <param name="sourceAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{T}"/> (T is <see cref="IEnumerable{TChildSource}"/>) object.
      /// </param>
      IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionPropertyWithSource<TChildVM, TChildSource>(
         IVMDescriptor itemDescriptor,
         IBehavior valueAccessor,
         IBehavior sourceAccessor = null,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource>;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="valueAccessor">
      ///   Usually an <see cref="IValueAccessorBehavior{TSourceObject}"/> object.
      /// </param>
      IVMPropertyDescriptor<ICommand> CommandProperty(
         IBehavior sourceObjectAccessor,
         IBehavior commandExecutor,
         Action<BehaviorChainConfiguration> chainConfigurationAction = null
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
