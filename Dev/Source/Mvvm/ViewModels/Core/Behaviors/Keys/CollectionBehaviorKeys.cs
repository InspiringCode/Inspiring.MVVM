namespace Inspiring.Mvvm.ViewModels.Core {

   public class CollectionBehaviorKeys : BehaviorKeys {
      public static readonly BehaviorKey Undo = Key(() => Undo);

      public static readonly BehaviorKey CollectionValidationSource = Key(() => CollectionValidationSource);

      public static readonly BehaviorKey DescriptorSetter = new BehaviorKey("DescriptorSetter");

      public static readonly BehaviorKey ParentSetter = new BehaviorKey("ParentSetter");

      public static readonly BehaviorKey ChangeNotifier = new BehaviorKey("ChangeNotifier");

      public static readonly BehaviorKey SourceSynchronizer = new BehaviorKey("SourceSynchronizer");

      public static readonly BehaviorKey Populator = new BehaviorKey("Populator");

      public static readonly BehaviorKey SourceAccessor = new BehaviorKey("SourceAccessor");

      public static readonly BehaviorKey ViewModelFactory = new BehaviorKey("ViewModelFactory");
   }
}
