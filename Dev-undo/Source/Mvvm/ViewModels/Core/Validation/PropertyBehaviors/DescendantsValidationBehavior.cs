namespace Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors {
   using System;

   internal sealed class DescendantsValidationBehaviorNew<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IRefreshBehavior,
      IValueInitializerBehavior,
      IDescendantValidationBehavior {

      private FieldDefinitionGroup StateGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<State> _state;

      public void Initialize(BehaviorInitializationContext context) {
         _state = new DynamicFieldAccessor<State>(context, StateGroup);
         this.InitializeNext(context);
      }

      public void RevalidateDescendants(IBehaviorContext context, ValidationContext validationContext, ValidationScope scope, ValidationMode mode) {
         State s = GetState(context);

         switch (s.Type) {
            case StateType.Unvalidated:
               break;
            case StateType.Validated:
               break;
            case StateType.ValidateOnFirstAccess:
               break;
            default:
               break;
         }

         throw new NotImplementedException();
      }

      public void InitializeValue(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      public void Refresh(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      private State GetState(IBehaviorContext context) {
         return _state.GetWithDefault(context, State.Unvalidated());
      }

      private void TransitionToValidated(IBehaviorContext context, ValidationScope scope) {

         TransitionTo(context, State.Validated(scope));
      }

      private void TransitionTo(IBehaviorContext context, State state) {
         _state.Set(context, state);
      }

      private enum StateType {
         Unvalidated,
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

         public static State Unvalidated() {
            return new State { _type = StateType.Unvalidated };
         }

         public static State Validated(ValidationScope scope) {
            return new State { _type = StateType.Validated, _scope = scope };
         }

         public static State ValidateOnFirstAccess(ValidationScope scope) {
            return new State { _type = StateType.Validated, _scope = scope };
         }
      }
   }
}
