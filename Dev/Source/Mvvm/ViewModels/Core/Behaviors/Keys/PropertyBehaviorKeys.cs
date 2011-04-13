namespace Inspiring.Mvvm.ViewModels.Core {
   public class PropertyBehaviorKeys : PropertyBehaviorKeysBase {
      public static readonly BehaviorKey LazyRefresh = Key(() => LazyRefresh);

      public static readonly BehaviorKey ValueAccessor = Key(() => ValueAccessor);


      public static readonly BehaviorKey ValueInitializer = Key(() => ValueInitializer);

      public static readonly BehaviorKey ParentSetter = Key(() => ParentSetter);


      public static readonly BehaviorKey ValueCacheNew = Key(() => ValueCacheNew);

      public static readonly BehaviorKey ViewModelSourceSetter = Key(() => ViewModelSourceSetter);

      public static readonly BehaviorKey DescendantsValidator = Key(() => DescendantsValidator);


      public static readonly BehaviorKey InvalidDisplayValueCache = new BehaviorKey("InvalidDisplayValueCache");
      public static readonly BehaviorKey Validator = new BehaviorKey("Validator");

      public static readonly BehaviorKey PropertyValueCache = new BehaviorKey("PropertyValueCache");
      public static readonly BehaviorKey ManualUpdateBehavior = new BehaviorKey("ManualUpdateBehavior");

      /// <summary>
      ///   A <see cref="IValueAccessorBehavior"/> gets and sets the actual 
      ///   property value. The generic type argument is the type of the property
      ///   value.
      /// </summary>
      public static readonly BehaviorKey PropertyValueAcessor = new BehaviorKey("PropertyValueAcessor");

      /// <summary>
      ///   
      /// </summary>


      public static readonly BehaviorKey CollectionFactory = new BehaviorKey("CollectionFactory");

      public static readonly BehaviorKey CollectionInstanceCache = new BehaviorKey("CollectionInstanceCache");

      public static readonly BehaviorKey CollectionPopulator = new BehaviorKey("CollectionPopulator");

      public static readonly BehaviorKey ValueCache = new BehaviorKey("ValueCache");

      public static readonly BehaviorKey ViewModelAccessor = new BehaviorKey("ViewModelAccessor");

      public static readonly BehaviorKey ViewModelPropertyInitializer = new BehaviorKey("ViewModelPropertyInitializer");

      public static readonly BehaviorKey ViewModelFactory = new BehaviorKey("ViewModelFactory");

      //public static readonly BehaviorKey DescendantValidator = new BehaviorKey("DescendantValidator");

      public static readonly BehaviorKey ManualUpdateCoordinator = new BehaviorKey("ManualUpdateCoordinator");

      public static readonly BehaviorKey IsLoadedIndicator = new BehaviorKey("IsLoadedIndicator");

      public static readonly BehaviorKey RefreshBehavior = new BehaviorKey("RefreshBehavior");

      public static readonly BehaviorKey CommandExecutor = new BehaviorKey("CommandExecutor");

      public static readonly BehaviorKey CommandFactory = new BehaviorKey("CommandFactory");

      public static readonly BehaviorKey WaitCursor = new BehaviorKey("WaitCursor");

   }
}
