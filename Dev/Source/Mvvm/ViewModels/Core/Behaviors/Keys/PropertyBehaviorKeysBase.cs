namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class PropertyBehaviorKeysBase : BehaviorKeys {
      public static readonly BehaviorKey DisplayValueAccessor = Key(() => DisplayValueAccessor);
      public static readonly BehaviorKey UntypedValueAccessor = Key(() => UntypedValueAccessor);
      public static readonly BehaviorKey Undo = Key(() => Undo);
      public static readonly BehaviorKey ValueValidationSource = Key(() => ValueValidationSource);
      public static readonly BehaviorKey ChangeNotifier = Key(() => ChangeNotifier);
      public static readonly BehaviorKey PropertyDescriptorProvider = Key(() => PropertyDescriptorProvider);
      public static readonly BehaviorKey SourceAccessor = Key(() => SourceAccessor);
   }
}
