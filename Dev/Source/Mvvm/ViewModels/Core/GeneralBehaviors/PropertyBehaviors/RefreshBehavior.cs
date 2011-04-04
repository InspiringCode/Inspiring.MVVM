namespace Inspiring.Mvvm.ViewModels.Core {

   internal static class RefreshBehavior {
      internal sealed class SimpleProperty<TValue> :
         InitializableBehavior,
         IBehaviorInitializationBehavior,
         IRefreshBehavior {

         private IVMPropertyDescriptor _property;

         public void Refresh(IBehaviorContext context) {
            RequireInitialized();
            context.VM.Kernel.Revalidate(_property, ValidationMode.DiscardInvalidValues);
            context.NotifyChange(new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property));

            this.RefreshNext(context);
         }

         public void Initialize(BehaviorInitializationContext context) {
            _property = context.Property;
            SetInitialized();
            this.InitializeNext(context);
         }
      }

      internal sealed class ViewModelProperty<TValue> :
         InitializableBehavior,
         IBehaviorInitializationBehavior,
         IRefreshBehavior
         where TValue : IViewModel {

         private IVMPropertyDescriptor _property;

         public void Refresh(IBehaviorContext context) {
            this.RefreshNext(context);

            var childVM = this.GetValueNext<TValue>(context);

            if (childVM != null) {
               childVM.Kernel.Refresh();
            }
            
            //// Two cases:
            ////  (1) Validations on the view model property itself:
            ////      We have to load the VM to perform these validations (e.g. a
            ////      HasValue validation).
            ////  (2) The validation of the child VM.

            //RequireInitialized();

            //// Force recreation
            //GetNextBehavior<ValueCacheBehaviorOld<TValue>>().ClearCache(context);
            //var childVM = this.GetValueNext<TValue>(context);

            //if (childVM != null) {
            //   childVM.Kernel.Refresh();
            //}

            //context.VM.Kernel.Revalidate(_property, ValidationMode.DiscardInvalidValues);

            //// The value of the property has changed
            //var changeArgs = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            //context.NotifyChange(changeArgs);
         }

         public void Initialize(BehaviorInitializationContext context) {
            _property = context.Property;
            SetInitialized();
            this.InitializeNext(context);
         }
      }

      //internal sealed class ViewModelInstanceProperty<TValue> :
      //   Behavior,
      //   IRefreshBehavior
      //   where TValue : IViewModel {

      //   public void Refresh(IBehaviorContext context) {
      //      var viewModel = this.GetValueNext<TValue>(context);
      //      viewModel.Kernel.Refresh();

      //      this.RefreshNext(context);
      //   }
      //}

      internal sealed class PopulatedCollectionProperty<TItemVM> :
         Behavior,
         IRefreshBehavior
         where TItemVM : IViewModel {

         public void Refresh(IBehaviorContext context) {
            this.PopulateNext(context);

            var collection = this.GetValueNext<IVMCollection<TItemVM>>(context);

            foreach (TItemVM item in collection) {
               item.Kernel.Revalidate(
                  ValidationScope.SelfAndLoadedDescendants,
                  ValidationMode.DiscardInvalidValues
               );
            }

            this.RefreshNext(context);
         }
      }

      internal sealed class CollectionInstanceProperty<TItemVM> :
         Behavior,
         IRefreshBehavior
         where TItemVM : IViewModel {

         public void Refresh(IBehaviorContext context) {
            var collection = this.GetValueNext<IVMCollection<TItemVM>>(context);

            foreach (TItemVM item in collection) {
               item.Kernel.Refresh();
            }

            this.RefreshNext(context);
         }
      }
   }
}
