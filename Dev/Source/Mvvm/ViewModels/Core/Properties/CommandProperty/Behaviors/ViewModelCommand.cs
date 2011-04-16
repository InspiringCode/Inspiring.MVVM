namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows.Input;

   public class ViewModelCommand : ICommand {
      /// <param name="ownerVM">
      ///   The view model instance to which this command instance belongs. The
      ///   VM must be known by the command because a command always related to
      ///   a certain VM (the execute action/can execute result is different for
      ///   every VM).
      /// </param>
      public ViewModelCommand(IViewModel ownerVM, IVMPropertyDescriptor ownerProperty) {
         Contract.Requires(ownerVM != null);
         Contract.Requires(ownerProperty != null);

         OwnerVM = ownerVM;
         OwnerProperty = ownerProperty;
      }

      public event EventHandler CanExecuteChanged {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }

      protected IViewModel OwnerVM { get; private set; }

      protected IVMPropertyDescriptor OwnerProperty { get; private set; }

      public virtual bool CanExecute(object parameter) {
         return OwnerProperty
            .Behaviors
            .CanExecuteNext(OwnerVM.GetContext(), parameter);
      }

      public virtual void Execute(object parameter) {
         OwnerProperty
            .Behaviors
            .ExecuteNext(OwnerVM.GetContext(), parameter);
      }
   }
}
