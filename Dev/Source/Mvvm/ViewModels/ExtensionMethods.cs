namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ExtensionMethods {
      /// <summary>
      ///   Initializes the behavior chain for the given VM descriptor and 
      ///   optionally VM property.
      /// </summary>
      public static void Initialize(
         this BehaviorChain chain,
         VMDescriptorBase descriptor,
         IVMPropertyDescriptor property = null
      ) {
         var context = new BehaviorInitializationContext(descriptor, property);

         chain.TryCall<IBehaviorInitializationBehavior>(x =>
            x.Initialize(context)
         );
      }

      public static void InitializeNext(this Behavior behavior, BehaviorInitializationContext context) {
         IBehaviorInitializationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Initialize(context);
         }
      }

      public static void InitializeValueNext(this Behavior behavior, IBehaviorContext context) {
         IValueInitializerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.InitializeValue(context);
         }
      }

      public static TValue GetValidatedValueNext<TValue>(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IValidatedValueAccessorBehavior<TValue> validatedValueAccessor;
         bool containsValidatedValueAccessor = behavior.TryGetBehavior(out validatedValueAccessor);

         if (containsValidatedValueAccessor) {
            return validatedValueAccessor.GetValidatedValue(context);
         } else {
            TValue unvalidatedValue = behavior.GetValueNext<TValue>(context);
            return unvalidatedValue;
         }
      }

      public static TValue GetValueNext<TValue>(this Behavior behavior, IBehaviorContext context) {
         return behavior
            .GetNextBehavior<IValueAccessorBehavior<TValue>>()
            .GetValue(context);
      }

      public static void SetValueNext<TValue>(this Behavior behavior, IBehaviorContext context, TValue value) {
         behavior
            .GetNextBehavior<IValueAccessorBehavior<TValue>>()
            .SetValue(context, value);
      }

      public static object GetValueNext(this Behavior behavior, IBehaviorContext context) {
         return behavior
            .GetNextBehavior<IUntypedValueGetterBehavior>()
            .GetValue(context);
      }

      public static void SetValueNext(this Behavior behavior, IBehaviorContext context, object value) {
         behavior
            .GetNextBehavior<IUntypedValueSetterBehavior>()
            .SetValue(context, value);
      }

      public static object GetDisplayValueNext(this Behavior behavior, IBehaviorContext context) {
         return behavior
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .GetDisplayValue(context);
      }

      public static void SetDisplayValueNext(this Behavior behavior, IBehaviorContext context, object value) {
         behavior
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .SetDisplayValue(context, value);
      }

      //public static void OnSelfChangedNext(
      //   this Behavior behavior,
      //   IBehaviorContext context,
      //   ChangeArgs args
      //) {
      //   ViewModelBehavior next;
      //   if (behavior.TryGetBehavior(out next)) {
      //      next.OnSelfChanged(context, args);
      //   }
      //}

      //public static void OnChildChangedNext(
      //   this Behavior behavior,
      //   IBehaviorContext context,
      //   ChangeArgs args,
      //   InstancePath changedChildPath
      //) {
      //   ViewModelBehavior next;
      //   if (behavior.TryGetBehavior(out next)) {
      //      next.OnChildChanged(context, args, changedChildPath);
      //   }
      //}

      public static void OnChangedNext(
         this Behavior behavior,
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnChanged(context, args, changedPath);
         }
      }

      public static void HandleChangedNext(
         this Behavior behavior,
         IBehaviorContext context,
         ChangeArgs args
      ) {
         IChangeHandlerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.HandleChange(context, args);
         }
      }

      public static void OnValidatingNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationArgs args
      ) {
         ViewModelBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.OnValidating(context, args);
         }
      }

      public static void HandleChangeNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         CollectionChangedArgs<TItemVM> args
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.HandleChange(context, args);
         }
      }

      public static void CollectionPopulatetNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.CollectionPopulated(context, collection, previousItems);
         }
      }

      public static void ItemInsertedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemInserted(context, collection, item, index);
         }
      }

      public static void ItemRemovedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemRemoved(context, collection, item, index);
         }
      }

      public static void ItemSetNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM previousItem,
         TItemVM item,
         int index
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.ItemSet(context, collection, previousItem, item, index);
         }
      }

      public static void ItemsClearedNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         IVMCollection<TItemVM> collection,
         TItemVM[] previousItems
      ) where TItemVM : IViewModel {
         IModificationCollectionBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.CollectionCleared(context, collection, previousItems);
         }
      }

      public static void UpdatePropertyFromSourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManualUpdateBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdatePropertyFromSource(context);
         }
      }

      public static void UpdatePropertySourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManualUpdateBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdatePropertySource(context);
         }
      }

      public static void UpdateFromSourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManualUpdateCoordinatorBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateFromSource(context);
         }
      }

      public static void UpdateFromSourceNext(
         this Behavior behavior,
         IBehaviorContext context,
         IVMPropertyDescriptor property
      ) {
         IManualUpdateCoordinatorBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateFromSource(context, property);
         }
      }

      public static void UpdateSourceNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IManualUpdateCoordinatorBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateSource(context);
         }
      }

      public static void UpdateSourceNext(
         this Behavior behavior,
         IBehaviorContext context,
         IVMPropertyDescriptor property
      ) {
         IManualUpdateCoordinatorBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.UpdateSource(context, property);
         }
      }

      public static void RevalidateNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationContext validationContext,
         ValidationMode mode
      ) {
         IRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context, validationContext, mode);
         }
      }

      public static void RevalidateNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context);
         }
      }

      public static void RevalidateNext(
         this Behavior behavior,
         IBehaviorContext context,
         CollectionResultCache cache
      ) {
         IRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context, cache);
         }
      }

      public static void RevalidateDescendantsNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationContext validationContext,
         ValidationScope scope,
         ValidationMode mode
      ) {
         IDescendantValidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.RevalidateDescendants(context, validationContext, scope, mode);
         }
      }

      public static ValidationResult GetValidationStateNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IValidationStateProviderBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.GetValidationState(context) :
            ValidationResult.Valid;
      }

      public static ValidationResult GetDescendantsValidationStateNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IValidationStateProviderBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.GetDescendantsValidationState(context) :
            ValidationResult.Valid;
      }

      public static bool IsLoadedNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IIsLoadedIndicatorBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.IsLoaded(context) :
            true;
      }

      public static void RefreshNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IRefreshBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context);
         }
      }

      public static void PopulateNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IPopulationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Populate(context);
         }
      }

      public static void ExecuteNext(
         this Behavior behavior,
         IBehaviorContext context,
         object parameter
      ) {
         ICommandExecuteBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Execute(context, parameter);
         }
      }

      public static bool CanExecuteNext(
         this Behavior behavior,
         IBehaviorContext context,
         object parameter
      ) {
         ICommandCanExecuteBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.CanExecute(context, parameter) :
            true;
      }

      // TODO: What about naming conflicts between VM Behaviors and Property behaviors?
      public static void ViewModelRefreshNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IRefreshControllerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context);
         }
      }

      // TODO: What about naming conflicts between VM Behaviors and Property behaviors?
      public static void ViewModelRefreshNext(
         this Behavior behavior,
         IBehaviorContext context,
         IVMPropertyDescriptor property
      ) {
         IRefreshControllerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context, property);
         }
      }

      public static ValidationResult ValidateNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationRequest request
      ) {
         IValidationExecutorBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.Validate(context, request) :
            ValidationResult.Valid;
      }

      public static void HandlePropertyChangedNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IHandlePropertyChangedBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.HandlePropertyChanged(context);
         }
      }

      public static VMDescriptorBase GetItemDescriptor(this IVMCollection collection) {
         return collection
            .Behaviors
            .GetNextBehavior<IItemDescriptorProviderCollectionBehavior>()
            .ItemDescriptor;
      }

      public static ValidationResult ExecuteValidationRequest(this IViewModel requestTarget, ValidationRequest request) {
         IValidationExecutorBehavior executor;

         bool hasExecutors = requestTarget
            .Descriptor
            .Behaviors
            .TryGetBehavior(out executor);

         if (!hasExecutors) {
            return ValidationResult.Valid;
         }

         return executor.Validate(requestTarget.GetContext(), request);
      }
   }
}
