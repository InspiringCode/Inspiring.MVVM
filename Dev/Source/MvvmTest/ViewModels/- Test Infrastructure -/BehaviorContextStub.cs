namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class BehaviorContextStub : IBehaviorContext {
      private readonly IBehaviorContext _inner;

      public BehaviorContextStub(IViewModel viewModel = null) {
         viewModel = viewModel ?? ViewModelStub.Build();
         _inner = viewModel.GetContext();
         NotifyChangeInvocations = new List<ChangeArgs>();
      }

      public List<ChangeArgs> NotifyChangeInvocations {
         get;
         private set;
      }

      public IViewModel VM {
         get { return _inner.VM; }
      }

      public FieldValueHolder FieldValues {
         get { return _inner.FieldValues; }
      }

      public IServiceLocator ServiceLocator {
         get { return _inner.ServiceLocator; }
      }

      public void NotifyChange(ChangeArgs args) {
         _inner.NotifyChange(args);
         NotifyChangeInvocations.Add(args);
      }
   }
}
