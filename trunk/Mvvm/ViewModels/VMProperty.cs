namespace Inspiring.Mvvm.ViewModels {
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

      internal VMDescriptor Descriptor { get; private set; }

      internal VMPropertyDescriptor PropertyDescriptor { get; set; }

      public void Initialize(string propertyName) {
         Contract.Requires<ArgumentNullException>(propertyName != null);

         PropertyName = propertyName;
      }

      [Obsolete]
      internal void Initialize(string propertyName, VMDescriptor descriptor) {
         PropertyName = propertyName;
         Descriptor = descriptor;
      }

      internal abstract void ConfigureBehaviors(BehaviorConfiguration configuration, FieldDefinitionCollection fieldDefinitions);

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

         // HACK: Is there a neater way to seperate the Descriptor stuff?
         if (PropertyDescriptor != null) {
            PropertyDescriptor.RaiseValueChanged(vm);
         }
      }

      internal abstract void Revalidate(IBehaviorContext context);

      internal ValidationResult GetValidationResult(IBehaviorContext context) {
         IValidationBehavior validationBehavior;
         if (Behaviors.TryGetBehavior(out validationBehavior)) {
            return validationBehavior.GetValidationResult(context);
         }

         return ValidationResult.Success();
      }

      internal bool IsMutable(IBehaviorContext context) {
         IMutabilityCheckerBehavior checker;
         if (Behaviors.TryGetBehavior(out checker)) {
            return checker.IsMutable(context);
         }
         return true;
      }
   }

   public abstract class VMPropertyBase<T> : VMPropertyBase, IVMProperty<T> {

      public VMPropertyBase()
         : base(typeof(T)) {
      }

      internal override void ConfigureBehaviors(BehaviorConfiguration configuration, FieldDefinitionCollection fieldDefinitions) {
         Behaviors = configuration.CreateBehaviorChain<T>();
         ((IBehavior)Behaviors).Initialize(new BehaviorInitializationContext(fieldDefinitions, this));
      }

      internal T GetValue(IBehaviorContext vm) {
         Contract.Requires(vm != null);
         return Behaviors.GetNextBehavior<IAccessPropertyBehavior<T>>().GetValue(vm);
      }

      internal void SetValue(IBehaviorContext vm, T value) {
         Contract.Requires(vm != null);
         Behaviors.GetNextBehavior<IAccessPropertyBehavior<T>>().SetValue(vm, value);
      }

      internal override void Revalidate(IBehaviorContext context) {
         if (IsMutable(context)) {
            IAccessPropertyBehavior displayValueAccessor;
            if (Behaviors.TryGetBehavior(out displayValueAccessor)) {
               object value = displayValueAccessor.GetValue(context);
               displayValueAccessor.SetValue(context, value);
            } else {
               var typedAccessor = Behaviors.GetNextBehavior<IAccessPropertyBehavior<T>>();
               T value = typedAccessor.GetValue(context);
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
