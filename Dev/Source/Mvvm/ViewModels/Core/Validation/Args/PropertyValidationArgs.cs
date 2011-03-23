namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
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
      }

      public TTargetVM Target { get; private set; }

      public IVMPropertyDescriptor<TValue> TargetProperty { get; private set; }

      public TValue Value {
         get { return Target.Kernel.GetValue(TargetProperty); }
      }

      public void AddError(string message, object details = null) {
         Contract.Requires<ArgumentNullException>(message != null);

         var e = new ValidationError(Validator, Target, TargetProperty, message, details);
         AddError(e);
      }

      internal static PropertyValidationArgs<TOwnerVM, TTargetVM, TValue> Create(
         IValidator validator,
         ValidationRequest request
      ) {
         Path path = request.TargetPath;

         var owner = (TOwnerVM)path[0].ViewModel;
         var target = (TTargetVM)path[path.Length - 2].ViewModel;
         var property = (IVMPropertyDescriptor<TValue>)path[path.Length - 1].Property;

         return new PropertyValidationArgs<TOwnerVM, TTargetVM, TValue>(validator, owner, target, property);
      }
   }
}
