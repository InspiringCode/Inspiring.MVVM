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
         Behavior,
         IRefreshBehavior
         where TValue : IViewModel {

         public void Refresh(IBehaviorContext context) {
            GetNextBehavior<ValueCacheBehavior<TValue>>().ClearCache(context);

            var viewModel = this.GetValueNext<TValue>(context);
            viewModel.Kernel.Revalidate(
               ValidationScope.SelfAndLoadedDescendants,
               ValidationMode.DiscardInvalidValues
            );

            this.RefreshNext(context);
         }
      }

      internal sealed class ViewModelInstanceProperty<TValue> :
         Behavior,
         IRefreshBehavior
         where TValue : IViewModel {

         public void Refresh(IBehaviorContext context) {
            var viewModel = this.GetValueNext<TValue>(context);
            viewModel.Kernel.Refresh();

            this.RefreshNext(context);
         }
      }

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
