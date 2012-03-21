namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal enum BuildActionOrder {
      Validator = 100,
      UserConditions = 200,
      PredefinedConditions = 300,
      Other = 400
   }

   internal sealed class ValidatorBuilderOperation : IValidatorBuilderOperationProvider {
      private readonly Dictionary<BuildActionOrder, Stack<Action>> _buildActions =
         new Dictionary<BuildActionOrder, Stack<Action>>();

      public ValidatorBuilderOperation(
         IVMDescriptor descriptor,
         VMDescriptorConfiguration config
      ) {
         Descriptor = descriptor;
         Config = config;
         Path = PathDefinition.Empty;
         ActionArgs = new Stack<IValidator>();
      }

      public IVMDescriptor Descriptor { get; private set; }
      public VMDescriptorConfiguration Config { get; private set; }
      public PathDefinition Path { get; set; }
      public Stack<IValidator> ActionArgs { get; private set; }

      public void PushGeneralBuildAction(BuildActionOrder order, Action action) {
         Stack<Action> stack = _buildActions
            .EnsureItem(order, () => new Stack<Action>());

         stack.Push(action);
      }

      public void PushValidatorBuildActions(
         IValidator validator,
         ValidationStep step
      ) {
         PushGeneralBuildAction(
            BuildActionOrder.Validator, 
            () => ActionArgs.Push(validator)
         );

         PushConditionBuildAction(
            BuildActionOrder.PredefinedConditions,
            new ValidationStepCondition(step)
         );
      }

      public void PushConditionBuildAction(
         BuildActionOrder order,
         ICondition<ValidationRequest> condition
      ) {
         PushGeneralBuildAction(order, () => {
            IValidator inner = ActionArgs.Pop();

            IValidator conditional = new ConditionalValidator(
               condition,
               inner
            );

            ActionArgs.Push(conditional);
         });
      }

      public void EnableViewModelValidationSourceBehavior() {
         EnableValidationExecutorBehavior();

         Config
            .ViewModelConfiguration
            .Enable(ViewModelBehaviorKeys.ViewModelValidationSource);
      }

      public void EnablePropertyValidationSourceBehavior(IVMPropertyDescriptor property) {
         EnableValidationExecutorBehavior();

         Config
            .PropertyConfigurations[property]
            .Enable(PropertyBehaviorKeys.ValueValidationSource);
      }

      public ValidatorBuilderOperation GetOperation() {
         return this;
      }

      public void Execute() {
         if (!_buildActions.Any()) {
            return;
         }

         PushGeneralBuildAction(BuildActionOrder.PredefinedConditions, () => {
            IValidator inner = ActionArgs.Pop();

            IValidator conditional = new ConditionalValidator(
               new ValidationTargetCondition(Path),
               inner
            );

            ActionArgs.Push(conditional);
         });

         IEnumerable<Stack<Action>> orderedActions = _buildActions
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

         foreach (Stack<Action> actions in orderedActions) {
            while (actions.Any()) {
               var action = actions.Pop();
               action();
            }

         }

         AddValidatorToBehavior(ActionArgs.Single());
      }

      private void AddValidatorToBehavior(IValidator validator) {
         Config
            .ViewModelConfiguration
            .ConfigureBehavior<ValidatorExecutorBehavior>(
               ViewModelBehaviorKeys.ValidationExecutor,
               b => b.AddValidator(validator)
            );
      }

      /// <summary>
      ///   Note: We have to enable the validation executor behavior even if we have NO
      ///   validators defined on a VM because otherwise validation requests would not
      ///   be forwarded to potential parents!
      /// </summary>
      private void EnableValidationExecutorBehavior() {
         Config
            .ViewModelConfiguration
            .Enable(ViewModelBehaviorKeys.ValidationExecutor);
      }
   }

   internal sealed class ValidatorBuilderOperationCollection : IValidatorBuilderOperationProvider {
      private readonly IVMDescriptor _descriptor;
      private readonly VMDescriptorConfiguration _config;
      private readonly List<ValidatorBuilderOperation> _operations = new List<ValidatorBuilderOperation>();

      public ValidatorBuilderOperationCollection(
         IVMDescriptor descriptor,
         VMDescriptorConfiguration config
      ) {
         _descriptor = descriptor;
         _config = config;
      }

      public ValidatorBuilderOperation GetOperation() {
         var op = new ValidatorBuilderOperation(_descriptor, _config);
         _operations.Add(op);
         return op;
      }

      internal void Perform() {
         _operations.ForEach(o => o.Execute());
      }
   }
}
