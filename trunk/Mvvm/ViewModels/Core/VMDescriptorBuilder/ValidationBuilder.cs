namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal sealed class ValidationBuilder<TVM> : IValidationBuilder<TVM> where TVM : IViewModel {
      private VMDescriptorConfiguration _configuration;
      private VMDescriptor _descriptor;

      public ValidationBuilder(VMDescriptorConfiguration configuration, VMDescriptor descriptor) {
         Contract.Requires(configuration != null);
         Contract.Requires(descriptor != null);

         _configuration = configuration;
         _descriptor = descriptor;
      }

      public IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property) {
         throw new NotImplementedException();
         //BehaviorConfiguration config = _configuration.GetConfiguration(property);

         //// Enable here, to allow enabling validation without defining a validation
         //// (needed for collection validations).
         //config.Enable(VMBehaviorKey.Validator);
         //config.Enable(VMBehaviorKey.InvalidDisplayValueCache);

         //return new ValidationBuilder<TVM, TValue>(config, property);
      }

      public ICollectionValidationBuilder<TItemVM> CheckCollection<TItemVM>(IVMProperty<VMCollection<TItemVM>> property) where TItemVM : IViewModel {
         throw new NotImplementedException();
         //return new CollectionValidationBuilder<TItemVM>(_configuration, property);
      }

      public void ViewModelValidator(Action<TVM, _ViewModelValidationArgs> validator) {
         Contract.Requires<ArgumentNullException>(validator != null);

         var validatorHolder = _descriptor
            .GetService<ViewModelValidatorHolder>();

         validatorHolder.AddValidator(args => {
            validator((TVM)args.ValidationTarget, args);
         });
      }
   }

   internal sealed class ValidationBuilder<TVM, TValue> : IValidationBuilder<TVM, TValue> where TVM : IViewModel {
      private BehaviorConfiguration _config;
      private IVMProperty _property;

      public ValidationBuilder(BehaviorConfiguration config, IVMProperty property) {
         Contract.Requires(config != null);
         Contract.Requires(property != null);

         _config = config;
         _property = property;
      }

      public void Custom(Action<TVM, TValue, ValidationArgs> validatorCallback) {
         var validator = new DelegateValidator<TVM, TValue>(validatorCallback);

         ViewModelValidationBehavior behavior = null;

         //behavior.AddValidator(validator, ValidationType.PropertyValue, VMPropertyPath.Empty, _property);

         //_config.Configure<PropertyValidationBehavior<TValue>>(VMBehaviorKey.Validator, behavior => {

         //});



         throw new NotImplementedException("TODO2");
         //_config.Configure(VMBehaviorKey.Validator, (ValidationBehavior<TValue> behavior) => {
         //   behavior.Add(args => {
         //      var result = validation((TVM)args.VM, (TValue)args.PropertyValue);
         //      if (!result.Successful) {
         //         args.AddError(result.ErrorMessage);
         //      }
         //   });
         //});
      }

      private sealed class DelegateValidator<TVM, TValue> : Validator {
         private Action<TVM, TValue, ValidationArgs> _validatorCallback;

         public DelegateValidator(Action<TVM, TValue, ValidationArgs> validatorCallback) {
            Contract.Requires(validatorCallback != null);
            _validatorCallback = validatorCallback;
         }

         public override void Validate(ValidationArgs args) {
            TVM vm = (TVM)args.TargetVM;
            TValue value = (TValue)args.TargetVM.GetValue(args.TargetProperty, ValueStage.PreValidation);
            _validatorCallback(vm, value, args);
         }

         public override string ToString() {
            return String.Format(
               "{{DelegateValidator: {0}}}",
               DelegateUtils.GetFriendlyName(_validatorCallback)
            );
         }
      }
   }
}
