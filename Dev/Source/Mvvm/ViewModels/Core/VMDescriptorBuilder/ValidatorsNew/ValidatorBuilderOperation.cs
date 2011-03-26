namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.ValidatorsNew {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   internal sealed class ValidatorBuilderOperation : IValidatorBuilderOperationProvider {
      public ValidatorBuilderOperation(
         VMDescriptorBase descriptor,
         VMDescriptorConfiguration config
      ) {
         Descriptor = descriptor;
         Config = config;
         Path = PathDefinition.Empty;
         BuildActions = new Stack<Action>();
         ActionArgs = new Stack<IValidator>();
      }

      public VMDescriptorBase Descriptor { get; private set; }
      public VMDescriptorConfiguration Config { get; private set; }
      public PathDefinition Path { get; set; }
      public Stack<Action> BuildActions { get; private set; }
      public Stack<IValidator> ActionArgs { get; private set; }

      public void EnableViewModelValidationSourceBehavior() {
         Config
            .ViewModelConfiguration
            .Enable(PropertyBehaviorKeys.Validator); // TODO: Use correct key.

         throw new NotImplementedException();
      }

      public void EnablePropertyValidationSourceBehavior(IVMPropertyDescriptor property) {
         Config
            .PropertyConfigurations[property]
            .Enable(PropertyBehaviorKeys.Validator); // TODO: Use correct key.

         throw new NotImplementedException();
      }

      public ValidatorBuilderOperation GetOperation() {
         return this;
      }

      public void Execute() {

      }
   }

   internal sealed class ValidatorBuilderOperationCollection : IValidatorBuilderOperationProvider {
      private readonly VMDescriptorBase _descriptor;
      private readonly VMDescriptorConfiguration _config;
      private readonly List<ValidatorBuilderOperation> _operations = new List<ValidatorBuilderOperation>();

      public ValidatorBuilderOperationCollection(
         VMDescriptorBase descriptor,
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
   }
}
