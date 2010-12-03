﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class VMPropertyBase : IVMProperty {
      internal VMPropertyBase(Type propertyType) {
         Contract.Requires(propertyType != null);

         PropertyType = propertyType;
      }

      internal VMPropertyBase(Type propertyType, string propertyName)
         : this(propertyType) {
         Contract.Requires(propertyName != null);

         Initialize(propertyName);
      }

      public Behavior Behaviors { get; internal set; }

      public string PropertyName { get; private set; }

      public Type PropertyType { get; private set; }

      public void Initialize(string propertyName) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         PropertyName = propertyName;
      }

      internal abstract void ConfigureBehaviors(BehaviorConfiguration configuration, VMDescriptorBase descriptor);


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

      internal abstract void Revalidate(IBehaviorContext context);

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


      object IVMProperty.GetValue(IBehaviorContext context, ValueStage stage) {
         return GetValueCore(context, stage);
      }

      void IVMProperty.SetValue(IBehaviorContext context, object value) {
         SetValueCore(context, value);
      }

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

      protected abstract object GetValueCore(IBehaviorContext context, ValueStage stage);

      protected abstract void SetValueCore(IBehaviorContext context, object value);
   }

   public abstract class VMPropertyBase<T> : VMPropertyBase, IVMProperty<T> {

      public VMPropertyBase()
         : base(typeof(T)) {
      }

      internal override void ConfigureBehaviors(BehaviorConfiguration configuration, VMDescriptorBase descriptor) {
         Behaviors = configuration.CreateBehaviorChain<T>();
         ((IBehavior)Behaviors).Initialize(new BehaviorInitializationContext(descriptor, this));
      }

      internal T GetValue(IBehaviorContext context, ValueStage stage) {
         Contract.Requires(context != null);

         return Behaviors
            .GetNextBehavior<IValueAccessorBehavior<T>>()
            .GetValue(context, stage);
      }

      internal void SetValue(IBehaviorContext context, T value) {
         Contract.Requires(context != null);

         Behaviors
            .GetNextBehavior<IValueAccessorBehavior<T>>()
            .SetValue(context, value);
      }

      protected override object GetValueCore(IBehaviorContext context, ValueStage stage) {
         return stage == ValueStage.PreConversion ?
            GetDisplayValue(context) :
            GetValue(context, stage);
      }

      protected override void SetValueCore(IBehaviorContext context, object value) {
         SetDisplayValue(context, value);
      }

      [Obsolete]
      internal override void Revalidate(IBehaviorContext context) {
         if (this.IsMutable(context)) {
            IDisplayValueAccessorBehavior displayValueAccessor;
            if (Behaviors.TryGetBehavior(out displayValueAccessor)) {
               object value = displayValueAccessor.GetDisplayValue(context);
               displayValueAccessor.SetDisplayValue(context, value);
            } else {
               var typedAccessor = Behaviors.GetNextBehavior<IValueAccessorBehavior<T>>();
               T value = typedAccessor.GetValue(context, ValueStage.PostValidation);
               typedAccessor.SetValue(context, value);
            }
         } else {
            // TODO: Implement validation for readonly properties!
         }
      }
   }

   public class VMProperty<T> : VMPropertyBase<T> {
   }
}
