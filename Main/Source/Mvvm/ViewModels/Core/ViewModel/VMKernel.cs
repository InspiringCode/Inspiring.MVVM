namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Tracing;

   public sealed class VMKernel : IBehaviorContext {
      private readonly IViewModel _vm;
      private readonly IVMDescriptor _descriptor;
      private FieldValueHolder _fieldValues;

      private CountingSet<IVMCollection> _ownerCollections = new CountingSet<IVMCollection>();
      private CountingSet<IViewModel> _parents = new CountingSet<IViewModel>();

      public VMKernel(IViewModel vm, IVMDescriptor descriptor, IServiceLocator serviceLocator) {
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
         get { return GetValidationResult(ValidationResultScope.All).IsValid; }
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

      public void Load(IVMPropertyDescriptor property) {
         var dummyValue = GetValue(property);
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

      public ValidationResult GetValidationResult(ValidationResultScope scope = ValidationResultScope.All) {
         return _descriptor
            .Behaviors
            .GetValidationResultNext(this, scope);
      }

      public ValidationResult GetValidationResult(IVMPropertyDescriptor forProperty) {
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

      public void Refresh(
         bool executeRefreshDependencies = false
      ) {
         RefreshTrace.BeginManualRefresh();
         RefreshInternal(executeRefreshDependencies);
         RefreshTrace.EndLastRefresh();
      }

      public void Refresh(
         IVMPropertyDescriptor property, 
         bool executeRefreshDependencies = false
      ) {
         RefreshTrace.BeginManualRefresh();
         RefreshInternal(property, executeRefreshDependencies);
         RefreshTrace.EndLastRefresh();
      }

      public void AddParent(IViewModel parent) {
         Parents.Add(parent);
      }

      public void RemoveParent(IViewModel parent) {
         Parents.Remove(parent);
      }

      public void Revalidate(ValidationScope scope) {
         Revalidator.Revalidate(_vm, scope);
      }

      public void Revalidate(
         IVMPropertyDescriptor property,
         ValidationScope scope = ValidationScope.Self
      ) {
         Revalidator.RevalidatePropertyValidations(_vm, property, scope);
      }

      public IBehaviorContext GetContext() {
         return this;
      }

      internal void RefreshInternal(bool executeRefreshDependencies = false) {
         RefreshWithoutValidation(executeRefreshDependencies);
         Revalidator.Revalidate(_vm, ValidationScope.SelfAndLoadedDescendants);
      }

      internal void RefreshWithoutValidation(bool executeRefreshDependencies = false) {
         _descriptor.Behaviors.ViewModelRefreshNext(this, executeRefreshDependencies);
      }

      internal void RefreshInternal(IVMPropertyDescriptor property, bool executeRefreshDependencies = false) {
         _descriptor.Behaviors.ViewModelRefreshNext(this, property, executeRefreshDependencies);

         Revalidator.RevalidatePropertyValidations(
            _vm,
            property,
            ValidationScope.SelfAndLoadedDescendants
         );
      }

      private void NotifyChangeCore(ChangeArgs args) {
         CallPropertyChangedHandlerBehaviors(args);
         CallChangeHandlerBehaviors(args);
         ForwardChangeNotificationToParents(args);
         ForwardChangeToViewModel(args);
      }

      private void CallPropertyChangedHandlerBehaviors(ChangeArgs args) {
         if (args.ChangeType != ChangeType.PropertyChanged) {
            return;
         }

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
         // Parents have to be cached, because parents may changed by change handlers (declarative dependencies).
         foreach (var parent in Parents.ToArray()) {
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
