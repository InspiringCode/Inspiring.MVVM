namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core.Validation;

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

      //
      // IBehaviorContext implementation
      //

      public IServiceLocator ServiceLocator {
         get;
         private set;
      }

      IViewModel IBehaviorContext.VM {
         get { return _vm; }
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
         NotifyChangeCore(args.PrependViewModel(_vm));
      }


      public bool IsValid {
         get { return GetValidationState(ValidationResultScope.All).IsValid; }
      }

      public UndoManager UndoManager {
         get {
            var manager = UndoManager.GetManager(_vm);
            if (manager == null) {
               throw new InvalidOperationException(ExceptionTexts.NoUndoRootManagerFound);
            }
            return manager;
         }
      }

      public CountingSet<IViewModel> Parents {
         get { return _parents; }
      }

      internal CountingSet<IVMCollection> OwnerCollections {
         get { return _ownerCollections; }
      }

      public bool IsLoaded(IVMPropertyDescriptor property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.Behaviors.IsLoadedNext(this);
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

      public T GetValue<T>(IVMPropertyDescriptor<T> property) {
         return property.Behaviors.GetValueNext<T>(this);
      }

      public void SetValue<T>(IVMPropertyDescriptor<T> property, T value) {
         property.Behaviors.SetValueNext<T>(this, value);
      }

      public T GetValidatedValue<T>(IVMPropertyDescriptor<T> property) {
         return property.Behaviors.GetValidatedValueNext<T>(this);
      }

      public ValidationResult GetValidationState(ValidationResultScope scope = ValidationResultScope.All) {
         return _descriptor
            .Behaviors
            .GetValidationResultNext(this, ValidationResultScope.All);
      }

      public ValidationResult GetValidationState(IVMPropertyDescriptor forProperty) {
         // TODO: Is it a good idea to extract the state for a property from the _propertiesValidationState?
         // It would be faster, but would it preserve all sematics and so?
         return forProperty.Behaviors.GetValidationResultNext(this);
      }

      public IVMPropertyDescriptor GetProperty(string propertyName) {
         return _descriptor.Properties[propertyName];
      }

      public void UpdateFromSource() {
         throw new NotImplementedException();
         //_descriptor.Behaviors.UpdateFromSourceNext(this);
      }

      public void UpdateFromSource(IVMPropertyDescriptor property) {
         throw new NotImplementedException();
         //_descriptor.Behaviors.UpdateFromSourceNext(this, property);
      }

      public void UpdateSource() {
         throw new NotImplementedException();
         //_descriptor.Behaviors.UpdateSourceNext(this);
      }

      public void UpdateSource(IVMPropertyDescriptor property) {
         throw new NotImplementedException();
         //_descriptor.Behaviors.UpdateSourceNext(this, property);
      }

      public void Refresh() {
         _descriptor.Behaviors.ViewModelRefreshNext(this);
      }

      public void Refresh(IVMPropertyDescriptor property) {
         _descriptor.Behaviors.ViewModelRefreshNext(this, property);
      }

      public void AddParent(IViewModel parent) {
         Parents.Add(parent);
      }

      public void RemoveParent(IViewModel parent) {
         Parents.Remove(parent);
      }

      public void Revalidate(ValidationScope scope, ValidationMode mode) {
         Revalidator.Revalidate(_vm, scope);
      }

      public void Revalidate(
         IVMPropertyDescriptor property,
         ValidationMode mode,
         ValidationScope scope = ValidationScope.SelfOnly
      ) {
         Revalidator.RevalidatePropertyValidations(_vm, property, scope);
      }

      public IBehaviorContext GetContext() {
         return this;
      }

      private void NotifyChangeCore(ChangeArgs args) {
         CallPropertyChangedHandlerBehaviors(args);
         CallChangeHandlerBehaviors(args);
         ForwardChangeNotificationToParents(args);
         ForwardChangeToViewModel(args);
      }

      private void CallPropertyChangedHandlerBehaviors(ChangeArgs args) {
         var result = args
            .ChangedPath
            .SelectsOnlyPropertyOf(_vm);

         bool ownPropertyChanged = result.Success;

         if (ownPropertyChanged) {
            result
               .Property
               .Behaviors
               .HandlePropertyChangedNext(GetContext());
         }
      }

      private void CallChangeHandlerBehaviors(ChangeArgs args) {
         _descriptor
            .Behaviors
            .HandleChangedNext(this, args);
      }

      private void ForwardChangeNotificationToParents(ChangeArgs args) {
         foreach (var parent in Parents) {
            parent
               .Kernel
               .GetContext()
               .NotifyChange(args);
         }
      }

      private void ForwardChangeToViewModel(ChangeArgs args) {
         _vm.NotifyChange(args);
      }
   }
}
