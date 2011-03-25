namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   internal interface IValidatorBuilderOperationProvider {
      ValidatorBuilderOperation GetOperation();
   }

   internal sealed class ValidatorBuilderOperation : IValidatorBuilderOperationProvider {
      public ValidatorBuilderOperation(VMDescriptorConfiguration config) {
         Config = config;
         BuildActions = new Stack<Action>();
         ActionArgs = new Stack<IValidator>();
         Path = PathDefinition.Empty;
      }

      public VMDescriptorConfiguration Config { get; private set; }
      public Stack<Action> BuildActions { get; private set; }
      public Stack<IValidator> ActionArgs { get; private set; }
      public PathDefinition Path { get; set; }

      public void EnableViewModelValidationSourceBehavior() {
         Config
            .ViewModelConfiguration
            .Enable(BehaviorKeys.Validator); // TODO: Use correct key.

         throw new NotImplementedException();
      }

      public void EnablePropertyValidationSourceBehavior(IVMPropertyDescriptor property) {
         Config
            .PropertyConfigurations[property]
            .Enable(BehaviorKeys.Validator); // TODO: Use correct key.

         throw new NotImplementedException();
      }

      public ValidatorBuilderOperation GetOperation() {
         return this;
      }
   }

   internal sealed class ValidatorBuilderOperationCollection : IValidatorBuilderOperationProvider {
      private readonly VMDescriptorConfiguration _config;
      private readonly List<ValidatorBuilderOperation> _operations = new List<ValidatorBuilderOperation>();

      public ValidatorBuilderOperationCollection(VMDescriptorConfiguration config) {
         _config = config;
      }

      public ValidatorBuilderOperation GetOperation() {
         var op = new ValidatorBuilderOperation(_config);
         _operations.Add(op);
         return op;
      }
   }
}
