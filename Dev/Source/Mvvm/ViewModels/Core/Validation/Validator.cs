namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class Validator {
      internal void Validate(ValidationArgs args) {
         ValidateCore(args.SetTargetValidator(this));
      }

      public abstract void ValidateCore(ValidationArgs args);
   }
}
