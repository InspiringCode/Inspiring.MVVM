namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A class that holds a list of VM instances that need to be traversed
   ///   to get from one VM to an descendant VM in a VM hierarchy.
   /// </summary>
   public sealed class InstancePath {
      private readonly InstancePathStep[] _steps;

      internal InstancePath() {
         _steps = new InstancePathStep[0];
      }

      internal InstancePath(IViewModel vm)
         : this(new InstancePathStep(vm)) {
         Contract.Requires(vm != null);
      }

      private InstancePath(params InstancePathStep[] steps) {
         Contract.Requires(steps != null);
         _steps = steps;
      }



      public InstancePathStep[] Steps {
         get {
            Contract.Ensures(Contract.Result<InstancePathStep[]>() != null);
            return _steps;
         }
      }


   }

   public sealed class InstancePathStep {
      internal InstancePathStep(IViewModel vm) {
         Contract.Requires(vm != null);

         VM = vm;
      }

      public IViewModel VM { get; private set; }

      public IEnumerable ParentCollection { get; set; }

      //public override bool Equals(object obj) {
      //   InstancePathStep other = obj as InstancePathStep;
      //   return
      //      other != null &&
      //      Object.ReferenceEquals(VM, other.VM) &&
      //      Object.ReferenceEquals(ParentCollection, other.ParentCollection);
      //}

      //public override int GetHashCode() {
      //   int code = VM.GetHashCode();

      //   if (ParentCollection != null) {
      //      code ^= ParentCollection.GetHashCode();
      //   }

      //   return code;
      //}
   }
}
