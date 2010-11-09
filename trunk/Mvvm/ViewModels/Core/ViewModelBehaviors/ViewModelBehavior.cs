using Inspiring.Mvvm.ViewModels.Core.Common;
namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class ViewModelBehavior : Behavior {
      protected internal virtual void OnPropertyValidating(ValidationContext context, InstancePath targetVM) {
         this.CallNext(x => x.OnPropertyValidating(context, targetVM));
      }

      protected internal virtual void OnChanged(ChangeArgs args) {
         this.CallNext(x => x.OnChanged(args));
      }

      protected internal virtual void OnChildChanged(ChangeArgs args, InstancePath path) {
         this.CallNext(x => x.OnChildChanged(args, path));
      }
   }
}
