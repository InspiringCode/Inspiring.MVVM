namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   internal sealed class ViewModelValidationBehavior : ViewModelBehavior {
      private List<ValidationInvoker> _validators = new List<ValidationInvoker>();

      public void AddValidation(VMPropertyPath validatorPath, ViewModelValidator validator) {
         Contract.Requires<ArgumentNullException>(validatorPath != null);
         Contract.Requires<ArgumentNullException>(validator != null);

         _validators.Add(
            new ValidationInvoker(validatorPath, validator)
         );
      }

      protected internal override void OnChanged(
         IViewModelBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {

         ViewModelValidationArgs validationArgs = new ViewModelValidationArgs(
            context.VM,
            context.VM,
            args.ChangedVM,
            args.ChangedProperty
         );

         Validate(validationArgs, changedPath);

         base.OnChanged(context, args, changedPath);
      }

      //protected internal override void OnChanged(IViewModelBehaviorContext context, ChangeArgs args) {
      //   base.OnChanged(context, args);
      //   InstancePath changedVM = new InstancePath(context.VM);

      //   //var validationArgs = new ViewModelValidationArgs(context.VM);
      //   //Validate(validationArgs);
      //}

      //protected internal override void OnChildChanged(IViewModelBehaviorContext context, ChangeArgs args, InstancePath childPath) {
      //   base.OnChildChanged(context, args, childPath);
      //   InstancePath changedVM = childPath.PrependVM(context.VM);
      //}

      // OnSelfChanged(Context, ChangeArgs)

      // OnChildChanged(Context, ChangeArgs, childPath)

      // OnChanged(Context, ChangeArgs, changedVMPath)

      //public void Validate(IViewModelBehaviorContext context) {
      //   ViewModelValidationArgs args = new ViewModelValidationArgs(
      //}

      private void Validate(ViewModelValidationArgs args, InstancePath changedVMPath) {
         //_validators.ForEach(i => i.InvokeValidation(args, changedVMPath));
      }

      private class ValidationInvoker {
         public ValidationInvoker(VMPropertyPath validatorPath, ViewModelValidator validator) {
            ValidatorPath = validatorPath;
            Validator = validator;
         }
         public VMPropertyPath ValidatorPath { get; private set; }
         public ViewModelValidator Validator { get; private set; }

         public void InvokeValidator(InstancePath changedPath, ValidationErrorCollection errors) {
            Contract.Requires(changedPath != null);
            Contract.Requires(errors != null);
            Contract.Requires(!changedPath.IsEmpty);

            InstancePathMatch match = changedPath.MatchStart(ValidatorPath);
            if (match.Success) {
               IViewModel validationOwner = changedPath.RootVM;

               IViewModel validationTarget = match.MatchedPath.LeafVM;

               IViewModel changedVM = changedPath.LeafVM;



               //var args = new ViewModelValidationArgs(


            }


         }

         public void InvokeValidation(ViewModelValidationArgs args, InstancePath changedVMPath) {


            InstancePathMatch match = changedVMPath.MatchStart(ValidatorPath);
            if (match.Success) {
               //Validator.Validate(args, match.RemainingPath);
            }
         }
      }
   }
}
