namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Future;

   public sealed class VMKernel : IBehaviorContext_ {
      private readonly IViewModel _vm;
      private readonly VMDescriptorBase _descriptor;

      public VMKernel(IViewModel vm, VMDescriptorBase descriptor) {
         Contract.Requires<ArgumentNullException>(vm != null);
         Contract.Requires<ArgumentNullException>(descriptor != null);

         _vm = vm;
         _descriptor = descriptor;
      }

      public IViewModel Parent { get; set; }

      IViewModel IBehaviorContext_.VM {
         get { return _vm; }
      }

      FieldValueHolder IBehaviorContext_.FieldValues {
         get { throw new NotImplementedException(); }
      }

      void IBehaviorContext_.NotifyChange(ChangeArgs args) {
         NotifyChange(args, InstancePath.Empty);
      }


      void IBehaviorContext_.NotifyValidating(_ValidationArgs args) {
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }

      private void NotifyChange(ChangeArgs args, InstancePath changedPath) {
         bool selfChanged = changedPath.IsEmpty;

         changedPath = changedPath.PrependVM(_vm);

         ViewModelBehavior behavior;
         if (_descriptor.Behaviors.TryGetBehavior(out behavior)) {
            if (selfChanged) {
               behavior.OnSelfChanged(this, args);
            } else {
               behavior.OnChildChanged(this, args, changedPath);
            }

            behavior.OnChanged(this, args, changedPath);
         }

         if (Parent != null) {
            Parent.Kernel.NotifyChange(args, changedPath);
         }
      }

      private void HandleNotifyValidating(_ValidationArgs args) {
         _descriptor.Behaviors.Call<ViewModelBehavior>(b =>
            b.OnValidating(this, args)
         );
      }

      private void NotifyValidating(_ValidationArgs args) {
         args = args.PrependTargetPath(with: _vm);
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }


      public IServiceLocator ServiceLocator {
         get { throw new NotImplementedException(); }
      }
   }
}
