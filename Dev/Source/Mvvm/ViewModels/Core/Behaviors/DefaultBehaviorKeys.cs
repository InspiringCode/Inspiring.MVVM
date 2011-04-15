namespace Inspiring.Mvvm.ViewModels.Core {

   public class PropertyBehaviorKeys : BehaviorKeys {
      public static readonly BehaviorKey DisplayValueAccessor = Key(() => DisplayValueAccessor);
      public static readonly BehaviorKey UntypedValueAccessor = Key(() => UntypedValueAccessor);
      public static readonly BehaviorKey Undo = Key(() => Undo);
      public static readonly BehaviorKey ValueValidationSource = Key(() => ValueValidationSource);
      public static readonly BehaviorKey ChangeNotifier = Key(() => ChangeNotifier);
      public static readonly BehaviorKey ValueAccessor = Key(() => ValueAccessor);
      public static readonly BehaviorKey SourceAccessor = Key(() => SourceAccessor);
      public static readonly BehaviorKey PropertyDescriptorProvider = Key(() => PropertyDescriptorProvider);

      public static readonly BehaviorKey LazyRefresh = Key(() => LazyRefresh);
      public static readonly BehaviorKey DescendantsValidator = Key(() => DescendantsValidator);
      public static readonly BehaviorKey ValueInitializer = Key(() => ValueInitializer);
      public static readonly BehaviorKey ValueFactory = Key(() => ValueFactory);
   }

   public class CollectionPropertyBehaviorKeys : PropertyBehaviorKeys {
      public static readonly BehaviorKey Synchronizer = Key(() => Synchronizer);
   }

   public class CommandPropertyBehaviorKeys : PropertyBehaviorKeys {
      public static readonly BehaviorKey CommandExecutor = Key(() => CommandExecutor);
      public static readonly BehaviorKey WaitCursor = Key(() => WaitCursor);
   }

   public class ViewModelBehaviorKeys : BehaviorKeys {
      public static readonly BehaviorKey LoadOrderController = Key(() => LoadOrderController);
      public static readonly BehaviorKey ViewModelValidationSource = Key(() => ViewModelValidationSource);
      public static readonly BehaviorKey ValidationExecutor = Key(() => ValidationExecutor);
      public static readonly BehaviorKey UndoRoot = Key(() => UndoRoot);
      public static readonly BehaviorKey TypeDescriptorProvider = Key(() => TypeDescriptorProvider);
   }

   public class DefaultBehaviorChainTemplateKeys : BehaviorChainTemplateKeys {
      public static readonly BehaviorChainTemplateKey Property = Key(() => Property);
      public static readonly BehaviorChainTemplateKey ViewModelProperty = Key(() => ViewModelProperty);
      public static readonly BehaviorChainTemplateKey CollectionProperty = Key(() => CollectionProperty);
      public static readonly BehaviorChainTemplateKey CommandProperty = Key(() => CommandProperty);
      public static readonly BehaviorChainTemplateKey ViewModel = Key(() => ViewModel);
   }
}
