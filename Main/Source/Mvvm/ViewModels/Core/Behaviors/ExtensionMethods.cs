namespace Inspiring.Mvvm.ViewModels {
   using System.ComponentModel;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ExtensionMethods {
      /// <summary>
      ///   Initializes the behavior chain for the given VM descriptor and 
      ///   optionally VM property.
      /// </summary>
      public static void Initialize(
         this BehaviorChain chain,
         IVMDescriptor descriptor,
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

      // TODO: Is TryGetBehavior a good idea?
      public static void SetValueNext<TValue>(this Behavior behavior, IBehaviorContext context, TValue value) {
         IValueAccessorBehavior<TValue> b;
         if (behavior.TryGetBehavior(out b)) {
            b.SetValue(context, value);
         }
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

      //public static void OnChangedNext(
      //   this Behavior behavior,
      //   IBehaviorContext context,
      //   ChangeArgs args,
      //   InstancePath changedPath
      //) {
      //   ViewModelBehavior next;
      //   if (behavior.TryGetBehavior(out next)) {
      //      next.OnChanged(context, args, changedPath);
      //   }
      //}

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

      public static void HandleChangeNext<TItemVM>(
         this Behavior behavior,
         IBehaviorContext context,
         CollectionChangedArgs<TItemVM> args
      ) where TItemVM : IViewModel {
         ICollectionChangeHandlerBehavior<TItemVM> next;
         if (behavior.TryGetBehavior(out next)) {
            next.HandleChange(context, args);
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

      public static void BeginValidationNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationController controller
      ) {
         IPropertyRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.BeginValidation(context, controller);
         }
      }

      public static void EndValidationNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IPropertyRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.EndValidation(context);
         }
      }

      public static void PropertyRevalidateNext(this Behavior behavior, IBehaviorContext context) {
         IPropertyRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context);
         }
      }

      public static void ViewModelRevalidateNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationController controller
      ) {
         IViewModelRevalidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Revalidate(context, controller);
         }
      }

      public static void RevalidateDescendantsNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationScope scope
      ) {
         IDescendantValidationBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.RevalidateDescendants(context, scope);
         }
      }

      public static ValidationResult GetValidationResultNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IValidationResultProviderBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.GetValidationResult(context) :
            ValidationResult.Valid;
      }

      public static ValidationResult GetValidationResultNext(
         this Behavior behavior,
         IBehaviorContext context,
         ValidationResultScope scope
      ) {
         IValidationResultAggregatorBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.GetValidationResult(context, scope) :
            ValidationResult.Valid;
      }

      public static ValidationResult GetDescendantsValidationResultNext(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         IDescendantsValidationResultProviderBehavior next;
         return behavior.TryGetBehavior(out next) ?
            next.GetDescendantsValidationResult(context) :
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
         IBehaviorContext context,
         bool executeRefreshDependencies
      ) {
         IRefreshBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context, executeRefreshDependencies);
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
         IBehaviorContext context,
         bool executeRefreshDependencies
      ) {
         IRefreshControllerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context, executeRefreshDependencies);
         }
      }

      // TODO: What about naming conflicts between VM Behaviors and Property behaviors?
      public static void ViewModelRefreshNext(
         this Behavior behavior,
         IBehaviorContext context,
         IVMPropertyDescriptor property,
         bool executeRefreshDependencies
      ) {
         IRefreshControllerBehavior next;
         if (behavior.TryGetBehavior(out next)) {
            next.Refresh(context, property, executeRefreshDependencies);
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

      public static TValue CreateValueNext<TValue>(
         this Behavior behavior,
         IBehaviorContext context
      ) {
         return behavior
            .GetNextBehavior<IValueFactoryBehavior<TValue>>()
            .CreateValue(context);
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

      public static IVMDescriptor GetItemDescriptor(
         this Behavior behavior
      ) {
         return behavior
            .GetNextBehavior<IItemDescriptorProviderBehavior>()
            .ItemDescriptor;
      }

      public static IVMDescriptor GetItemDescriptor(this IVMCollection collection) {
         return collection
            .OwnerProperty
            .Behaviors
            .GetItemDescriptor();
      }

      internal static PropertyDescriptorCollection GetPropertyDescriptors(this IVMDescriptor descriptor) {
         return descriptor
            .Behaviors
            .GetNextBehavior<TypeDescriptorProviderBehavior>()
            .PropertyDescriptors;
      }

      // TODO: Inline this??
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
