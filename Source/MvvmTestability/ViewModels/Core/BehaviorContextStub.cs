namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class BehaviorContextStub : IBehaviorContext {
      private readonly IBehaviorContext _inner;
      private IServiceLocator _overriddenServiceLocator;

      public BehaviorContextStub(IViewModel viewModel = null) {
         viewModel = viewModel ?? ViewModelStub.Build();
         _inner = viewModel.GetContext();
         NotifyChangeInvocations = new List<ChangeArgs>();
      }

      public static BehaviorContextStubBuilder DecoratingContextOf(IViewModel viewModel) {
         return new BehaviorContextStubBuilder().DecoratingContextOf(viewModel);
      }

      public static BehaviorContextStub Build() {
         return new BehaviorContextStubBuilder().Build();
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
         get { return _overriddenServiceLocator ?? _inner.ServiceLocator; }
         set { _overriddenServiceLocator = value; }
      }

      public void NotifyChange(ChangeArgs args) {
         _inner.NotifyChange(args);
         NotifyChangeInvocations.Add(args);
      }
   }

   public class BehaviorContextStubBuilder {
      private IViewModel _decoratedViewModel;

      public BehaviorContextStubBuilder DecoratingContextOf(IViewModel viewModel) {
         _decoratedViewModel = viewModel;
         return this;
      }

      public BehaviorContextStub Build() {
         return new BehaviorContextStub(_decoratedViewModel);
      }
   }
}
