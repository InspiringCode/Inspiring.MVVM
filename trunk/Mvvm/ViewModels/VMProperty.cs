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

   public class VMProperty<T> : VMProperty {
      public VMProperty(IAccessPropertyBehavior<T> accessorBehavior)
         : base(typeof(T)) {
         Contract.Requires<ArgumentNullException>(accessorBehavior != null);
         EnableBehavior(accessorBehavior, VMBehaviors.BehaviorStack);
         EnableDefaultBehaviors();
      }

      internal override IBehavior EnsureBehavior(VMBehaviorFactory behavior) {
         IBehavior b;
         if (!TryGetBehavior(behavior.BehaviorType, out b)) {
            b = behavior.Create<T>();
            EnableBehavior(b, VMBehaviors.BehaviorStack);
         }
         return b;
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
         EnsureBehavior<DisplayValueValidationBehavior>().Add(validation);
      }

      internal void AddValidation(Func<T, ValidationResult> validation) {
         EnsureBehavior<ValidationBehavior<T>>().Add(validation);
      }

      private void EnableDefaultBehaviors() {
         EnsureBehavior<DisplayValueAccessorBehavior<T>>(() =>
            new DisplayValueAccessorBehavior<T>(PropertyName)
         );
      }
   }
}
