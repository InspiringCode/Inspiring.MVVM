namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public sealed class ValidatorSetup {
      private List<ValidatorResultSetup> _setups = new List<ValidatorResultSetup>();
      private List<ValidatorInvocation> _expectedInvocations = new List<ValidatorInvocation>();
      private List<ValidatorInvocation> _actualInvocations = new List<ValidatorInvocation>();
      private List<IViewModel> _validViewModels = new List<IViewModel>();
      private IViewModel _defaultOwner;

      public ValidatorSetup() {
      }

      private ValidatorSetup(IViewModel defaultOwner, ValidatorSetup parent) {
         _defaultOwner = defaultOwner;
         _setups = parent._setups;
         _validViewModels = parent._validViewModels;
         _actualInvocations = parent._actualInvocations;
         _expectedInvocations = parent._expectedInvocations;
      }

      public ValidatorSetup ForOwner(IViewModel owner) {
         return new ValidatorSetup(owner, this);
      }

      public void SetPropertyError(IViewModel target, IVMPropertyDescriptor targetProperty) {
         SetError(ValidatorType.Property, target, targetProperty);
      }

      public void SetViewModelError(IViewModel target) {
         SetError(ValidatorType.ViewModel, target);
      }

      public void SetCollectionPropertyError(
         IViewModel targetItem,
         IVMPropertyDescriptor targetProperty
      ) {
         SetError(ValidatorType.CollectionProperty, targetItem, targetProperty);
      }

      public void SetCollectionViewModelError(IViewModel targetItem) {
         SetError(ValidatorType.CollectionViewModel, targetItem);
      }

      public void SetupPropertyError(IViewModel target, IVMPropertyDescriptor targetProperty) {
         SetupError(ValidatorType.Property, target, targetProperty);
      }

      public void SetupViewModelError(IViewModel target) {
         SetupError(ValidatorType.ViewModel, target);
      }

      public void SetupCollectionPropertyError(
         IViewModel targetItem,
         IVMPropertyDescriptor targetProperty
      ) {
         SetupError(ValidatorType.CollectionProperty, targetItem, targetProperty);
      }

      public void SetupCollectionViewModelError(IViewModel targetItem) {
         SetupError(ValidatorType.CollectionViewModel, targetItem);
      }

      public void ExpectPropertyValidatorInvocation(IViewModel target, IVMPropertyDescriptor targetProperty) {
         ExpectInvocation(ValidatorType.Property, target, targetProperty);
      }

      public void ExpectViewModelValidatorInvocation(IViewModel target) {
         ExpectInvocation(ValidatorType.ViewModel, target);
      }

      public void ExpectCollectionPropertyInvocation(
         IViewModel targetItem,
         IVMPropertyDescriptor targetProperty
      ) {
         ExpectInvocation(ValidatorType.CollectionProperty, targetItem, targetProperty);
      }

      public void ExpectCollectionViewModelValidatorInvocation(IViewModel targetItem) {
         ExpectInvocation(ValidatorType.CollectionViewModel, targetItem);
      }

      public void SetupPropertySuccess(IViewModel target, IVMPropertyDescriptor targetProperty) {
         SetupSuccess(ValidatorType.Property, target, targetProperty);
      }

      public void SetupViewModelSuccess(IViewModel target) {
         SetupSuccess(ValidatorType.ViewModel, target);
      }

      public void SetupCollectionPropertySuccess(
         IViewModel targetItem,
         IVMPropertyDescriptor targetProperty
      ) {
         SetupSuccess(ValidatorType.CollectionProperty, targetItem, targetProperty);
      }

      public void SetupCollectionViewModelSuccess(IViewModel targetItem) {
         SetupSuccess(ValidatorType.CollectionViewModel, targetItem);
      }

      public void ExpectedValid(IViewModel viewModel) {
         _validViewModels.Add(viewModel);
      }

      public void VerifyAll() {
         VerifySequence();
         VerifyResults();
      }

      public void VerifyAllStrict() {
         VerifySequenceStrict();
         VerifyResults();
      }

      public void VerifySequence() {
         throw new NotImplementedException();
         //var actualInvocationsThatWereSetup = _actualInvocations
         //   .Where(x => _setups.Contains(x));

         //VerifySequence(actualInvocationsThatWereSetup);
      }

      public void VerifySequenceStrict() {
         VerifySequence(_actualInvocations);
      }

      private void VerifySequence(IEnumerable<ValidatorInvocation> actualInvocations) {
         if (!_expectedInvocations.SequenceEqual(actualInvocations)) {
            Assert.Fail(
               "Expected validator invocations{0}\t{1}{0}but was{0}\t{2}{0}.",
               Environment.NewLine,
               String.Join(Environment.NewLine + "\t", _expectedInvocations),
               String.Join(Environment.NewLine + "\t", actualInvocations)
            );
         }
      }

      public void VerifyResults() {
         var expectedErrors = _setups
            .Where(x => x.Error != null) // TODO: Check valids?
            .Select(x => x.Error)
            .ToArray();

         ValidationAssert.Errors(expectedErrors);

         foreach (IViewModel valid in _validViewModels) {
            ValidationAssert.IsValid(valid);
         }
      }

      public void ClearActualInvocations() {
         _actualInvocations.Clear();
      }

      public void Reset() {
         _actualInvocations.Clear();
         _setups.Clear();
      }

      public void PerformValidation<TOwnerVM, TTargetVM, TValue>(
         PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> args
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         var errors = _setups
            .Where(s => s.Matches(ValidatorType.Property, args.Owner, args.Target, args.TargetProperty));

         errors.Where(x => x.Error != null).ForEach(s => {
            args.AddError(s.Error.Message, s.Error.Details);
         });

         AddInvocation(ValidatorType.Property, args.Owner, args.Target, args.TargetProperty);
      }

      public void PerformValidation<TOwnerVM, TTargetVM>(
         ViewModelValidationArgs<TOwnerVM, TTargetVM> args
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         var errors = _setups
            .Where(s => s.Matches(ValidatorType.ViewModel, args.Owner, args.Target));

         errors.Where(x => x.Error != null).ForEach(s => {
            args.AddError(s.Error.Message, s.Error.Details);
         });

         AddInvocation(ValidatorType.ViewModel, args.Owner, args.Target);
      }

      public void PerformValidation<TOwnerVM, TItemVM, TValue>(
         CollectionValidationArgs<TOwnerVM, TItemVM, TValue> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            var errors = _setups
               .Where(s => s.Matches(ValidatorType.CollectionProperty, args.Owner, item, args.TargetProperty));

            errors.Where(x => x.Error != null).ForEach(s => {
               args.AddError(item, s.Error.Message, s.Error.Details);
            });

            AddInvocation(ValidatorType.CollectionProperty, args.Owner, item, args.TargetProperty);
         }
      }

      public void PerformValidation<TOwnerVM, TItemVM>(
         CollectionValidationArgs<TOwnerVM, TItemVM> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            var errors = _setups
               .Where(s => s.Matches(ValidatorType.CollectionViewModel, args.Owner, item));

            errors.Where(x => x.Error != null).ForEach(s => {
               args.AddError(item, s.Error.Message, s.Error.Details);
            });

            AddInvocation(ValidatorType.CollectionViewModel, args.Owner, item);
         }
      }

      private void SetError(
         ValidatorType type,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         var setup = new ValidatorResultSetup(type, _defaultOwner, target, targetProperty, false);
         _setups.Add(setup);

         if (targetProperty != null) {
            Revalidator.RevalidatePropertyValidations(
               target,
               targetProperty,
               ValidationScope.SelfOnly
            );
         } else {
            Revalidator.RevalidateViewModelValidations(target);
         }

         _setups.Remove(setup);
         //_actualInvocations.Remove(setup);
      }

      private void SetupError(
         ValidatorType type,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         _setups.Add(new ValidatorResultSetup(type, _defaultOwner, target, targetProperty, false));
      }

      private void ExpectInvocation(
         ValidatorType type,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         _expectedInvocations.Add(new ValidatorInvocation(type, _defaultOwner, target, targetProperty));
      }

      private void SetupSuccess(
         ValidatorType type,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         _setups.Add(new ValidatorResultSetup(type, _defaultOwner, target, targetProperty, true));
      }

      private void AddInvocation(
         ValidatorType type,
         IViewModel owner,
         IViewModel target,
         IVMPropertyDescriptor property = null
      ) {
         _actualInvocations.Add(new ValidatorInvocation(type, owner, target, property));
      }

      private enum ValidatorType {
         Property,
         ViewModel,
         CollectionProperty,
         CollectionViewModel
      }

      private class ValidatorInvocation {
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

            var result = String.Format("{0} for {1}{2}", Type, target, ownerPostfix);
            return result;
         }
      }

      private class ValidatorResultSetup {
         public ValidatorResultSetup(
            ValidatorType type,
            IViewModel owner,
            IViewModel target,
            IVMPropertyDescriptor targetProperty,
            bool validatorResult
         ) {
            ValidatorType = type;
            Owner = owner;
            Target = target;
            TargetProperty = targetProperty;

            if (!validatorResult) {
               if (TargetProperty != null) {
                  Error = new ValidationError(
                     NullValidator.Instance,
                     Target,
                     TargetProperty,
                     this.ToString()
                  );
               } else {
                  Error = new ValidationError(
                     NullValidator.Instance,
                     Target,
                     this.ToString()
                  );
               }
            }
         }

         public IViewModel Owner { get; private set; }
         public IViewModel Target { get; private set; }
         public IVMPropertyDescriptor TargetProperty { get; private set; }
         public ValidatorType ValidatorType { get; private set; }

         public ValidationError Error { get; private set; }

         public bool Matches(
            ValidatorType type,
            IViewModel owner,
            IViewModel target,
            IVMPropertyDescriptor targetProperty = null
         ) {
            return
               ValidatorType == type &&
               (Owner == owner || Owner == null) &&
               Target == target &&
               TargetProperty == targetProperty;
         }

         public override string ToString() {
            string ownerPostfix = Owner != null && Owner != Target ?
               String.Format(" of {0}", Owner) :
               String.Empty;

            string propertyPostfix = TargetProperty != null ?
               String.Format(".{0}", TargetProperty) :
               String.Empty;

            return String.Format(
               "{0} for {1}{2}{3}",
               ValidatorType,
               Target,
               propertyPostfix,
               ownerPostfix
            );
         }
      }
   }
}
