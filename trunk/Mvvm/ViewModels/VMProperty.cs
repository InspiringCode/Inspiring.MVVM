namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMProperty {
      private CustomPropertyDescriptor _propertyDescriptor;

      internal VMProperty(Type type) {
         Contract.Requires(type != null);
         PropertyType = type;
      }

      public Behavior Behaviors { get; internal set; }

      public string PropertyName { get; private set; }

      public Type PropertyType { get; private set; }

      internal VMDescriptor Descriptor { get; private set; }

      internal CustomPropertyDescriptor PropertyDescriptor {
         get {
            if (_propertyDescriptor == null) {
               _propertyDescriptor = new CustomPropertyDescriptor(this);
            }
            return _propertyDescriptor;
         }
      }

      internal void Initialize(string propertyName, VMDescriptor descriptor) {
         PropertyName = propertyName;
         Descriptor = descriptor;
      }

      internal abstract void ConfigureBehaviors(BehaviorConfiguration configuration);

      internal object GetDisplayValue(IBehaviorContext vm) {
         Contract.Requires(vm != null);
         return Behaviors.GetNextBehavior<IAccessPropertyBehavior>().GetValue(vm);
      }

      internal void SetDisplayValue(IBehaviorContext vm, object value) {
         Contract.Requires(vm != null);
         Behaviors.GetNextBehavior<IAccessPropertyBehavior>().SetValue(vm, value);
      }

      internal void OnPropertyChanged(IBehaviorContext vm) {
         IHandlePropertyChangingBehavior changingBehavior;
         if (Behaviors.TryGetBehavior(out changingBehavior)) {
            changingBehavior.HandlePropertyChanging(vm);
         }

         IHandlePropertyChangedBehavior changedBehavior;
         if (Behaviors.TryGetBehavior(out changedBehavior)) {
            changedBehavior.HandlePropertyChanged(vm);
         }

         if (_propertyDescriptor != null) {
            _propertyDescriptor.RaiseValueChanged(vm);
         }
      }
   }

   public abstract class VMPropertyBase<T> : VMProperty, IBindableProperty<T> {

      public VMPropertyBase()
         : base(typeof(T)) {
      }

      internal override void ConfigureBehaviors(BehaviorConfiguration configuration) {
         Behaviors = configuration.CreateBehaviorChain<T>();
         ((IBehavior)Behaviors).Initialize(new BehaviorInitializationContext(this));
      }

      internal T GetValue(IBehaviorContext vm) {
         Contract.Requires(vm != null);
         return Behaviors.GetNextBehavior<IAccessPropertyBehavior<T>>().GetValue(vm);
      }

      internal void SetValue(IBehaviorContext vm, T value) {
         Contract.Requires(vm != null);
         Behaviors.GetNextBehavior<IAccessPropertyBehavior<T>>().SetValue(vm, value);
      }
   }

   public class VMProperty<T> : VMPropertyBase<T> {
   }
}
