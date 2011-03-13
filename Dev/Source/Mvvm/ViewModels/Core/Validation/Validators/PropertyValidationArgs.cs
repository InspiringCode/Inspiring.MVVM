﻿namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> :
      ValidationArgs<TOwnerVM>
      where TTargetVM : IViewModel
      where TOwnerVM : IViewModel {

      public PropertyValidationArgs(
         IValidator validator,
         TOwnerVM owner,
         TTargetVM target,
         IVMPropertyDescriptor<TValue> targetProperty
      )
         : base(validator, owner) {
         Contract.Requires(target != null);
         Contract.Requires(targetProperty != null);

         Target = target;
         TargetProperty = targetProperty;
         Value = Target.Kernel.GetValue(TargetProperty);
      }

      public TTargetVM Target { get; private set; }

      public IVMPropertyDescriptor<TValue> TargetProperty { get; private set; }

      public TValue Value { get; private set; }

      public void AddError(string message) {
         Contract.Requires<ArgumentNullException>(message != null);
         AddError(Target, message);
      }

      internal static PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> Create(ValidationRequest request) {
         Path path = request.ValidationTarget;

         var owner = (TOwnerVM)path[0].ViewModel;
         var target = (TTargetVM)path[path.Length - 2].ViewModel;
         var property = (IVMPropertyDescriptor<TValue>)path[path.Length - 1].Property;

         //return new PropertyValidationArgs<TOwnerVM,TTargetVM,TValue>(
         throw new NotImplementedException();
      }
   }
}