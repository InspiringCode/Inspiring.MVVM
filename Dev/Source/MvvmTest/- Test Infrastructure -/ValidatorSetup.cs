namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   public sealed class ValidatorSetup {
      private List<ValidatorResultSetup> _setups = new List<ValidatorResultSetup>();
      private List<ValidatorResultSetup> _actualInvocations = new List<ValidatorResultSetup>();

      public void PropertyError(IViewModel target, IVMPropertyDescriptor targetProperty) {
         Error(ValidatorType.Property, target, targetProperty);
      }

      public void ViewModelError(IViewModel target) {
         Error(ValidatorType.ViewModel, target);
      }

      public void CollectionPropertyError(
         IViewModel targetItem,
         IVMPropertyDescriptor targetProperty
      ) {
         Error(ValidatorType.CollectionProperty, targetItem, targetProperty);
      }

      public void CollectionViewModelError(IViewModel targetItem) {
         Error(ValidatorType.CollectionViewModel, targetItem);
      }

      public void VerifyAll() {
         VerifySequence();
         VerifyResults();
      }

      public void VerifySequence() {
         if (!_setups.SequenceEqual(_actualInvocations)) {
            Assert.Fail(
               "Expected validator invocations »{0}« but was »{1}«.",
               String.Join(", ", _setups),
               String.Join(", ", _actualInvocations)
            );
         }
      }

      public void VerifyResults() {
         var expectedErrors = _setups
            .Select(x => x.Error)
            .ToArray();

         ValidationAssert.Errors(expectedErrors);
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
            .Where(s => s.Matches(ValidatorType.Property, args.Target, args.TargetProperty));

         errors.ForEach(s => {
            args.AddError(s.Error.Message, s.Error.Details);
            _actualInvocations.Add(s);
         });

         if (!errors.Any()) {
            _actualInvocations.Add(
               new ValidatorResultSetup(ValidatorType.Property, args.Target, args.TargetProperty)
            );
         }
      }

      public void PerformValidation<TOwnerVM, TTargetVM>(
         ViewModelValidationArgs<TOwnerVM, TTargetVM> args
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         var errors = _setups
            .Where(s => s.Matches(ValidatorType.ViewModel, args.Target));

         errors.ForEach(s => {
            args.AddError(s.Error.Message, s.Error.Details);
            _actualInvocations.Add(s);
         });

         if (!errors.Any()) {
            _actualInvocations.Add(
               new ValidatorResultSetup(ValidatorType.ViewModel, args.Target, null)
            );
         }
      }

      public void PerformValidation<TOwnerVM, TItemVM, TValue>(
         CollectionValidationArgs<TOwnerVM, TItemVM, TValue> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            var errors = _setups
               .Where(s => s.Matches(ValidatorType.CollectionProperty, item, args.TargetProperty));

            errors.ForEach(s => {
               args.AddError(item, s.Error.Message, s.Error.Details);
               _actualInvocations.Add(s);
            });

            if (!errors.Any()) {
               _actualInvocations.Add(
                  new ValidatorResultSetup(ValidatorType.CollectionProperty, null, null)
               );
            }
         }
      }

      public void PerformValidation<TOwnerVM, TItemVM>(
         CollectionValidationArgs<TOwnerVM, TItemVM> args
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         foreach (var item in args.Items) {
            var errors = _setups
               .Where(s => s.Matches(ValidatorType.Property, item));

            errors.ForEach(s => {
               args.AddError(item, s.Error.Message, s.Error.Details);
               _actualInvocations.Add(s);
            });

            if (!errors.Any()) {
               _actualInvocations.Add(
                  new ValidatorResultSetup(ValidatorType.CollectionViewModel, null, null)
               );
            }
         }
      }

      private void Error(
         ValidatorType type,
         IViewModel target,
         IVMPropertyDescriptor targetProperty = null
      ) {
         _setups.Add(new ValidatorResultSetup(type, target, targetProperty));
      }

      private enum ValidatorType {
         Property,
         ViewModel,
         CollectionProperty,
         CollectionViewModel
      }

      private class ValidatorResultSetup {
         public ValidatorResultSetup(
            ValidatorType type,
            IViewModel target,
            IVMPropertyDescriptor targetProperty
         ) {
            ValidatorType = type;
            Target = target;
            TargetProperty = targetProperty;
         }

         public IViewModel Target { get; private set; }
         public IVMPropertyDescriptor TargetProperty { get; private set; }
         public ValidatorType ValidatorType { get; private set; }

         public ValidationError Error {
            get {
               return new ValidationError(
                  NullValidator.Instance,
                  Target,
                  TargetProperty,
                  this.ToString()
               );
            }
         }

         public bool Matches(
            ValidatorType type,
            IViewModel target,
            IVMPropertyDescriptor targetProperty = null
         ) {
            return
               ValidatorType == type &&
               Target == target &&
               TargetProperty == targetProperty;
         }

         public override string ToString() {
            return TargetProperty != null ?
               String.Format("{0} Val of {1}", ValidatorType, Target) :
               String.Format("{0} Val of {1}.{2}", ValidatorType, Target, TargetProperty);
         }
      }
   }
}
