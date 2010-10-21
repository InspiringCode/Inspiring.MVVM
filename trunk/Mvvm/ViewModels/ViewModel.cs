namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;


   public abstract partial class ViewModel : INotifyPropertyChanged, IDataErrorInfo, ISupportsValidation {
      private FieldValueHolder _dynamicFieldValues = null;
      // TODO, HACK: Make private!
      protected VMDescriptor _descriptor;
      private List<string> _viewModelErrors = new List<string>();

      internal ViewModel(IServiceLocator serviceLocator) {
         ServiceLocator = serviceLocator;
      }

      internal ViewModel(VMDescriptor descriptor, IServiceLocator serviceLocator)
         : this(serviceLocator) {
         Contract.Requires<ArgumentNullException>(descriptor != null);
         InitializeWithDescriptor(descriptor);
      }

      public IServiceLocator ServiceLocator { get; private set; }

      internal ViewModel Parent { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      string IDataErrorInfo.Error {
         get {
            return _viewModelErrors.FirstOrDefault();
         }
      }

      string IDataErrorInfo.this[string columnName] {
         get {
            RequireDescriptor();
            VMProperty property = _descriptor.Properties.SingleOrDefault(x => x.PropertyName == columnName);
            if (property != null) {
               ValidationResult result = ValidateProperty(property);
               return result.Successful ?
                  null :
                  result.ErrorMessage;
            }

            return null;
         }
      }

      // TODO: Is it possible to make it non-virtual?
      public virtual bool IsValid(bool validateChildren) {
         if (validateChildren) {
            return
               _viewModelErrors.Count == 0 &&
               _descriptor
                  .Properties
                  .All(p => {
                     // HACK: We actually want the real value, not the possibly converted
                     // display value...
                     ISupportsValidation childVM = p.GetDisplayValue(this) as ISupportsValidation;

                     return validateChildren && childVM != null ?
                        ValidateProperty(p).Successful && childVM.IsValid(true) :
                        ValidateProperty(p).Successful;
                  });
         } else {
            // Make sure the value of the properties not accessed in this case because
            // it may trigger lazy loading.
            return
               _viewModelErrors.Count == 0 &&
               _descriptor
                  .Properties
                  .All(p => ValidateProperty(p).Successful);
         }


      }

      internal bool AreChildrenValid(bool validateGrandchildren) {
         return _descriptor
            .Properties
            .All(p => {
               // HACK: We actually want the real value, not the possibly converted
               // display value...
               ISupportsValidation childVM = p.GetDisplayValue(this) as ISupportsValidation;

               ICanValidateChildrenHack hack = childVM as ICanValidateChildrenHack;

               if (hack != null) {
                  return hack.AreItemsValid(validateItemChildren: false);
               }

               return childVM != null ?
                  childVM.IsValid(validateGrandchildren) :
                  true;
            });
      }

      internal void InitializeWithDescriptor(VMDescriptor descriptor) {
         Contract.Requires(descriptor != null);
         if (_descriptor != null) {
            // TODO
            //throw new InvalidOperationException();
         }
         _descriptor = descriptor;
      }

      protected internal void SetValue<T>(VMPropertyBase<T> property, T value) {
         Contract.Requires<ArgumentNullException>(property != null);
         property.SetValue(this, value);
      }

      protected internal T GetValue<T>(VMPropertyBase<T> property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.GetValue(this);
      }

      protected virtual void OnPropertyChanged<T>(VMPropertyBase<T> property) {
         OnPropertyChanged(property.PropertyName);
         InvokeValidate(this, property);
      }

      protected virtual void OnValidationStateChanged<T>(VMPropertyBase<T> property) {
         InvokeValidate(this, property);
      }

      protected virtual void OnPropertyChanged(string propertyName) {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      protected virtual void OnChildChanged<T>(
         ViewModel changedChild,
         VMPropertyBase<T> changedProperty
      ) {
         InvokeValidate(changedChild, changedProperty);
         if (Parent != null) {
            Parent.OnChildChanged(changedChild, changedProperty);
         }
      }

      protected virtual void OnChildValidationStateChanged<T>(
         ViewModel changedChild,
         VMPropertyBase<T> changedProperty
      ) {
         InvokeValidate(changedChild, changedProperty);
         if (Parent != null) {
            Parent.OnChildValidationStateChanged(changedChild, changedProperty);
         }
      }

      protected virtual void OnValidate(ViewModelValidationArgs args) {
      }


      // TODO: Does this have to be internal? Isn't there a better way?
      internal void InvokeValidate(ViewModel changedVM, VMProperty changedProperty) {
         string oldError = _viewModelErrors.FirstOrDefault();

         var args = new ViewModelValidationArgs(
            this,
            changedVM,
            changedProperty
         );

         _descriptor.Validators.ForEach(v => v(args));
         OnValidate(args);

         _viewModelErrors = args.Errors;

         string newError = _viewModelErrors.FirstOrDefault();

         if (newError != oldError) {
            OnPropertyChanged("Error");
         }
      }

      //protected virtual ValidationResult Validate() {



      //}

      /// <summary>
      ///   This method is only a temporary solution for validation. It will be
      ///   replaced with behaviors in the next version.
      /// </summary>
      protected virtual ValidationResult ValidateProperty(VMProperty property) {
         return property.GetValidationResult(this);
      }

      internal protected void Revalidate() {
         RequireDescriptor();
         foreach (VMProperty property in _descriptor.Properties) {
            Revalidate(property);
         }
         InvokeValidate(this, null);
      }

      protected void Revalidate(VMProperty property) {
         property.Revalidate(this);
      }

      protected void UpdateFromSource(VMProperty property) {
         property.Behaviors
            .GetNextBehavior<IManuelUpdateBehavior>()
            .UpdateFromSource(this);
      }

      protected void UpdateSource(VMProperty property) {
         property.Behaviors
            .GetNextBehavior<IManuelUpdateBehavior>()
            .UpdateSource(this);
      }

      private void RequireDescriptor() {
         if (_descriptor == null) {
            throw new InvalidOperationException(); // TODO: Good exception
         }
      }
   }

   partial class ViewModel : IBehaviorContext {
      public IVMContext VMContext { get; set; }

      public event EventHandler<ValidationEventArgs> Validating;

      public event EventHandler<ValidationEventArgs> Validated;

      FieldValueHolder IBehaviorContext.FieldValues {
         get {
            RequireDescriptor();
            if (_dynamicFieldValues == null) {
               _dynamicFieldValues = _descriptor.DynamicFields.CreateValueHolder();
            }
            return _dynamicFieldValues;
         }
      }

      ViewModel IBehaviorContext.VM {
         get { return this; }
      }

      void IBehaviorContext.RaisePropertyChanged<T>(VMPropertyBase<T> property) {
         property.OnPropertyChanged(this);
         OnPropertyChanged(property);
         if (Parent != null) {
            Parent.OnChildChanged(this, property);
         }
      }

      void IBehaviorContext.RaiseValidationStateChanged<T>(VMPropertyBase<T> property) {
         OnPropertyChanged("Item[]");
         if (Parent != null) {
            Parent.OnChildValidationStateChanged(this, property);
         }
      }

      void IBehaviorContext.OnValidating(ValidationEventArgs args) {
         var handler = Validating;
         if (handler != null) {
            handler(this, args);
         }
      }

      void IBehaviorContext.OnValidated(ValidationEventArgs args) {
         var handler = Validated;
         if (handler != null) {
            handler(this, args);
         }
      }
   }

   partial class ViewModel : ICustomTypeDescriptor {
      PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
         RequireDescriptor();
         return _descriptor.PropertyDescriptors;
      }

      PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
         RequireDescriptor();
         if (attributes != null && attributes.Length > 0) {
            throw new NotSupportedException(ExceptionTexts.GetPropertiesWithAttributesIsNotSupport);
         }

         return _descriptor.PropertyDescriptors;
      }

      object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
         return this;
      }

      AttributeCollection ICustomTypeDescriptor.GetAttributes() {
         return TypeDescriptor.GetAttributes(this, true);
      }

      EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
         return TypeDescriptor.GetEvents(this, true);
      }

      EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
         return TypeDescriptor.GetEvents(this, attributes, true);
      }

      EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
         return TypeDescriptor.GetDefaultEvent(this, true);
      }

      PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
         return TypeDescriptor.GetDefaultProperty(this, true);
      }

      string ICustomTypeDescriptor.GetClassName() {
         return TypeDescriptor.GetClassName(this, true);
      }

      string ICustomTypeDescriptor.GetComponentName() {
         return TypeDescriptor.GetComponentName(this, true);
      }

      TypeConverter ICustomTypeDescriptor.GetConverter() {
         return TypeDescriptor.GetConverter(this, true);
      }

      object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
         return TypeDescriptor.GetEditor(this, editorBaseType, true);
      }
   }

   public abstract partial class ViewModel<TDescriptor> : ViewModel
      where TDescriptor : VMDescriptor {

      public ViewModel(IServiceLocator serviceLocator = null)
         : base(serviceLocator ?? Inspiring.Mvvm.ServiceLocator.Current) {
      }

      public ViewModel(TDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator ?? Inspiring.Mvvm.ServiceLocator.Current) {
      }
   }
}