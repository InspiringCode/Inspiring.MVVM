namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class ViewModelBehavior : Behavior {
      protected internal virtual void OnValidating(
         IBehaviorContext context,
         ValidationArgs args
      ) {
         this.CallNext(x => x.OnValidating(context, args));
      }

      protected internal virtual void OnSelfChanged(
         IBehaviorContext context,
         ChangeArgs args
      ) {
         this.CallNext(x => x.OnSelfChanged(context, args));
      }

      protected internal virtual void OnChildChanged(
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedChildPath
      ) {
         this.CallNext(x => x.OnChildChanged(context, args, changedChildPath));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="context"></param>
      /// <param name="args"></param>
      /// <param name="changedPath">
      ///   A path containing all VMs between the <paramref name="context"/> VM
      ///   and the changed VM (including both).
      /// </param>
      protected internal virtual void OnChanged(
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedPath
      ) {
         //Contract.Requires(changedPath.RootVM == context.VM && changedPath.LeafVM == args.ChangedVM);
         this.CallNext(x => x.OnChanged(context, args, changedPath));
      }


   }
}
