namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Windows.Input;

   public class CommandAccessorBehavior :
      CachedAccessorBehavior<ICommand> {

      private IVMPropertyDescriptor _property;

      public override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = context.Property;
      }

      protected override ICommand ProvideValue(IBehaviorContext context) {
         return CreateCommand(
            ownerVM: context.VM,
            ownerProperty: _property
         );
      }

      protected virtual ICommand CreateCommand(
         IViewModel ownerVM,
         IVMPropertyDescriptor ownerProperty
      ) {
         return new ViewModelCommand(ownerVM, ownerProperty);
      }
   }
}
