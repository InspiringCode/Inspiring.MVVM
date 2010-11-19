namespace Inspiring.MvvmTest.Stubs {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelBehaviorContextStub : IBehaviorContext_ {
      public ViewModelBehaviorContextStub(IViewModel vm) {
         VM = vm;
      }

      public IViewModel VM { get; private set; }

      public void NotifyChange(ChangeArgs args) {
         throw new System.NotImplementedException();
      }

      public FieldValueHolder FieldValues {
         get { throw new System.NotImplementedException(); }
      }

      public void NotifyValidating(_ValidationArgs args) {
         throw new System.NotImplementedException();
      }

      IViewModel IBehaviorContext_.VM {
         get { throw new System.NotImplementedException(); }
      }

      FieldValueHolder IBehaviorContext_.FieldValues {
         get { throw new System.NotImplementedException(); }
      }

      Mvvm.IServiceLocator IBehaviorContext_.ServiceLocator {
         get { throw new System.NotImplementedException(); }
      }

      void IBehaviorContext_.NotifyValidating(_ValidationArgs args) {
         throw new System.NotImplementedException();
      }

      void IBehaviorContext_.NotifyChange(ChangeArgs args) {
         throw new System.NotImplementedException();
      }
   }
}
