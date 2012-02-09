namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public class ViewModelValidationArgs<TOwnerVM, TTargetVM> :
      ValidationArgs<TOwnerVM>
      where TOwnerVM : IViewModel
      where TTargetVM : IViewModel {

      public ViewModelValidationArgs(IValidator validator, TOwnerVM owner, TTargetVM target)
         : base(ValidationStep.ViewModel, validator, owner) {

         Contract.Requires(target != null);
         Target = target;
      }

      public TTargetVM Target { get; private set; }

      public void AddError(string message, object details = null) {
         Contract.Requires<ArgumentNullException>(message != null);

         var target = ValidationTarget.ForError(Step, Target, null, null);
         var error = new ValidationError(Validator, target, message, details);
         AddError(error);
      }

      internal static ViewModelValidationArgs<TOwnerVM, TTargetVM> Create(
         IValidator validator,
         ValidationRequest request
      ) {
         Path path = request.TargetPath;

         var owner = (TOwnerVM)path[0].ViewModel;
         var target = (TTargetVM)path[path.Length - 1].ViewModel;

         return new ViewModelValidationArgs<TOwnerVM, TTargetVM>(validator, owner, target);
      }
   }
}
