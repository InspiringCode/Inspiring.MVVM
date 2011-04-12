using Inspiring.Mvvm.ViewModels.Core.Behaviors;
namespace Inspiring.Mvvm.ViewModels.Core {
   public static partial class BehaviorChainTemplateRegistry {
      static BehaviorChainTemplateRegistry() {
         ViewModelBehaviorChainTemplates.RegisterDefaultTemplates();
         //ResetToDefaults();
      }

      public static void ResetToDefaults() {
         //RegisterViewModelTemplate();
         //RegisterPropertyTemplate();
         //RegisterCollectionPropertyTemplate();
         //RegisterCommandPropertyTemplate();
         //RegisterCommandBehaviorsTemplate();
         //RegisterViewModelPropertyTemplate();
         //RegisterCollectionBehaviorsTemplate();
      }

      private static void RegisterViewModelTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModel,
            new BehaviorChainTemplate(ViewModelBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.ManualUpdateCoordinator)
               .Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)
               .Append(ViewModelBehaviorKeys.ViewModelValidationSource, DefaultBehaviorState.Disabled)
               .Append(ViewModelBehaviorKeys.ValidationExecutor, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
               .Append(ViewModelBehaviorKeys.UndoRoot, DefaultBehaviorState.Disabled)
         );
      }

      private static void RegisterPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.InvalidDisplayValueCache, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.RefreshBehavior, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)
               .Append(PropertyBehaviorKeys.PropertyValueCache, DefaultBehaviorState.DisabledWithoutFactory) // TODO!
            //.Append(BehaviorKeys.ManualUpdateBehavior) // TODO: Is this correct?
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCollectionPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.CollectionProperty,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.RefreshBehavior, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.DisabledWithoutFactory)

               .Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)

               .Append(PropertyBehaviorKeys.DescendantsValidator) // TODO: Construction!

               .Append(PropertyBehaviorKeys.IsLoadedIndicator)
               .Append(PropertyBehaviorKeys.CollectionPopulator, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ValueCache)
               .Append(PropertyBehaviorKeys.CollectionFactory, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCommandPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.CommandProperty,
            new BehaviorChainTemplate(PropertyBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.ValueCache)
               .Append(PropertyBehaviorKeys.CommandFactory, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCommandBehaviorsTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.CommandBehaviors,
            new BehaviorChainTemplate(CommandBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.WaitCursor)
               .Append(PropertyBehaviorKeys.CommandExecutor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         );
      }

      private static void RegisterViewModelPropertyTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.ViewModelProperty,
            new BehaviorChainTemplate(ViewModelPropertyBehaviorFactory.Instance)
               .Append(PropertyBehaviorKeys.DisplayValueAccessor)
               .Append(PropertyBehaviorKeys.UntypedValueAccessor)
               .Append(PropertyBehaviorKeys.LazyRefresh)
               .Append(PropertyBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
            // TODO: Rethink.
               .Append(PropertyBehaviorKeys.RefreshBehavior, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ManualUpdateBehavior, DefaultBehaviorState.DisabledWithoutFactory)

               .Append(PropertyBehaviorKeys.DescendantsValidator)

               //.Append(PropertyBehaviorKeys.ValueInitializer)

               .Append(PropertyBehaviorKeys.ParentSetter)

               .Append(PropertyBehaviorKeys.ValueValidationSource, DefaultBehaviorState.Disabled)
               .Append(PropertyBehaviorKeys.Validator, DefaultBehaviorState.Disabled)

               .Append(PropertyBehaviorKeys.PropertyChangedNotifier)


               //.Append(PropertyBehaviorKeys.ViewModelSourceSetter, DefaultBehaviorState.DisabledWithoutFactory)
            //.Append(PropertyBehaviorKeys.ValueCache)
               .Append(PropertyBehaviorKeys.ViewModelAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(PropertyBehaviorKeys.ViewModelFactory)
               .Append(PropertyBehaviorKeys.TypeDescriptor)
         );
      }

      private static void RegisterCollectionBehaviorsTemplate() {
         RegisterTemplate(
            BehaviorChainTemplateKeys.DefaultCollectionBehaviors,
            new BehaviorChainTemplate(CollectionBehaviorFactory.Instance)
               .Append(CollectionBehaviorKeys.DescriptorSetter, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.ParentSetter)
               .Append(CollectionBehaviorKeys.SourceSynchronizer, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.CollectionValidationSource, DefaultBehaviorState.Disabled)
               .Append(CollectionBehaviorKeys.Undo, DefaultBehaviorState.Disabled)
               .Append(CollectionBehaviorKeys.ChangeNotifier)
               .Append(CollectionBehaviorKeys.Populator, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.SourceAccessor, DefaultBehaviorState.DisabledWithoutFactory)
               .Append(CollectionBehaviorKeys.ViewModelFactory, DefaultBehaviorState.Disabled)
         );
      }

      ///// <summary>
      /////   Appends the behavior key with the <see cref="PropertyBehaviorFactory"/>.
      ///// </summary>
      //private static void Append(
      //   BehaviorChainTemplate template,
      //   BehaviorKey key,
      //   bool disabled = false,
      //   bool withoutFactory = false
      //) {
      //   Contract.Requires(withoutFactory ? disabled : true);

      //   if (withoutFactory) {
      //      template.Append(key);
      //   } else {
      //      template.Append(key, new PropertyBehaviorFactory(key), disabled);
      //   }
      //}

      ///// <summary>
      /////   Appends the behavior key with the <see cref="ViewModelBehaviorFactory"/>.
      ///// </summary>
      //private static void AppendViewModel(
      //   BehaviorChainTemplate template,
      //   BehaviorKey key,
      //   bool disabled = false
      //) {
      //   template.Append(key, new ViewModelBehaviorFactory(key), disabled);
      //}
   }
}
