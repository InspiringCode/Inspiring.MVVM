namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   public abstract class VMProperty : VMPropertyBehaviorChain {
      private CustomPropertyDescriptor _propertyDescriptor;

      internal VMProperty(Type type) {
         Contract.Requires(type != null);
         PropertyType = type;
      }

      public string PropertyName { get; private set; }

      public Type PropertyType { get; private set; }

      internal CustomPropertyDescriptor PropertyDescriptor {
         get {
            if (_propertyDescriptor == null) {
               _propertyDescriptor = new CustomPropertyDescriptor(this);
            }
            return _propertyDescriptor;
         }
      }

      internal abstract IBehavior EnsureBehavior(VMBehaviorFactory behavior);

      internal void Initialize(string propertyName, VMDescriptor descriptor) {
         PropertyName = propertyName;

         for (IBehavior b = this.Successor; b != null; b = b.Successor) {
            VMPropertyBehavior pb = b as VMPropertyBehavior;
            if (pb != null) {
               pb.Initialize(descriptor.DynamicFields, propertyName);
            }
         }
      }

      internal object GetDisplayValue(IBehaviorContext vm) {
         Contract.Requires(vm != null);
         return GetNextBehavior<IAccessPropertyBehavior>().GetValue(vm);
      }

      internal void SetDisplayValue(IBehaviorContext vm, object value) {
         Contract.Requires(vm != null);
         GetNextBehavior<IAccessPropertyBehavior>().SetValue(vm, value);
      }

      internal void OnPropertyChanged(IBehaviorContext vm) {
         IHandlePropertyChangingBehavior changingBehavior;
         if (TryGetBehavior(out changingBehavior)) {
            changingBehavior.HandlePropertyChanging(vm);
         }

         IHandlePropertyChangedBehavior changedBehavior;
         if (TryGetBehavior(out changedBehavior)) {
            changedBehavior.HandlePropertyChanged(vm);
         }

         if (_propertyDescriptor == null) {
            _propertyDescriptor.RaiseValueChanged(vm);
         }
      }


   }

   public abstract class VMPropertyBase<T> : VMProperty {

      public VMPropertyBase()
         : base(typeof(T)) {
      }

      internal override IBehavior EnsureBehavior(VMBehaviorFactory behavior) {
         for (IBehavior b = this; b != null; b = b.Successor) {
            if (behavior.Matches(b)) {
               return b;
            }
         }

         IBehavior newBehavior = behavior.Create<T>();
         InsertBehavior(newBehavior);
         return newBehavior;
      }

      internal T GetValue(IBehaviorContext vm) {
         Contract.Requires(vm != null);
         return GetNextBehavior<IAccessPropertyBehavior<T>>().GetValue(vm);
      }

      internal void SetValue(IBehaviorContext vm, T value) {
         Contract.Requires(vm != null);
         GetNextBehavior<IAccessPropertyBehavior<T>>().SetValue(vm, value);
      }

      internal void AddValidation(Func<object, ValidationResult> validation) {
         EnsureBehavior(() => new DisplayValueValidationBehavior()).Add(validation);
      }

      internal void AddValidation(Func<T, ValidationResult> validation) {
         EnsureBehavior(() => new ValidationBehavior<T>()).Add(validation);
      }

      //protected IEnumerable<Type> GetPotentialBehaviorsOrder() {

      //}
   }

   public class VMProperty<T> : VMPropertyBase<T> {
      public VMProperty(IAccessPropertyBehavior<T> accessorBehavior) {
         Contract.Requires<ArgumentNullException>(accessorBehavior != null);
         InsertBehavior(accessorBehavior);
         EnableDefaultBehaviors();
      }

      private void EnableDefaultBehaviors() {
         EnsureBehavior<DisplayValueAccessorBehavior<T>>(() =>
            new DisplayValueAccessorBehavior<T>(PropertyName)
         );
      }
   }
}
