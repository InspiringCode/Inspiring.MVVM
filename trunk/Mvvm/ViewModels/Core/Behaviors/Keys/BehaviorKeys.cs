namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorKeys {

      public static readonly BehaviorKey TypeDescriptor = new BehaviorKey("TypeDescriptor");

      public static readonly BehaviorKey InvalidDisplayValueCache = new BehaviorKey("InvalidDisplayValueCache");
      public static readonly BehaviorKey DisplayValueAccessor = new BehaviorKey("DisplayValueAccessor");
      public static readonly BehaviorKey Validator = new BehaviorKey("Validator");
      public static readonly BehaviorKey PropertyChangedTrigger = new BehaviorKey("PropertyChangedTrigger");
      public static readonly BehaviorKey PreValidationValueCache = new BehaviorKey("PreValidationValueCache");

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
      public static readonly BehaviorKey SourceAccessor = new BehaviorKey("SourceAccessor");


      public static readonly BehaviorKey CollectionFactory = new BehaviorKey("CollectionFactory");

      public static readonly BehaviorKey CollectionInstanceCache = new BehaviorKey("CollectionInstanceCache");

      public static readonly BehaviorKey CollectionPopulator = new BehaviorKey("CollectionPopulator");

      public static readonly BehaviorKey ValueCache = new BehaviorKey("ValueCache");

      public static readonly BehaviorKey ViewModelAccessor = new BehaviorKey("ViewModelAccessor");

      public static readonly BehaviorKey ViewModelPropertyInitializer = new BehaviorKey("ViewModelPropertyInitializer");

      public static readonly BehaviorKey ParentInitializer = new BehaviorKey("ParentInitializer");

      public static readonly BehaviorKey ParentSetter = new BehaviorKey("ParentSetter");

      public static readonly BehaviorKey ViewModelFactory = new BehaviorKey("ViewModelFactory");
   }
}
