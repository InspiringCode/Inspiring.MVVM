namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal abstract class DescendantsValidatorBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IRefreshBehavior,
      IValueInitializerBehavior,
      IDescendantValidationBehavior,
      IDescendantsValidationResultProviderBehavior {

      private FieldDefinitionGroup StateGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<State> _state;

      protected DescendantsValidatorBehavior() {
      }

      public void Initialize(BehaviorInitializationContext context) {
         _state = new DynamicFieldAccessor<State>(context, StateGroup);
         this.InitializeNext(context);
      }

      public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {
         State s = GetState(context);

         switch (s.Type) {
            case StateType.Initial:
               bool isUnloaded = !this.IsLoadedNext(context);
               bool validateOnlyLoadedDescendants = scope == ValidationScope.SelfAndLoadedDescendants;

               if (isUnloaded && validateOnlyLoadedDescendants) {
                  TransitionTo(context, State.ValidateOnFirstAccess(scope));
               } else {
                  TransitionToValidated(context, scope);
               }

               break;
            case StateType.Validated:
               TransitionToValidated(context, scope);
               break;
            case StateType.ValidateOnFirstAccess:
               if (scope == ValidationScope.SelfAndAllDescendants) {
                  TransitionToValidated(context, ValidationScope.SelfAndAllDescendants);
               }
               break;
         }

         this.RevalidateDescendantsNext(context, scope);
      }

      public void InitializeValue(IBehaviorContext context) {
         this.InitializeValueNext(context);

         State s = GetState(context);
         switch (s.Type) {
            case StateType.ValidateOnFirstAccess:
               TransitionToValidated(context, s.Scope);
               break;
         }
      }

      public void Refresh(IBehaviorContext context) {
         this.RefreshNext(context);

         State s = GetState(context);
         switch (s.Type) {
            case StateType.Validated:
               TransitionToValidated(context, s.Scope);
               break;
         }
      }

      public ValidationResult GetDescendantsValidationResult(IBehaviorContext context) {
         var nextResult = this.GetDescendantsValidationResultNext(context);

         // Note: We cannot use our 'State' to determin wheter we should get
         //       the descendant validaton state because we cannot possibly
         //       know if the descendants were already validated. A SetValue
         //       on a descendant for example validates a collection but our
         //       behavior has not chance to detect this, so our state is 
         //       still 'Initial' until some actually calss Revalidate 
         //       manually.
         bool isLoaded = this.IsLoadedNext(context);

         if (isLoaded) {
            var result = GetDescendantsValidationResultCore(context);
            return ValidationResult.Join(result, nextResult);
         } else {
            return nextResult;
         }
      }

      protected abstract void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope);

      protected abstract ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context);

      private void TransitionToValidated(IBehaviorContext context, ValidationScope scope) {
         // Note: We need to transition FIRST, because a revalidate may trigger
         // a lazy load which calls InitializeValue. The validated state ignores
         // this event to avoid revalidating twice.
         TransitionTo(context, State.Validated(scope));
         RevalidateDescendantsCore(context, scope);
      }

      private void TransitionTo(IBehaviorContext context, State state) {
         _state.Set(context, state);
      }

      private State GetState(IBehaviorContext context) {
         return _state.GetWithDefault(context, State.Initial());
      }

      private enum StateType {
         Initial,
         Validated,
         ValidateOnFirstAccess
      }

      private class State {
         private StateType _type;
         private Nullable<ValidationScope> _scope;

         public StateType Type {
            get { return _type; }
         }

         public ValidationScope Scope {
            get { return _scope.Value; }
         }

         public static State Initial() {
            return new State { _type = StateType.Initial };
         }

         public static State Validated(ValidationScope scope) {
            return new State { _type = StateType.Validated, _scope = scope };
         }

         public static State ValidateOnFirstAccess(ValidationScope scope) {
            return new State { _type = StateType.ValidateOnFirstAccess, _scope = scope };
         }
      }
   }
}
