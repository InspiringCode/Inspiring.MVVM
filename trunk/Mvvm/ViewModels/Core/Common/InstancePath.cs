namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections;
   using System.Diagnostics.Contracts;
   using System.Linq;

   /// <summary>
   ///   A class that holds a list of VM instances that need to be traversed
   ///   to get from one VM to an descendant VM in a VM hierarchy.
   /// </summary>
   public sealed class InstancePath {
      public static readonly InstancePath Empty = new InstancePath();

      private readonly InstancePathStep[] _steps;

      internal InstancePath() {
         _steps = new InstancePathStep[0];
      }

      internal InstancePath(params IViewModel[] steps)
         : this(CreateSteps(steps)) {
         Contract.Requires(steps != null);
         Contract.Requires(Contract.ForAll(steps, x => x != null));
      }

      private InstancePath(InstancePathStep[] steps) {
         Contract.Requires(steps != null);
         _steps = steps;
      }

      public InstancePathStep[] Steps {
         get {
            Contract.Ensures(Contract.Result<InstancePathStep[]>() != null);
            return _steps;
         }
      }

      public IViewModel RootVM {
         get {
            Contract.Requires<InvalidOperationException>(!IsEmpty);
            Contract.Ensures(Contract.Result<IViewModel>() != null);

            return Steps.First().VM;
         }
      }

      public IViewModel LeafVM {
         get {
            Contract.Requires<InvalidOperationException>(!IsEmpty);
            Contract.Ensures(Contract.Result<IViewModel>() != null);

            return Steps.Last().VM;
         }
      }

      public int Length {
         get { return _steps.Length; }
      }

      public bool IsEmpty {
         get { return Length == 0; }
      }

      public InstancePath PrependVM(IViewModel vm) {
         Contract.Requires<ArgumentNullException>(vm != null);

         var steps = new InstancePathStep[Steps.Length + 1];
         steps[0] = new InstancePathStep(vm);
         Steps.CopyTo(steps, 1);
         return new InstancePath(steps);
      }

      //public void PrependCollection(IEnumerable vmCollection) {
      //   Contract.Requires<InvalidOperationException>(
      //      !IsEmpty,
      //      ExceptionTexts.CannotPrependCollectionToEmptyInstancePath
      //   );

      //   Steps[0].ParentCollection = vmCollection;
      //}

      public InstancePathMatch MatchStart(VMPropertyPath properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         if (MatchStartCore(properties)) {
            return new InstancePathMatch(this, properties.Length + 1);
         }

         return InstancePathMatch.Failed;
      }

      public bool Matches(VMPropertyPath properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         if (properties.Length != Length - 1) {
            return false;
         }

         return MatchStartCore(properties);
      }

      internal InstancePath Subpath(int start) {
         return Subpath(start, Int32.MaxValue);
      }

      internal InstancePath Subpath(int start, int count) {
         Contract.Requires(0 <= start && start <= Steps.Length);
         Contract.Requires(count >= 0);

         count = Math.Min(count, Steps.Length - start);

         var subpathSteps = new InstancePathStep[count];
         Array.Copy(Steps, start, subpathSteps, 0, count);

         return new InstancePath(subpathSteps);
      }

      private static InstancePathStep[] CreateSteps(IViewModel[] steps) {
         InstancePathStep[] stepsArr = new InstancePathStep[steps.Length];

         for (int i = 0; i < steps.Length; i++) {
            stepsArr[i] = new InstancePathStep(steps[i]);
         }

         return stepsArr;
      }

      public bool MatchStartCore(VMPropertyPath properties) {
         Contract.Requires<ArgumentNullException>(properties != null);

         if (properties.Length > Steps.Length - 1) {
            return false;
         }

         for (int i = 0; i < properties.Length; i++) {
            IViewModel vm = Steps[i].VM;
            IVMProperty prop = properties.GetProperty(i, vm.Descriptor);

            IViewModel expectedValue = Steps[i + 1].VM;
            IEnumerable expectedCollection = Steps[i + 1].ParentCollection;

            object propertyValue = vm.GetValue(prop);

            if (!PropertyValueMatchesStep(propertyValue, Steps[i + 1])) {
               //if (!Object.ReferenceEquals(expectedValue, propertyValue) &&
               //    !Object.ReferenceEquals(expectedCollection, propertyValue)) {
               return false;
            }
         }

         return true;
      }

      private bool PropertyValueMatchesStep(object propertyValue, InstancePathStep step) {
         // HACK: Is there a better way? An alternative would be that the item VMs
         //       know its parent.

         IViewModel stepVM = step.VM;

         if (Object.ReferenceEquals(propertyValue, stepVM)) {
            return true;
         }

         var collection = propertyValue as IEnumerable;
         if (collection != null) {
            return collection.Cast<object>().Contains(stepVM);
         }

         return false;
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

   public sealed class InstancePathMatch {
      public static readonly InstancePathMatch Failed = new InstancePathMatch();

      private readonly InstancePath _originalPath;
      private readonly int _matchedStepCount;

      internal InstancePathMatch(InstancePath originalPath, int matchedStepCount) {
         Contract.Requires(originalPath != null);
         Contract.Requires(0 <= matchedStepCount && matchedStepCount <= originalPath.Steps.Length);

         _originalPath = originalPath;
         _matchedStepCount = matchedStepCount;

         Success = true;
      }

      private InstancePathMatch() {
         _originalPath = new InstancePath();
         _matchedStepCount = 0;
      }

      public bool Success { get; private set; }

      public InstancePath MatchedPath {
         get {
            return _originalPath.Subpath(start: 0, count: _matchedStepCount);
         }
      }

      public InstancePath RemainingPath {
         get {
            return _originalPath.Subpath(start: _matchedStepCount);
         }
      }
   }
}