namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;

   public sealed class VMKernel : IBehaviorContext {
      private readonly IViewModel _vm;
      private readonly VMDescriptorBase _descriptor;
      private FieldValueHolder _fieldValues;

      private bool _validationStateIsCurrent = false;
      private bool _isValid = true;
      private ValidationResult _viewModelValidationState = ValidationResult.Valid;
      private ValidationResult _propertiesValidationState = ValidationResult.Valid;
      private ValidationResult _selfOnlyValidationState = ValidationResult.Valid;
      private ValidationResult _descendantsOnlyValidationState = ValidationResult.Valid;
      private ValidationResult _validationState = ValidationResult.Valid;

      private CountingSet<IVMCollection> _ownerCollections = new CountingSet<IVMCollection>();
      private CountingSet<IViewModel> _parents = new CountingSet<IViewModel>();

      public VMKernel(IViewModel vm, VMDescriptorBase descriptor, IServiceLocator serviceLocator) {
         Contract.Requires<ArgumentNullException>(vm != null);
         Contract.Requires<ArgumentNullException>(descriptor != null);
         Contract.Requires<ArgumentNullException>(serviceLocator != null);

         _vm = vm;
         _descriptor = descriptor;
         ServiceLocator = serviceLocator;
      }

      public CountingSet<IViewModel> Parents {
         get { return _parents; }
      }

      public IServiceLocator ServiceLocator {
         get;
         private set;
      }

      public bool IsValid {
         get {
            if (!_validationStateIsCurrent) {
               UpdateValidationState();
            }
            return _isValid;
         }
      }

      IViewModel IBehaviorContext.VM {
         get { return _vm; }
      }

      internal CountingSet<IVMCollection> OwnerCollections {
         get { return _ownerCollections; }
      }

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
         Parents.ForEach(x => x.Kernel.NotifyValidating(args));
      }

      public bool IsLoaded(IVMPropertyDescriptor property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.Behaviors.IsLoadedNext(this);
      }

      public T GetValue<T>(IVMPropertyDescriptor<T> property) {
         return property.Behaviors.GetValueNext<T>(this);
      }

      public T GetValidatedValue<T>(IVMPropertyDescriptor<T> property) {
         return property.Behaviors.GetValidatedValueNext<T>(this);
      }

      public void SetValue<T>(IVMPropertyDescriptor<T> property, T value) {
         property.Behaviors.SetValueNext<T>(this, value);
      }

      public object GetDisplayValue(IVMPropertyDescriptor property) {
         return property.Behaviors.GetDisplayValueNext(this);
      }

      public void SetDisplayValue(IVMPropertyDescriptor property, object value) {
         property.Behaviors.SetDisplayValueNext(this, value);
      }

      public object GetValue(IVMPropertyDescriptor property) {
         return property.Behaviors.GetValueNext(this);
      }

      public void SetValue(IVMPropertyDescriptor property, object value) {
         property.Behaviors.SetValueNext(this, value);
      }

      public ValidationResult GetValidationState(IVMPropertyDescriptor forProperty) {
         // TODO: Is it a good idea to extract the state for a property from the _propertiesValidationState?
         // It would be faster, but would it preserve all sematics and so?
         return forProperty.Behaviors.GetValidationStateNext(this);
      }

      public ValidationResult GetValidationState(ValidationStateScope scope = ValidationStateScope.All) {
         if (!_validationStateIsCurrent) {
            UpdateValidationState();
         }
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

      public IVMPropertyDescriptor GetProperty(string propertyName) {
         return _descriptor.Properties[propertyName];
      }

      public void UpdateFromSource() {
         _descriptor.Behaviors.UpdateFromSourceNext(this);
      }

      public void UpdateFromSource(IVMPropertyDescriptor property) {
         _descriptor.Behaviors.UpdateFromSourceNext(this, property);
      }

      public void UpdateSource() {
         _descriptor.Behaviors.UpdateSourceNext(this);
      }

      public void UpdateSource(IVMPropertyDescriptor property) {
         _descriptor.Behaviors.UpdateSourceNext(this, property);
      }

      public void Refresh() {
         _descriptor.Behaviors.ViewModelRefreshNext(this);

         PerformViewModelValidations(); // TODO: Should this be in the behavior?
      }

      public void Refresh(IVMPropertyDescriptor property) {
         _descriptor.Behaviors.ViewModelRefreshNext(this, property);
      }

      public IRollbackPoint GetRollbackPoint() {
         return UndoManager.GetManager(_vm).GetCurrentAction();
      }

      public void RollbackTo(IRollbackPoint point) {
         Contract.Requires<ArgumentNullException>(point != null);
         UndoManager.GetManager(_vm).Undo(point);
      }

      private void PerformViewModelValidations() {
         ValidationContext.BeginValidation();
         PerformViewModelValidations(ValidationContext.Current);
         ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);
      }

      private void PerformViewModelValidations(ValidationContext validationContext) {
         ViewModelValidationBehavior behavior;

         if (_descriptor.Behaviors.TryGetBehavior(out behavior)) {
            behavior.Validate(this, validationContext);
         }
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

         if (scope == ValidationScope.FullSubtree || scope == ValidationScope.SelfAndLoadedDescendants) { // TODO Only loaded descendants
            foreach (IVMPropertyDescriptor property in _descriptor.Properties) {
               property
                  .Behaviors
                  .RevalidateDescendantsNext(this, validationContext, scope, mode);
            }
         }

         foreach (IVMPropertyDescriptor property in _descriptor.Properties) {
            Revalidate(property, validationContext, mode);
         }

         PerformViewModelValidations(validationContext);
      }

      public void Revalidate(
         IVMPropertyDescriptor property,
         ValidationMode mode
      ) {
         ValidationContext.BeginValidation();
         Revalidate(property, ValidationContext.Current, mode);
         ValidationContext.CompleteValidation(mode);
      }

      private void Revalidate(
         IVMPropertyDescriptor property,
         ValidationContext validationContext,
         ValidationMode mode
      ) {
         property.Behaviors.RevalidateNext(this, validationContext, mode);
      }

      private void NotifyChange(ChangeArgs args, InstancePath changedPath) {
         bool selfChanged = changedPath.IsEmpty;
         changedPath = changedPath.PrependVM(_vm);
         args = args.PrependViewModel(_vm);

         if (selfChanged && args.ChangeType == ChangeType.PropertyChanged) {
            args
               .ChangedProperty
               .Behaviors
               .TryCall<IHandlePropertyChangedBehavior>(b =>
                  b.HandlePropertyChanged(this)
               );
         }

         bool validationStateChanged = args.ChangeType == ChangeType.ValidationStateChanged;
         bool collectionChanged =
            args.ChangeType == ChangeType.AddedToCollection ||
            args.ChangeType == ChangeType.RemovedFromCollection;

         bool viewModelPropertyChanged = args.ChangedProperty != null ?
            PropertyTypeHelper.IsViewModel(args.ChangedProperty.PropertyType) :
            false;

         if (validationStateChanged || collectionChanged || viewModelPropertyChanged) {
            _validationStateIsCurrent = false;
            //UpdateValidationState();
         }

         ViewModelBehavior behavior;
         if (_descriptor.Behaviors.TryGetBehavior(out behavior)) {
            //if (selfChanged) {
            //   behavior.OnSelfChanged(this, args);
            //} else {
            //   behavior.OnChildChanged(this, args, changedPath);
            //}

            behavior.OnChanged(this, args, changedPath);
         }

         _descriptor.Behaviors.HandleChangedNext(this, args);

         Parents.ForEach(x => x.Kernel.NotifyChange(args, changedPath));

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
         Parents.ForEach(x => x.Kernel.NotifyValidating(args));
      }

      private void UpdateValidationState() {
         _viewModelValidationState = _descriptor.Behaviors.GetValidationStateNext(this);

         _propertiesValidationState = ValidationResult.Join(
            _descriptor
               .Properties
               .Select(x => GetValidationState(x))
               .ToArray()
         );

         _selfOnlyValidationState = ValidationResult.Join(
            _propertiesValidationState,
            _viewModelValidationState
         );

         _descendantsOnlyValidationState = ValidationResult.Join(
            _descriptor
               .Properties
               .Select(x => x.Behaviors.GetDescendantsValidationStateNext(this))
               .ToArray()
         );

         _validationState = ValidationResult.Join(
            _selfOnlyValidationState,
            _descendantsOnlyValidationState
         );

         _isValid = _validationState.IsValid;
         _validationStateIsCurrent = true;
      }
   }
}
