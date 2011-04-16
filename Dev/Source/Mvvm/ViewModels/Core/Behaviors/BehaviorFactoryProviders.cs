namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class BehaviorFactoryProviders : BehaviorFactoryProviderInterfaces {
      public static readonly IBehaviorFactoryProvider SimpleProperty = new SimplePropertyProvider();
      public static readonly IBehaviorFactoryProvider ViewModelProperty = new ViewModelPropertyProvider();
      public static readonly IBehaviorFactoryProvider CollectionProperty = new CollectionPropertyFactoryProvider();
      public static readonly IBehaviorFactoryProvider CommandProperty = new CommandPropertyProvider();
      public static readonly IBehaviorFactoryProvider ViewModel = new ViewModelProvider();

      protected abstract class PropertyProvider {
         protected virtual BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>() {
            return new BehaviorFactory()
               .RegisterBehavior<DisplayValueAccessorBehavior<TValue>>(PropertyBehaviorKeys.DisplayValueAccessor)
               .RegisterBehavior<UntypedPropertyAccessorBehavior<TValue>>(PropertyBehaviorKeys.UntypedValueAccessor)
               .RegisterBehavior<UndoSetValueBehavior<TValue>>(PropertyBehaviorKeys.Undo)
               .RegisterBehavior<ValueValidationSourceBehavior<TValue>>(PropertyBehaviorKeys.ValueValidationSource)
               .RegisterBehavior<PropertyChangedNotifierBehavior<TValue>>(PropertyBehaviorKeys.ChangeNotifier)
               .RegisterBehavior<PropertyDescriptorProviderBehavior>(PropertyBehaviorKeys.PropertyDescriptorProvider);
         }
      }

      protected class SimplePropertyProvider : PropertyProvider, ISimplePropertyProvider {
         public virtual BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>()
            where TOwnerVM : IViewModel {
            return GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>();
         }

         public virtual BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>()
            where TOwnerVM : IViewModel {
            return GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>();
         }
      }

      protected class ViewModelPropertyProvider : PropertyProvider, IChildProvider {
         public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel {

            return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
         }

         public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

            return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
         }

         protected virtual new BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>()
            where TValue : IViewModel {

            return base
               .GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>()
               .RegisterBehavior<ViewModelInitializerBehavior<TValue>>(PropertyBehaviorKeys.ValueInitializer)
               .RegisterBehavior<LazyRefreshBehavior>(PropertyBehaviorKeys.LazyRefresh)
               .RegisterBehavior<ViewModelPropertyDescendantsValidatorBehavior<TValue>>(PropertyBehaviorKeys.DescendantsValidator)
               .RegisterBehavior<ServiceLocatorValueFactoryBehavior<TValue>>(PropertyBehaviorKeys.ValueFactory);
         }
      }

      protected class CollectionPropertyFactoryProvider : PropertyProvider, IChildProvider {
         public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel {

            return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>();
         }

         public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
            where TOwnerVM : IViewModel
            where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

            return GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>()
               .RegisterBehavior<SynchronizerCollectionBehavior<TChildVM, TChildSource>>(CollectionPropertyBehaviorKeys.Synchronizer)
               .RegisterBehavior<ServiceLocatorValueFactoryBehavior<TChildVM>>(CollectionPropertyBehaviorKeys.ItemFactory);
         }

         protected virtual new BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TChildVM, TSourceObject>()
            where TChildVM : IViewModel {

            return base
               .GetFactoryWithCommonBehaviors<TOwnerVM, IVMCollection<TChildVM>, TSourceObject>()
               .RegisterBehavior<ItemInitializerBehavior<TChildVM>>(PropertyBehaviorKeys.ValueInitializer)
               .RegisterBehavior<LazyRefreshBehavior>(PropertyBehaviorKeys.LazyRefresh)
               .RegisterBehavior<UndoCollectionModifcationBehavior<TChildVM>>(PropertyBehaviorKeys.Undo)
               .RegisterBehavior<CollectionValidationSourceBehavior<TChildVM>>(PropertyBehaviorKeys.ValueValidationSource)
               .RegisterBehavior<ChangeNotifierCollectionBehavior<TChildVM>>(PropertyBehaviorKeys.ChangeNotifier)
               .RegisterBehavior<CollectionFactoryBehavior<TChildVM>>(PropertyBehaviorKeys.ValueFactory);
         }
      }

      protected class CommandPropertyProvider : SimplePropertyProvider {
         protected override BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>() {
            return base
               .GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>()
               .RegisterBehavior<WaitCursorBehavior>(CommandPropertyBehaviorKeys.WaitCursor)
               .RegisterBehavior<WaitCursorBehavior>(CommandPropertyBehaviorKeys.CommandExecutor)
               .RegisterBehavior<CommandAccessorBehavior>(PropertyBehaviorKeys.ValueAccessor);
         }
      }

      protected class ViewModelProvider : IViewModelProvider {
         public virtual BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
            return new BehaviorFactory()
               .RegisterBehavior<LoadOrderBehavior>(ViewModelBehaviorKeys.LoadOrderController)
               .RegisterBehavior<ViewModelValidationSourceBehavior>(ViewModelBehaviorKeys.ViewModelValidationSource)
               .RegisterBehavior<ValidatorExecutorBehavior>(ViewModelBehaviorKeys.ValidationExecutor)
               .RegisterBehavior<TypeDescriptorBehavior>(ViewModelBehaviorKeys.TypeDescriptorProvider)
               .RegisterBehavior<UndoRootBehavior>(ViewModelBehaviorKeys.UndoRoot);
         }
      }
   }
}
