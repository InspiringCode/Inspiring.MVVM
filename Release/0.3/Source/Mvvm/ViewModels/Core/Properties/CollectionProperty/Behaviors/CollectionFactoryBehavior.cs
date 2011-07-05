namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Test me
   internal sealed class CollectionFactoryBehavior<TItemVM> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueFactoryBehavior<IVMCollection<TItemVM>>
      where TItemVM : IViewModel {

      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
      }

      public IVMCollection<TItemVM> CreateValue(IBehaviorContext context) {
         return new VMCollection<TItemVM>(
            ownerVM: context.VM,
            ownerProperty: _property
         );
      }
   }
}
