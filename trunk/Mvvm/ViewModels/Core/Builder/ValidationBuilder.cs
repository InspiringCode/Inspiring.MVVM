namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class ValidationBuilder<TVM> : IValidationBuilder<TVM> where TVM : IViewModel {
      private BehaviorConfigurationDictionary _configs;
      private VMDescriptor _descriptor;

      public ValidationBuilder(BehaviorConfigurationDictionary configs, VMDescriptor descriptor) {
         Contract.Requires(configs != null);
         Contract.Requires(descriptor != null);

         _configs = configs;
         _descriptor = descriptor;
      }

      public IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property) {
         BehaviorConfiguration config = _configs.GetConfiguration(property);

         // Enable here, to allow enabling validation without defining a validation
         // (needed for collection validations).
         config.Enable(VMBehaviorKey.Validator);
         config.Enable(VMBehaviorKey.InvalidDisplayValueCache);

         return new ValidationBuilder<TVM, TValue>(config);
      }

      public ICollectionValidationBuilder<TItemVM> CheckCollection<TItemVM>(IVMProperty<VMCollection<TItemVM>> property) where TItemVM : IViewModel {
         return new CollectionValidationBuilder<TItemVM>(_configs, property);
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

      public ValidationBuilder(BehaviorConfiguration config) {
         Contract.Requires(config != null);
         _config = config;
      }

      public void Custom(Func<TVM, TValue, ValidationResult> validation) {
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

      public IValidationBuilder<TParentVM, TVM, TValue> WithParent<TParentVM>() where TParentVM : IViewModel {
         return new ValidationBuilder<TParentVM, TVM, TValue>(_config);
      }
   }

   internal sealed class ValidationBuilder<TParentVM, TVM, TValue> : IValidationBuilder<TParentVM, TVM, TValue>
      where TVM : IViewModel
      where TParentVM : IViewModel {
      private BehaviorConfiguration _config;

      public ValidationBuilder(BehaviorConfiguration config) {
         Contract.Requires(config != null);
         _config = config;
      }

      public void Custom(Func<TParentVM, TVM, TValue, ValidationResult> validation) {
         _config.Enable(VMBehaviorKey.Validator);
         _config.Enable(VMBehaviorKey.InvalidDisplayValueCache);

         throw new NotImplementedException("TODO2");

         //_config.Configure(VMBehaviorKey.Validator, (ValidationBehavior<TValue> behavior) => {
         //   behavior.Add(args => {
         //      TParentVM parent = args.VM.Parent as TParentVM;
         //      if (parent != null) {
         //         var result = validation(parent, (TVM)args.VM, (TValue)args.PropertyValue);
         //         if (!result.Successful) {
         //            args.AddError(result.ErrorMessage);
         //         }
         //      }
         //   });
         //});
      }
   }
}
