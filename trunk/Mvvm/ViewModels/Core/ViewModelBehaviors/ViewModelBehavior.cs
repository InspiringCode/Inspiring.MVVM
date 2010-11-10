using Inspiring.Mvvm.ViewModels.Core.Common;
namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class ViewModelBehavior : Behavior {
      protected internal virtual void OnPropertyValidating(
         IViewModelBehaviorContext context,
         ValidationContext validationContext,
         InstancePath targetVMPath
      ) {
         this.CallNext(x => x.OnPropertyValidating(context, validationContext, targetVMPath));
      }

      protected internal virtual void OnSelfChanged(
         IViewModelBehaviorContext context,
         ChangeArgs args
      ) {
         this.CallNext(x => x.OnSelfChanged(context, args));
      }

      protected internal virtual void OnChildChanged(
         IViewModelBehaviorContext context,
         ChangeArgs args,
         InstancePath changedChildPath
      ) {
         this.CallNext(x => x.OnChildChanged(context, args, changedChildPath));
      }

      protected internal virtual void OnChanged(
         IViewModelBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {
         //Contract.Requires(changedPath.RootVM == context.VM && changedPath.LeafVM == args.ChangedVM);
         this.CallNext(x => x.OnChanged(context, args, changedPath));
      }


   }
}
