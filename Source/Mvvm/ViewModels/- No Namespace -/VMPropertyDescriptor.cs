namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMPropertyDescriptor : IVMPropertyDescriptor {
      internal VMPropertyDescriptor(Type propertyType) {
         Check.NotNull(propertyType, nameof(propertyType));

         Behaviors = new BehaviorChain();
         PropertyType = propertyType;
      }

      internal VMPropertyDescriptor(Type propertyType, string propertyName)
         : this(propertyType) {
         Check.NotNull(propertyName, nameof(propertyName));

         Initialize(propertyName);
      }

      public BehaviorChain Behaviors { get; set; }

      public string PropertyName { get; private set; }

      public Type PropertyType { get; private set; }

      public void Initialize(string propertyName) {
         Check.NotNull(propertyName, nameof(propertyName));

         PropertyName = propertyName;
      }

      object IVMPropertyDescriptor.GetValue(IBehaviorContext context) {
         return GetValueCore(context);
      }

      internal object GetDisplayValue(IBehaviorContext context) {
         Check.NotNull(context, nameof(context));

         return Behaviors
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .GetDisplayValue(context);
      }

      internal void SetDisplayValue(IBehaviorContext context, object value) {
         Check.NotNull(context, nameof(context));

         Behaviors
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .SetDisplayValue(context, value);
      }

      protected abstract object GetValueCore(IBehaviorContext context);

      protected abstract void SetValueCore(IBehaviorContext context, object value);

      public override string ToString() {
         return PropertyName ?? base.ToString();
      }

   }

   public class VMPropertyDescriptor<T> : VMPropertyDescriptor, IVMPropertyDescriptor<T> {

      public VMPropertyDescriptor()
         : base(typeof(T)) {
      }

      //internal override void ConfigureBehaviors(BehaviorConfiguration configuration, VMDescriptorBase descriptor) {
      //   Behaviors = configuration.CreateBehaviorChain<T>();
      //   throw new NotImplementedException();
      //   //((IBehavior)Behaviors).Initialize(new BehaviorInitializationContext(descriptor, this));
      //}

      internal T GetValue(IBehaviorContext context) {
         Check.NotNull(context, nameof(context));

         return Behaviors
            .GetNextBehavior<IValueAccessorBehavior<T>>()
            .GetValue(context);
      }

      internal void SetValue(IBehaviorContext context, T value) {
         Check.NotNull(context, nameof(context));

         Behaviors
            .GetNextBehavior<IValueAccessorBehavior<T>>()
            .SetValue(context, value);
      }

      protected override object GetValueCore(IBehaviorContext context) {
         return GetValue(context);
      }

      protected override void SetValueCore(IBehaviorContext context, object value) {
         SetDisplayValue(context, value);
      }
   }

   //public class VMProperty<T> : VMProperty<T> {
   //}
}
