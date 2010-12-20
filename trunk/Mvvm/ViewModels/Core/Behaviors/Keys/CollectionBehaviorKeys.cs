namespace Inspiring.Mvvm.ViewModels.Core {

   public static class CollectionBehaviorKeys {
      public static readonly BehaviorKey DescriptorSetter = new BehaviorKey("DescriptorSetter");

      public static readonly BehaviorKey ParentSetter = new BehaviorKey("ParentSetter");

      public static readonly BehaviorKey ChangeNotifier = new BehaviorKey("ChangeNotifier");

      public static readonly BehaviorKey Populator = new BehaviorKey("Populator");

      public static readonly BehaviorKey SourceAccessor = new BehaviorKey("SourceAccessor");

      public static readonly BehaviorKey ViewModelFactory = new BehaviorKey("ViewModelFactory");
   }
}
