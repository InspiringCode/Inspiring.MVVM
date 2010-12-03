namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorKeys {
      public static readonly BehaviorKey TypeDescriptor = new BehaviorKey("TypeDescriptor");

      public static readonly BehaviorKey InvalidDisplayValueCache = new BehaviorKey("InvalidDisplayValueCache");
      public static readonly BehaviorKey DisplayValueAccessor = new BehaviorKey("DisplayValueAccessor");
      public static readonly BehaviorKey Validator = new BehaviorKey("Validator");
      public static readonly BehaviorKey PropertyChangedTrigger = new BehaviorKey("PropertyChangedTrigger");
      public static readonly BehaviorKey PropertyValueCache = new BehaviorKey("PropertyValueCache");
      public static readonly BehaviorKey PropertyValueAcessor = new BehaviorKey("PropertyValueAcessor");
      public static readonly BehaviorKey ManualUpdateBehavior = new BehaviorKey("ManualUpdateBehavior");
      public static readonly BehaviorKey SourceValueAccessor = new BehaviorKey("SourceValueAccessor");

   }
}
