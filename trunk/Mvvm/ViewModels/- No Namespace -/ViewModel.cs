namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels.Core;

   // TODO: Rename/Refactor me!
   public class ViewModel {
      public static readonly FieldDefinitionGroup GeneralFieldGroup = new FieldDefinitionGroup();
   }

   public abstract class ViewModel<TDescriptor> :
      ViewModelTypeDescriptor,
      IViewModel,
      IViewModel<TDescriptor>,
      INotifyPropertyChanged,
      IDataErrorInfo
      where TDescriptor : VMDescriptorBase {

      public ViewModel(IServiceLocator serviceLocator = null) {
         ServiceLocator = serviceLocator ?? Mvvm.ServiceLocator.Current;
      }

      public ViewModel(TDescriptor descriptor, IServiceLocator serviceLocator = null) {
         Descriptor = descriptor;
         ServiceLocator = serviceLocator ?? Mvvm.ServiceLocator.Current;
      }

      private VMKernel _kernel;

      public IViewModel Parent {
         get { return Kernel.Parent; }
         set { Kernel.Parent = value; }
      }

      VMKernel IViewModel.Kernel {
         get { return Kernel; }
      }

      VMDescriptorBase IViewModel.Descriptor {
         get { return Descriptor; }
         set { Descriptor = (TDescriptor)value; }
      }

      protected TDescriptor Descriptor {
         get;
         private set;
      }

      protected IServiceLocator ServiceLocator {
         get;
         private set;
      }

      protected VMKernel Kernel {
         get {
            Contract.Requires<InvalidOperationException>(
               Descriptor != null,
               ExceptionTexts.DescriptorNotSet
            );

            Contract.Ensures(Contract.Result<VMKernel>() != null);

            if (_kernel == null) {
               _kernel = new VMKernel(this, Descriptor, ServiceLocator);
            }

            return _kernel;
         }
      }

      string IDataErrorInfo.Error {
         get {
            ValidationState state = Kernel.GetValidationState(ValidationStateScope.ViewModelValidationsOnly);
            return state.IsValid ?
               null :
               state.Errors.First().Message;
         }
      }

      string IDataErrorInfo.this[string columnName] {
         get {
            IVMProperty property = Kernel.GetProperty(propertyName: columnName);
            ValidationState state = Kernel.GetValidationState(property);
            return state.IsValid ?
               null :
               state.Errors.First().Message;
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected internal T GetValue<T>(IVMProperty<T> property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return Kernel.GetValue<T>(property);
      }

      protected internal void SetValue<T>(IVMProperty<T> property, T value) {
         Contract.Requires<ArgumentNullException>(property != null);
         property.SetValue(Kernel, value);
      }

      protected internal T GetValidatedValue<T>(IVMProperty<T> property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.Behaviors.GetValidatedValueNext<T>(Kernel); // TODO: Refactor.
      }

      protected internal object GetDisplayValue(IVMProperty property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.GetDisplayValue(Kernel);
      }

      protected internal void SetDisplayValue(IVMProperty property, object value) {
         Contract.Requires<ArgumentNullException>(property != null);
         Kernel.SetDisplayValue(property, value);
      }

      protected void Revalidate(ValidationScope scope, ValidationMode mode) {
         Kernel.Revalidate(scope, mode);
      }

      protected void UpdateFromSource() {
         Kernel.UpdateFromSource();
      }

      protected void UpdateFromSource(IVMProperty property) {
         Kernel.UpdateFromSource(property);
      }

      protected void UpdateSource() {
         Kernel.UpdateSource();
      }

      protected void UpdateSource(IVMProperty property) {
         Kernel.UpdateSource(property);
      }


      object IViewModel.GetValue(IVMProperty property) {
         return property.GetValue(Kernel);
      }

      void IViewModel.SetValue(IVMProperty property, object value) {
         property.SetValue(Kernel, value);
      }

      public IBehaviorContext GetContext() {
         return Kernel;
      }

      void IViewModel.NotifyPropertyChanged(IVMProperty property) {
         OnPropertyChanged(property);
      }

      void IViewModel.NotifyValidationStateChanged(IVMProperty property) {
         OnValidationStateChanged(property);
      }

      protected virtual void OnPropertyChanged(IVMProperty property) {
         OnPropertyChanged(property.PropertyName);
      }

      protected virtual void OnValidationStateChanged(IVMProperty property) {
         if (property != null) {
            OnPropertyChanged("Item[]");
         } else {
            OnPropertyChanged("Error");
         }
      }

      protected virtual void OnPropertyChanged(string propertyName) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      protected override PropertyDescriptorCollection GetPropertyDescriptors() {
         Contract.Requires<InvalidOperationException>(
            Descriptor != null,
            ExceptionTexts.DescriptorNotSet
         );

         return Descriptor
            .Behaviors
            .GetNextBehavior<TypeDescriptorBehavior>()
            .PropertyDescriptors;
      }
   }
}
