﻿namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.ViewModels;

   public sealed class ValidatorMockConfigurationFluent : ValidatorMockConfiguration {
      public IValidatorTypeSelector SetupFailing {
         get { return new ValidatorTypeSelector(SetupFailingValidator); }
      }

      public IValidatorTypeSelector SetupSucceeding {
         get { return new ValidatorTypeSelector(SetupSucceedingValidator); }
      }

      public IValidatorTypeSelector SetFailed {
         get { return new ValidatorTypeSelector(SetFailedResult); }
      }

      public IValidatorTypeSelector ExpectInvocationOf {
         get { return new ValidatorTypeSelector(ExpectInvocation); }
      }

      public interface IValidatorTypeSelector {
         IValidatorInvocationTargetSelector PropertyValidation { get; }
         IValidatorInvocationTargetSelector ViewModelValidation { get; }
         IValidatorInvocationTargetSelector CollectionPropertyValidation { get; }
         IValidatorInvocationTargetSelector CollectionViewModelValidation { get; }
      }

      public interface IValidatorInvocationTargetSelector {
         /// <summary>
         ///   Specifies for which validation target the setup/expection is used.
         /// </summary>
         /// <param name="target">
         ///   The value that <see cref="ValidationArgs"/>.Target must have in order
         ///   that this setup/expection is used.
         /// </param>
         IValidatorInvocationBuilder Targeting(IViewModel target);

         /// <summary>
         ///   Specifies for which validation target the setup/expection is used.
         /// </summary>
         /// <param name="target">
         ///   The value that <see cref="ValidationArgs"/>.Target must have in order
         ///   that this setup/expection is used.
         /// </param>
         /// <param name="targetPropertySelector">
         ///   The value that <see cref="ValidationArgs"/>.TargetProperty must have 
         ///   in order that this setup/expection is used.
         /// </param>
         IValidatorInvocationBuilder Targeting<TDescriptor>(
            IViewModel<TDescriptor> target,
            Func<TDescriptor, IVMPropertyDescriptor> targetPropertySelector
         ) where TDescriptor : VMDescriptorBase;
      }

      public interface IValidatorInvocationBuilder {
         /// <summary>
         ///   Creates a setup/expectation for each owner in <paramref name="owners"/>.
         /// </summary>
         void On(params IViewModel[] owners);
      }

      private class ValidatorTypeSelector : IValidatorTypeSelector {
         private readonly Action<ValidatorInvocation> _additionAction;

         public ValidatorTypeSelector(Action<ValidatorInvocation> additonAction) {
            _additionAction = additonAction;
         }

         public IValidatorInvocationTargetSelector PropertyValidation {
            get { return CreateBuilder(ValidatorType.Property); }
         }

         public IValidatorInvocationTargetSelector ViewModelValidation {
            get { return CreateBuilder(ValidatorType.ViewModel); }
         }

         public IValidatorInvocationTargetSelector CollectionPropertyValidation {
            get { return CreateBuilder(ValidatorType.CollectionProperty); }
         }

         public IValidatorInvocationTargetSelector CollectionViewModelValidation {
            get { return CreateBuilder(ValidatorType.CollectionViewModel); }
         }

         private ValidatorInvocationBuilder CreateBuilder(ValidatorType type) {
            return new ValidatorInvocationBuilder(type, _additionAction);
         }
      }

      private class ValidatorInvocationBuilder :
         IValidatorInvocationTargetSelector,
         IValidatorInvocationBuilder {

         private readonly Action<ValidatorInvocation> _additionAction;
         private ValidatorType _validatorType;
         private IViewModel _target;
         private IVMPropertyDescriptor _targetProperty;

         public ValidatorInvocationBuilder(
            ValidatorType validatorType,
            Action<ValidatorInvocation> additionAction
         ) {
            _additionAction = additionAction;
            _validatorType = validatorType;
         }

         public IValidatorInvocationBuilder Targeting(IViewModel target) {
            _target = target;
            return this;
         }

         public IValidatorInvocationBuilder Targeting<TDescriptor>(
            IViewModel<TDescriptor> target,
            Func<TDescriptor, IVMPropertyDescriptor> targetPropertySelector
         ) where TDescriptor : VMDescriptorBase {
            _target = target;
            _targetProperty = targetPropertySelector((TDescriptor)target.Descriptor);
            return this;
         }

         public void On(params IViewModel[] owners) {
            foreach (IViewModel owner in owners) {
               _additionAction(new ValidatorInvocation(_validatorType, owner, _target, _targetProperty));
            }
         }
      }
   }
}
