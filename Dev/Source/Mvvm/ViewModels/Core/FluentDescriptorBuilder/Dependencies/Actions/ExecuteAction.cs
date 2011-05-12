namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ExecuteAction<TOwnerVM> :
      DependencyAction
      where TOwnerVM : IViewModel {
      private readonly Action<TOwnerVM, ChangeArgs> _action;
      public ExecuteAction(Action<TOwnerVM, ChangeArgs> action) {
         _action = action;
      }

      public override void Execute(IViewModel ownerVM, ChangeArgs args) {
         _action((TOwnerVM)ownerVM, args);
      }
   }
}
