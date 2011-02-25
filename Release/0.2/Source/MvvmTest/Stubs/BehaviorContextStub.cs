namespace Inspiring.MvvmTest.Stubs {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class BehaviorContextStub : IBehaviorContext {
      public BehaviorContextStub(IViewModel vm) {
         VM = vm;
      }

      public IViewModel VM { get; private set; }

      public void NotifyChange(ChangeArgs args) {
         throw new System.NotImplementedException();
      }

      public FieldValueHolder FieldValues {
         get { throw new System.NotImplementedException(); }
      }

      public void NotifyValidating(ValidationArgs args) {
         throw new System.NotImplementedException();
      }

      FieldValueHolder IBehaviorContext.FieldValues {
         get { throw new System.NotImplementedException(); }
      }

      Mvvm.IServiceLocator IBehaviorContext.ServiceLocator {
         get { throw new System.NotImplementedException(); }
      }

      void IBehaviorContext.NotifyValidating(ValidationArgs args) {
         throw new System.NotImplementedException();
      }

      void IBehaviorContext.NotifyChange(ChangeArgs args) {
         throw new System.NotImplementedException();
      }
   }
}
