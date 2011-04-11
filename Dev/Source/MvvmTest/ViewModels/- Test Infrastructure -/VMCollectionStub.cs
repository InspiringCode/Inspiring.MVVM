namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.ObjectModel;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal class VMCollectionStub {
      public static VMCollectionStub<T> Of<T>() {
         return new VMCollectionStub<T>();
      }

      public static VMCollectionStub<object> Build() {
         return Of<object>();
      }
   }

   internal class VMCollectionStub<TItemVM> : Collection<TItemVM>, IVMCollection<TItemVM> {
      private readonly IViewModel _owner;

      public VMCollectionStub()
         : this(ViewModelStub.Build()) {
      }

      public VMCollectionStub(IViewModel owner) {
         _owner = owner;
      }

      public void ReplaceItems(System.Collections.Generic.IEnumerable<TItemVM> newItems) {
         throw new System.NotImplementedException();
      }

      public BehaviorChain Behaviors {
         get { throw new System.NotImplementedException(); }
      }

      public bool IsPopulating {
         get {
            throw new System.NotImplementedException();
         }
         set {
            throw new System.NotImplementedException();
         }
      }

      public IViewModel Owner {
         get { return _owner; }
      }
   }
}
