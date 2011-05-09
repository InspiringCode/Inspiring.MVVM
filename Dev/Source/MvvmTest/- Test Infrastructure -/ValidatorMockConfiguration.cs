namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public class ValidatorMockConfiguration {
      private ValidatorMockConfigurationState _initialState;

      public ValidatorMockConfiguration() {
         ValidatorSetups = new List<ValidatorResultSetup>();
         ExpectedInvocations = new List<ValidatorInvocation>();
         ActualInvocations = new List<ValidatorInvocation>();

         _initialState = GetState();
      }

      public List<ValidatorResultSetup> ValidatorSetups { get; private set; }
      protected List<ValidatorInvocation> ExpectedInvocations { get; private set; }
      public List<ValidatorInvocation> ActualInvocations { get; private set; }

      public IEnumerable<ValidationResult> SetupResults {
         get {
            return ValidatorSetups
               .Select(x => x.Result)
               .Where(x => !x.IsValid);
         }
      }

      public void VerifySequenceAndResults() {
         VerifyInvocationSequence();
         VerifySetupValidationResults();
      }

      public void VerifySetupValidationResults() {
         var expectedErrors = ValidatorSetups
            .Where(x => !x.Result.IsValid)
            .SelectMany(x => x.Result.Errors)
            .ToArray();

         ValidationAssert.Errors(expectedErrors);



         // TODO: Check if this logic is correct for collection expectations.

         var validViewModelPropertyCombinations = ActualInvocations
            .Where(x => x.TargetVM != null)
            .GroupBy(x => new { VM = x.TargetVM, Prop = x.TargetProperty })
            .Where(g =>
               !ValidatorSetups
                  .Select(x => new { VM = x.Invocation.TargetVM, Prop = x.Invocation.TargetProperty })
                  .Contains(g.Key)
            );

         foreach (var item in validViewModelPropertyCombinations) {
            var target = item.Key.VM;
            var targetProperty = item.Key.Prop;

            if (targetProperty != null) {
               ValidationAssert.IsValid(target, targetProperty);
            } else {
               ValidationAssert.ValidViewModelValidationResultIsValid(target);
            }
         }

         var validSetup = ValidatorSetups.Where(x => x.Result.IsValid);

         foreach (var setup in validSetup) {
            var target = setup.Invocation.TargetVM;
            var targetProperty = setup.Invocation.TargetProperty;

            if (targetProperty != null) {
               ValidationAssert.IsValid(target, targetProperty);
            } else {
               ValidationAssert.ValidViewModelValidationResultIsValid(target);
            }
         }
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

      public void Reset() {
         _initialState.RestoreToState();
      }

      public void PerformValidation<TOwnerVM, TTargetVM, TValue>(
         PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> args,
         object validatorKey = null
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         PerformValidation(
            errorMessage => args.AddError(errorMessage),
            ValidatorType.Property,
            args.Owner,
            args.Target,
            validatorKey,
            args.TargetProperty
         );
      }

      public void PerformValidation<TOwnerVM, TTargetVM>(
         ViewModelValidationArgs<TOwnerVM, TTargetVM> args,
         object validatorKey = null
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         PerformValidation(
            errorMessage => args.AddError(errorMessage),
            ValidatorType.ViewModel,
            args.Owner,
            args.Target,
            validatorKey
         );
      }

      public void PerformValidation<TOwnerVM, TItemVM, TValue>(
         CollectionValidationArgs<TOwnerVM, TItemVM, TValue> args,
         object validatorKey = null
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         PerformCollectionValidation(
             args.AddError,
             ValidatorType.CollectionProperty,
             args.Owner,
             args.Items,
             validatorKey,
             args.TargetProperty
          );
      }

      public void PerformValidation<TOwnerVM, TItemVM>(
         CollectionValidationArgs<TOwnerVM, TItemVM> args,
         object validatorKey = null
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         PerformCollectionValidation(
            args.AddError,
            ValidatorType.CollectionViewModel,
            args.Owner,
            args.Items,
            validatorKey
         );
      }

      protected virtual void PerformValidation(
         Action<string> addValidationErrorAction,
         ValidatorType type,
         IViewModel owner,
         IViewModel targetVM,
         object validatorKey,
         IVMPropertyDescriptor targetProperty = null
      ) {
         var invocation = new ValidatorInvocation(type, owner, targetVM, null, targetProperty, validatorKey);

         var errors = ValidatorSetups
            .Where(x => x.Invocation.Equals(invocation))
            .SelectMany(x => x.Result.Errors);

         errors.ForEach(x => addValidationErrorAction(x.Message));

         ActualInvocations.Add(
            new ValidatorInvocation(type, owner, targetVM, null, targetProperty, validatorKey)
         );
      }

      protected virtual void PerformCollectionValidation<TItemVM>(
         Action<TItemVM, string, object> addValidationErrorAction,
         ValidatorType type,
         IViewModel owner,
         IVMCollectionBase<TItemVM> targetCollection,
         object validatorKey,
         IVMPropertyDescriptor targetProperty = null
      ) where TItemVM : IViewModel {
         foreach (TItemVM item in targetCollection) {
            var invocation = new ValidatorInvocation(type, owner, item, null, targetProperty, validatorKey);

            var errors = ValidatorSetups
               .Where(x => x.Invocation.Equals(invocation))
               .SelectMany(x => x.Result.Errors);

            errors.ForEach(x => addValidationErrorAction(item, x.Message, x.Details));
         }

         ActualInvocations.Add(
            new ValidatorInvocation(type, owner, null, targetCollection, targetProperty, validatorKey)
         );
      }


      protected void SetupFailingValidator(ValidatorInvocation forInvocation, string errorDetails) {
         ValidatorSetups.Add(ValidatorResultSetup.Failing(forInvocation, errorDetails));
      }

      protected void SetupSucceedingValidator(ValidatorInvocation forInvocation) {
         ValidatorSetups.Add(ValidatorResultSetup.Succeeding(forInvocation));
      }

      protected void SetFailedResult(ValidatorInvocation target, string errorDetails) {
         var stateBefore = GetState();

         SetupFailingValidator(target, errorDetails);
         if (target.TargetProperty != null) {
            Revalidator.RevalidatePropertyValidations(
               target.TargetVM,
               target.TargetProperty,
               ValidationScope.Self
            );
         } else {
            Revalidator.RevalidateViewModelValidations(target.TargetVM);
         }

         stateBefore.RestoreToState();
      }

      protected void ExpectInvocation(ValidatorInvocation invocation) {
         ExpectedInvocations.Add(invocation);
      }

      public class ValidatorMockConfigurationState {
         private ValidatorMockConfiguration _config;
         private Tuple<ValidatorResultSetup[], ValidatorInvocation[], ValidatorInvocation[]> _state;

         public ValidatorMockConfigurationState(ValidatorMockConfiguration config) {
            _config = config;
            _state = Tuple.Create(
               config.ValidatorSetups.ToArray(),
               config.ExpectedInvocations.ToArray(),
               config.ActualInvocations.ToArray()
            );
         }

         public void RestoreToState() {
            _config.ValidatorSetups = _state.Item1.ToList();
            _config.ExpectedInvocations = _state.Item2.ToList();
            _config.ActualInvocations = _state.Item3.ToList();
         }
      }

      public enum ValidatorType {
         Property,
         ViewModel,
         CollectionProperty,
         CollectionViewModel
      }

      public class ValidatorResultSetup {
         public static ValidatorResultSetup Succeeding(ValidatorInvocation invocation) {
            return new ValidatorResultSetup {
               Invocation = invocation,
               Result = ValidationResult.Valid
            };
         }

         public static ValidatorResultSetup Failing(ValidatorInvocation invocation, string errorDetails) {
            var error = new ValidationError(
               NullValidator.Instance,
               invocation.GetValidationTarget(),
               invocation.ToString(errorDetails)
            );

            return new ValidatorResultSetup {
               Invocation = invocation,
               Result = new ValidationResult(error)
            };
         }

         public ValidatorInvocation Invocation { get; private set; }
         public ValidationResult Result { get; private set; }
      }

      public class ValidatorInvocation {
         public ValidatorInvocation(
            ValidatorType type,
            IViewModel owner,
            IViewModel targetVM,
            IVMCollection targetCollection,
            IVMPropertyDescriptor targetProperty,
            object validatorKey = null
         ) {
            Type = type;
            Owner = owner;
            TargetVM = targetVM;
            TargetProperty = targetProperty;
            TargetCollection = targetCollection;
            ValidatorKey = validatorKey;
         }

         public ValidatorType Type { get; private set; }
         public IViewModel Owner { get; private set; }
         public IViewModel TargetVM { get; private set; }
         public IVMCollection TargetCollection { get; private set; }
         public IVMPropertyDescriptor TargetProperty { get; private set; }
         public object ValidatorKey { get; private set; }

         public ValidationStep Step {
            get {
               switch (Type) {
                  case ValidatorType.Property:
                  case ValidatorType.CollectionProperty:
                     return ValidationStep.Value;
                  case ValidatorType.ViewModel:
                  case ValidatorType.CollectionViewModel:
                     return ValidationStep.ViewModel;
                  default:
                     throw new NotSupportedException();
               }
            }
         }

         public IValidationErrorTarget GetValidationTarget() {
            return ValidationTarget.ForError(
               Step,
               TargetVM,
               TargetCollection,
               TargetProperty
            );
         }

         public override bool Equals(object obj) {
            ValidatorInvocation other = obj as ValidatorInvocation;

            return
               other != null &&
               Type == other.Type &&
               Owner == other.Owner &&
               TargetVM == other.TargetVM &&
               TargetCollection == other.TargetCollection &&
               TargetProperty == other.TargetProperty &&
               ValidatorKey == other.ValidatorKey;
         }

         public override int GetHashCode() {
            return HashCodeService.CalculateHashCode(
               this,
               Type,
               Owner,
               TargetVM,
               TargetCollection,
               TargetProperty,
               ValidatorKey
            );
         }

         public override string ToString() {
            return ToString(null);
         }

         public string ToString(string details) {
            var ownerPostfix = Owner != null && Owner != TargetVM ?
               String.Format(" of {0}", Owner) :
               String.Empty;

            var target = TargetCollection != null ?
               TypeService.GetFriendlyTypeName(TargetCollection) :
               String.Format("{0}", TargetVM);

            var targetWithProperty = TargetProperty != null ?
               String.Format("{0}.{1}", target, TargetProperty) :
               String.Format("{0}", target);

            var validatorPostfix = ValidatorKey != null ?
               String.Format(" by {0}", ValidatorKey) :
               String.Empty;

            var detailsPostfix = details != null ?
               String.Format(" ({0})", details) :
               String.Empty;

            return String.Format("{0} for {1}{2}{3}{4}", Type, targetWithProperty, ownerPostfix, validatorPostfix, detailsPostfix);
         }
      }
   }
}