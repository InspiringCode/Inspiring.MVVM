namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMPropertyDescriptor : IVMPropertyDescriptor {
      internal VMPropertyDescriptor(Type propertyType) {
         Contract.Requires(propertyType != null);

         Behaviors = new BehaviorChain();
         PropertyType = propertyType;
      }

      internal VMPropertyDescriptor(Type propertyType, string propertyName)
         : this(propertyType) {
         Contract.Requires(propertyName != null);

         Initialize(propertyName);
      }

      public BehaviorChain Behaviors { get; set; }

      public string PropertyName { get; private set; }

      public Type PropertyType { get; private set; }

      public void Initialize(string propertyName) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         PropertyName = propertyName;
      }

      //internal abstract void ConfigureBehaviors(BehaviorConfiguration configuration, VMDescriptorBase descriptor);


      [Obsolete]
      internal void OnPropertyChanged(IBehaviorContext vm) {
         //IHandlePropertyChangingBehavior changingBehavior;
         //if (Behaviors.TryGetBehavior(out changingBehavior)) {
         //   changingBehavior.HandlePropertyChanging(vm);
         //}

         IHandlePropertyChangedBehavior changedBehavior;
         if (Behaviors.TryGetBehavior(out changedBehavior)) {
            changedBehavior.HandlePropertyChanged(vm);
         }
      }

      //internal ValidationResult GetValidationResult(IBehaviorContext context) {
      //   IValidationBehavior validationBehavior;
      //   if (Behaviors.TryGetBehavior(out validationBehavior)) {
      //      return validationBehavior.GetValidationResult(context);
      //   }

      //   return ValidationResult.Success();
      //}

      //internal bool IsMutable(IBehaviorContext context) {
      //   IMutabilityCheckerBehavior checker;
      //   if (Behaviors.TryGetBehavior(out checker)) {
      //      return checker.IsMutable(context);
      //   }
      //   return true;
      //}


      object IVMPropertyDescriptor.GetValue(IBehaviorContext context) {
         return GetValueCore(context);
      }


      //object IVMProperty.GetDisplayValue(IBehaviorContext context) {
      //   return GetDisplayValue(context);
      //}

      //void IVMProperty.SetValue(IBehaviorContext context, object value) {
      //   SetValueCore(context, value);
      //}

      internal object GetDisplayValue(IBehaviorContext context) {
         Contract.Requires(context != null);

         return Behaviors
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .GetDisplayValue(context);
      }

      internal void SetDisplayValue(IBehaviorContext context, object value) {
         Contract.Requires(context != null);

         Behaviors
            .GetNextBehavior<IDisplayValueAccessorBehavior>()
            .SetDisplayValue(context, value);
      }

      protected abstract object GetValueCore(IBehaviorContext context);

      protected abstract void SetValueCore(IBehaviorContext context, object value);


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
         Contract.Requires(context != null);

         return Behaviors
            .GetNextBehavior<IValueAccessorBehavior<T>>()
            .GetValue(context);
      }

      internal void SetValue(IBehaviorContext context, T value) {
         Contract.Requires(context != null);

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
