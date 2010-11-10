namespace Inspiring.MvvmTest.Stubs {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelBehaviorContextStub : IViewModelBehaviorContext {
      public ViewModelBehaviorContextStub(IViewModel vm) {
         VM = vm;
      }

      public IViewModel VM { get; private set; }
   }
}
