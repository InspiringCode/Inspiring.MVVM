namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class ValidatorBuilderOperation : IValidatorBuilderOperationProvider {
      public ValidatorBuilderOperation(
         IVMDescriptor descriptor,
         VMDescriptorConfiguration config
      ) {
         Descriptor = descriptor;
         Config = config;
         Path = PathDefinition.Empty;
         BuildActions = new Stack<Action>();
         ActionArgs = new Stack<IValidator>();
      }

      public IVMDescriptor Descriptor { get; private set; }
      public VMDescriptorConfiguration Config { get; private set; }
      public PathDefinition Path { get; set; }
      public Stack<Action> BuildActions { get; private set; }
      public Stack<IValidator> ActionArgs { get; private set; }

      public void EnableViewModelValidationSourceBehavior() {
         Config
            .ViewModelConfiguration
            .Enable(ViewModelBehaviorKeys.ViewModelValidationSource);
      }

      public void EnablePropertyValidationSourceBehavior(IVMPropertyDescriptor property) {
         Config
            .PropertyConfigurations[property]
            .Enable(PropertyBehaviorKeys.ValueValidationSource);
      }

      public ValidatorBuilderOperation GetOperation() {
         return this;
      }

      public void Execute() {
         if (BuildActions.Any()) {
            while (BuildActions.Any()) {
               var action = BuildActions.Pop();
               action();
            }

            Contract.Assert(ActionArgs.Count == 1);

            AddValidator(ActionArgs.Single());
         }
      }

      private void AddValidator(IValidator validator) {
         Config
            .ViewModelConfiguration
            .ConfigureBehavior<ValidatorExecutorBehavior>(
               ViewModelBehaviorKeys.ValidationExecutor,
               b => b.AddValidator(validator)
            );
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
