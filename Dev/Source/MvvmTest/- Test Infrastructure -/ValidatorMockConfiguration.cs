namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public class ValidatorMockConfiguration {
      public ValidatorMockConfiguration() {
         ValidatorSetups = new List<ValidatorResultSetup>();
         ExpectedInvocations = new List<ValidatorInvocation>();
         ActualInvocations = new List<ValidatorInvocation>();
      }

      protected List<ValidatorResultSetup> ValidatorSetups { get; private set; }
      protected List<ValidatorInvocation> ExpectedInvocations { get; private set; }
      protected List<ValidatorInvocation> ActualInvocations { get; private set; }

      public void VerifySequenceAndResults() {
         VerifyInvocationSequence();
         VerifyValidationResults();
      }

      public void VerifyValidationResults() {
         var expectedErrors = ValidatorSetups
            .Where(x => !x.Result.IsValid)
            .SelectMany(x => x.Result.Errors)
            .ToArray();

         ValidationAssert.Errors(expectedErrors);

         var expectedSuccesses = ValidatorSetups
            .Where(x => x.Result.IsValid);

         expectedSuccesses
            .ForEach(x => ValidationAssert.IsValid(x.Invocation.Target));
      }

      public void VerifyInvocationSequence() {
         if (!ExpectedInvocations.SequenceEqual(ActualInvocations)) {
            Assert.Fail(
               "Expected validator invocations{0}\t{1}{0}but was{0}\t{2}{0}.",
               Environment.NewLine,
               String.Join(Environment.NewLine + "\t", ExpectedInvocations),
               String.Join(Environment.NewLine + "\t", ActualInvocations)
            );
         }
      }

      public ValidatorMockConfigurationState GetState() {
         return new ValidatorMockConfigurationState(this);
      }

      public void PerformValidation<TOwnerVM, TTargetVM, TValue>(
         PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> args
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         PerformValidation(
            errorMessage => args.AddError(errorMessage),
            ValidatorType.Property,
            args.Owner,
            args.Target,
            args.TargetProperty
         );
      }

      public void PerformValidation<TOwnerVM, TTargetVM>(
         ViewModelValidationArgs<TOwnerVM, TTargetVM> args
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         PerformValidation(
            errorMessage => args.AddError(errorMessage),
            ValidatorType.ViewModel,
            args.Owner,
            args.Target
         );
      }

      public void PerformValidation<TOwnerVM, TItemVM, TValue>(
         CollectionValidationArgs<TOwnerVM, TItemVM, TValue> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            PerformValidation(
               errorMessage => args.AddError(item, errorMessage),
               ValidatorType.CollectionProperty,
               args.Owner,
               item,
               args.TargetProperty
            );
         }
      }

      public void PerformValidation<TOwnerVM, TItemVM>(
         CollectionValidationArgs<TOwnerVM, TItemVM> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            PerformValidation(
               errorMessage => args.AddError(item, errorMessage),
               ValidatorType.CollectionViewModel,
               args.Owner,
               item
            );
         }
      }

      protected void PerformValidation(
         Action<string> addValidationErrorAction,
         ValidatorType type,
         IViewModel owner,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         var invocation = new ValidatorInvocation(type, owner, target, targetProperty);

         var errors = ValidatorSetups
            .Where(x => x.Invocation.Equals(invocation))
            .SelectMany(x => x.Result.Errors);

         errors.ForEach(x => addValidationErrorAction(x.Message));

         ActualInvocations.Add(
            new ValidatorInvocation(type, owner, target, targetProperty)
         );
      }


      protected void SetupFailingValidator(ValidatorInvocation forInvocation) {
         ValidatorSetups.Add(ValidatorResultSetup.Failing(forInvocation));
      }

      protected void SetupSucceedingValidator(ValidatorInvocation forInvocation) {
         ValidatorSetups.Add(ValidatorResultSetup.Succeeding(forInvocation));
      }

      protected void SetFailedResult(ValidatorInvocation target) {
         var stateBefore = GetState();

         SetupFailingValidator(target);
         if (target.TargetProperty != null) {
            Revalidator.RevalidatePropertyValidations(
               target.Target,
               target.TargetProperty,
               ValidationScope.SelfOnly
            );
         } else {
            Revalidator.RevalidateViewModelValidations(target.Target);
         }

         stateBefore.RestoreToState();
      }

      protected void ExpectInvocation(ValidatorInvocation invocation) {
         ExpectedInvocations.Add(invocation);
      }

      public class ValidatorMockConfigurationState {
         private ValidatorMockConfiguration _config;
         private Tuple<List<ValidatorResultSetup>, List<ValidatorInvocation>, List<ValidatorInvocation>> _state;

         public ValidatorMockConfigurationState(ValidatorMockConfiguration config) {
            _config = config;
            _state = Tuple.Create(
               config.ValidatorSetups.ToList(),
               config.ExpectedInvocations.ToList(),
               config.ActualInvocations.ToList()
            );
         }

         public void RestoreToState() {
            _config.ValidatorSetups = _state.Item1;
            _config.ExpectedInvocations = _state.Item2;
            _config.ActualInvocations = _state.Item3;
         }
      }

      protected enum ValidatorType {
         Property,
         ViewModel,
         CollectionProperty,
         CollectionViewModel
      }

      protected class ValidatorResultSetup {
         public static ValidatorResultSetup Succeeding(ValidatorInvocation invocation) {
            return new ValidatorResultSetup {
               Invocation = invocation,
               Result = ValidationResult.Valid
            };
         }

         public static ValidatorResultSetup Failing(ValidatorInvocation invocation) {
            var error = invocation.TargetProperty != null ?
               new ValidationError(
                  NullValidator.Instance,
                  invocation.Target,
                  invocation.TargetProperty,
                  invocation.ToString()) :
               new ValidationError(
                  NullValidator.Instance,
                  invocation.Target,
                  invocation.ToString());

            return new ValidatorResultSetup {
               Invocation = invocation,
               Result = new ValidationResult(error)
            };
         }

         public ValidatorInvocation Invocation { get; private set; }
         public ValidationResult Result { get; private set; }
      }

      protected class ValidatorInvocation {
         public ValidatorInvocation(
            ValidatorType type,
            IViewModel owner,
            IViewModel target,
            IVMPropertyDescriptor targetProperty
         ) {
            Type = type;
            Owner = owner;
            Target = target;
            TargetProperty = targetProperty;
         }

         public ValidatorType Type { get; private set; }
         public IViewModel Owner { get; private set; }
         public IViewModel Target { get; private set; }
         public IVMPropertyDescriptor TargetProperty { get; private set; }

         public override bool Equals(object obj) {
            ValidatorInvocation other = obj as ValidatorInvocation;

            return
               other != null &&
               Type == other.Type &&
               Owner == other.Owner &&
               Target == other.Target &&
               TargetProperty == other.TargetProperty;
         }

         public override int GetHashCode() {
            return HashCodeService.CalculateHashCode(
               this,
               Type,
               Owner,
               Target,
               TargetProperty
            );
         }

         public override string ToString() {
            var ownerPostfix = Owner != null && Owner != Target ?
               String.Format(" of {0}", Owner) :
               String.Empty;

            var target = TargetProperty != null ?
               String.Format("{0}.{1}", Target, TargetProperty) :
               String.Format("{0}", Target);

            return String.Format("{0} for {1}{2}", Type, target, ownerPostfix);
         }
      }
   }
}