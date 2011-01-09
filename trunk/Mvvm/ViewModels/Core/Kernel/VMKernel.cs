namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;

   public sealed class VMKernel : IBehaviorContext {
      private readonly IViewModel _vm;
      private readonly VMDescriptorBase _descriptor;
      private FieldValueHolder _fieldValues;

      private ValidationState _viewModelValidationState = ValidationState.Valid;
      private ValidationState _propertiesValidationState = ValidationState.Valid;
      private ValidationState _selfOnlyValidationState = ValidationState.Valid;
      private ValidationState _descendantsOnlyValidationState = ValidationState.Valid;
      private ValidationState _validationState = ValidationState.Valid;

      public VMKernel(IViewModel vm, VMDescriptorBase descriptor, IServiceLocator serviceLocator) {
         Contract.Requires<ArgumentNullException>(vm != null);
         Contract.Requires<ArgumentNullException>(descriptor != null);
         Contract.Requires<ArgumentNullException>(serviceLocator != null);

         _vm = vm;
         _descriptor = descriptor;
         ServiceLocator = serviceLocator;
      }

      public IViewModel Parent { get; set; }

      public IServiceLocator ServiceLocator {
         get;
         private set;
      }

      IViewModel IBehaviorContext.VM {
         get { return _vm; }
      }

      internal IEnumerable OwnerCollection { get; set; }

      FieldValueHolder IBehaviorContext.FieldValues {
         get {
            if (_fieldValues == null) {
               _fieldValues = _descriptor.Fields.CreateValueHolder();
            }

            return _fieldValues;
         }
      }

      void IBehaviorContext.NotifyChange(ChangeArgs args) {
         NotifyChange(args, InstancePath.Empty);
      }


      void IBehaviorContext.NotifyValidating(ValidationArgs args) {
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }

      public T GetValue<T>(IVMProperty<T> property) {
         return property.Behaviors.GetValueNext<T>(this);
      }

      public T GetValidatedValue<T>(IVMProperty<T> property) {
         return property.Behaviors.GetValidatedValueNext<T>(this);
      }

      public void SetValue<T>(IVMProperty<T> property, T value) {
         property.Behaviors.SetValueNext<T>(this, value);
      }

      public object GetDisplayValue(IVMProperty property) {
         return property.Behaviors.GetDisplayValueNext(this);
      }

      public void SetDisplayValue(IVMProperty property, object value) {
         property.Behaviors.SetDisplayValueNext(this, value);
      }

      public ValidationState GetValidationState(IVMProperty forProperty) {
         return forProperty.Behaviors.GetValidationStateNext(this);
      }

      public ValidationState GetValidationState(ValidationStateScope scope = ValidationStateScope.All) {
         switch (scope) {
            case ValidationStateScope.All:
               return _validationState;
            case ValidationStateScope.Self:
               return _selfOnlyValidationState;
            case ValidationStateScope.Descendants:
               return _descendantsOnlyValidationState;
            case ValidationStateScope.ViewModelValidationsOnly:
               return _viewModelValidationState;
            case ValidationStateScope.PropertiesOnly:
               return _propertiesValidationState;
            default:
               throw new NotSupportedException();
         }
      }

      public IVMProperty GetProperty(string propertyName) {
         return _descriptor.Properties[propertyName];
      }

      public void UpdateFromSource() {
         _descriptor.Behaviors.UpdateFromSourceNext(this);
      }

      public void UpdateFromSource(IVMProperty property) {
         _descriptor.Behaviors.UpdateFromSourceNext(this, property);
      }

      public void UpdateSource() {
         _descriptor.Behaviors.UpdateSourceNext(this);
      }

      public void UpdateSource(IVMProperty property) {
         _descriptor.Behaviors.UpdateSourceNext(this, property);
      }

      public void Revalidate(ValidationScope scope, ValidationMode mode) {
         ValidationContext.BeginValidation();
         Revalidate(ValidationContext.Current, scope, mode);
         ValidationContext.CompleteValidation(mode);
      }

      internal void Revalidate(
         ValidationContext validationContext,
         ValidationScope scope,
         ValidationMode mode
      ) {
         if (scope == ValidationScope.SelfAndValidatedChildren) {
            throw new NotImplementedException("Still TODO");
         }

         if (scope == ValidationScope.FullSubtree) {
            foreach (IVMProperty property in _descriptor.Properties) {
               property
                  .Behaviors
                  .RevalidateDescendantsNext(this, validationContext, scope, mode);
            }
         }

         foreach (IVMProperty property in _descriptor.Properties) {
            Revalidate(property, validationContext, mode);
         }

         _descriptor
            .Behaviors
            .GetNextBehavior<ViewModelValidationBehavior>()
            .Validate(this, validationContext);
      }

      private void Revalidate(
         IVMProperty property,
         ValidationContext validationContext,
         ValidationMode mode
      ) {
         property.Behaviors.RevalidateNext(this, validationContext, mode);
      }

      private void NotifyChange(ChangeArgs args, InstancePath changedPath) {
         bool selfChanged = changedPath.IsEmpty;
         changedPath = changedPath.PrependVM(_vm);

         if (selfChanged && args.ChangeType == ChangeType.PropertyChanged) {
            args
               .ChangedProperty
               .Behaviors
               .TryCall<IHandlePropertyChangedBehavior>(b =>
                  b.HandlePropertyChanged(this)
               );
         }

         if (args.ChangeType == ChangeType.ValidationStateChanged) {
            UpdateValidationState();
         }

         ViewModelBehavior behavior;
         if (_descriptor.Behaviors.TryGetBehavior(out behavior)) {
            if (selfChanged) {
               behavior.OnSelfChanged(this, args);
            } else {
               behavior.OnChildChanged(this, args, changedPath);
            }

            behavior.OnChanged(this, args, changedPath);
         }

         if (Parent != null) {
            Parent.Kernel.NotifyChange(args, changedPath);
         }

         if (args.ChangeType == ChangeType.PropertyChanged && args.ChangedVM == _vm) {
            _vm.NotifyPropertyChanged(args.ChangedProperty);
         }

         if (args.ChangeType == ChangeType.ValidationStateChanged && args.ChangedVM == _vm) {
            _vm.NotifyValidationStateChanged(args.ChangedProperty);
         }
      }

      private void HandleNotifyValidating(ValidationArgs args) {
         _descriptor.Behaviors.TryCall<ViewModelBehavior>(b =>
            b.OnValidating(this, args)
         );
      }

      private void NotifyValidating(ValidationArgs args) {
         args = args.PrependTargetPath(with: _vm);
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }

      private void UpdateValidationState() {
         _viewModelValidationState = _descriptor.Behaviors.GetValidationStateNext(this);

         _propertiesValidationState = ValidationState.Join(
            _descriptor
               .Properties
               .Select(x => GetValidationState(x))
               .ToArray()
         );

         _selfOnlyValidationState = ValidationState.Join(
            _propertiesValidationState,
            _viewModelValidationState
         );

         _descendantsOnlyValidationState = ValidationState.Join(
            _descriptor
               .Properties
               .Select(x => x.Behaviors.GetDescendantsValidationStateNext(this))
               .ToArray()
         );

         _validationState = ValidationState.Join(
            _selfOnlyValidationState,
            _descendantsOnlyValidationState
         );
      }
   }
}
