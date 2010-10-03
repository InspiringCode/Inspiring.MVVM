namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class ValidationBuilder<TVM> : IValidationBuilder<TVM> where TVM : ViewModel {
      private BehaviorConfigurationDictionary _configs;

      public ValidationBuilder(BehaviorConfigurationDictionary configs) {
         Contract.Requires(configs != null);
         _configs = configs;
      }

      public IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property) {
         BehaviorConfiguration config = _configs.GetConfiguration(property);
         return new ValidationBuilder<TVM, TValue>(config);
      }
   }

   internal sealed class ValidationBuilder<TVM, TValue> : IValidationBuilder<TVM, TValue> where TVM : ViewModel {
      private BehaviorConfiguration _config;

      public ValidationBuilder(BehaviorConfiguration config) {
         Contract.Requires(config != null);
         _config = config;
      }

      public void Custom(Func<TVM, TValue, ValidationResult> validation) {
         _config.Enable(VMBehaviorKey.Validator);
         _config.Enable(VMBehaviorKey.InvalidDisplayValueCache);

         _config.Configure(VMBehaviorKey.Validator, (ValidationBehavior<TValue> behavior) => {
            behavior.Add((ValidationParameter<TValue> p) => {
               return validation((TVM)p.VM, p.Value);
            });
         });
      }

      public IValidationBuilder<TParentVM, TVM, TValue> WithParent<TParentVM>() where TParentVM : ViewModel {
         return new ValidationBuilder<TParentVM, TVM, TValue>(_config);
      }
   }

   internal sealed class ValidationBuilder<TParentVM, TVM, TValue> : IValidationBuilder<TParentVM, TVM, TValue>
      where TVM : ViewModel
      where TParentVM : ViewModel {
      private BehaviorConfiguration _config;

      public ValidationBuilder(BehaviorConfiguration config) {
         Contract.Requires(config != null);
         _config = config;
      }

      public void Custom(Func<TParentVM, TVM, TValue, ValidationResult> validation) {
         _config.Enable(VMBehaviorKey.Validator);
         _config.Enable(VMBehaviorKey.InvalidDisplayValueCache);

         _config.Configure(VMBehaviorKey.Validator, (ValidationBehavior<TValue> behavior) => {
            behavior.Add((ValidationParameter<TValue> p) => {
               TParentVM parent = p.VM.Parent as TParentVM;
               return parent != null ?
                  validation(parent, (TVM)p.VM, p.Value) :
                  ValidationResult.Success();
            });
         });
      }
   }
}
