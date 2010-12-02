namespace Inspiring.Mvvm.ViewModels.Fluent {

   public interface IVMPropertyFactoryWithSource<TVM, TSourceValue> where TVM : IViewModel {
      VMProperty<TSourceValue> Property();

      //VMProperty<ICommand> Command(Action<TSourceValue> execute, Func<TVM, bool> canExecute = null);
   }
}
