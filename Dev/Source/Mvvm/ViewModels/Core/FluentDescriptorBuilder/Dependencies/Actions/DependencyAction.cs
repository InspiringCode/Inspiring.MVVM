namespace Inspiring.Mvvm.ViewModels.Core {

   internal abstract class DependencyAction {
      public abstract void Execute(ChangeArgs args);
   }
}