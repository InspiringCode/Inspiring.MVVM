namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;



   public abstract partial class ViewModel : INotifyPropertyChanged {
      private FieldValueHolder _dynamicFieldValues = null;
      private VMDescriptor _descriptor;

      internal ViewModel(IServiceLocator serviceLocator) {
         ServiceLocator = serviceLocator;
      }

      internal ViewModel(VMDescriptor descriptor, IServiceLocator serviceLocator)
         : this(serviceLocator) {
         Contract.Requires<ArgumentNullException>(descriptor != null);
         InitializeWithDescriptor(descriptor);
      }

      public IServiceLocator ServiceLocator { get; private set; }

      public event PropertyChangedEventHandler PropertyChanged;

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
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(property.PropertyName));
         }
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

      FieldValueHolder IBehaviorContext.FieldValues {
         get {
            RequireDescriptor();
            if (_dynamicFieldValues == null) {
               _dynamicFieldValues = _descriptor.DynamicFields.CreateValueHolder();
            }
            return _dynamicFieldValues;
         }
      }

      void IBehaviorContext.RaisePropertyChanged<T>(VMPropertyBase<T> property) {
         property.OnPropertyChanged(this);
         OnPropertyChanged(property);
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