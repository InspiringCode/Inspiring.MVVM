namespace Inspiring.Mvvm.ViewModels.Core {

   public abstract class ViewModelBehavior : Behavior {
      protected internal virtual void OnValidating(
         IBehaviorContext context,
         ValidationArgs args
      ) {
         this.OnValidatingNext(context, args);
      }

      protected internal virtual void OnSelfChanged(
         IBehaviorContext context,
         ChangeArgs args
      ) {
         this.OnSelfChangedNext(context, args);
      }

      protected internal virtual void OnChildChanged(
         IBehaviorContext context,
         ChangeArgs args,
         InstancePath changedChildPath
      ) {
         this.OnChildChangedNext(context, args, changedChildPath);
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
         this.OnChangedNext(context, args, changedPath);
      }


   }
}
