namespace Inspiring.MvvmTest.Stubs {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelBehaviorContextStub : IViewModelBehaviorContext {
      public ViewModelBehaviorContextStub(IViewModel vm) {
         VM = vm;
      }

      public IViewModel VM { get; private set; }


      public void NotifyViewModelValidating(ValidationState validationState) {
         throw new System.NotImplementedException();
      }


      public void NotifyChange(ChangeArgs args) {
         throw new System.NotImplementedException();
      }


      public FieldValueHolder FieldValues {
         get { throw new System.NotImplementedException(); }
      }
   }
}
