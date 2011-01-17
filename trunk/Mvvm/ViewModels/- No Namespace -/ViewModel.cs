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
            IVMPropertyDescriptor property = Kernel.GetProperty(propertyName: columnName);
            ValidationState state = Kernel.GetValidationState(property);
            return state.IsValid ?
               null :
               state.Errors.First().Message;
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected internal T GetValue<T>(IVMPropertyDescriptor<T> property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return Kernel.GetValue<T>(property);
      }

      protected internal void SetValue<T>(IVMPropertyDescriptor<T> property, T value) {
         Contract.Requires<ArgumentNullException>(property != null);
         Kernel.SetValue(property, value);
      }

      protected internal T GetValidatedValue<T>(IVMPropertyDescriptor<T> property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.Behaviors.GetValidatedValueNext<T>(Kernel); // TODO: Refactor.
      }

      protected internal object GetDisplayValue(IVMPropertyDescriptor property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return Kernel.GetDisplayValue(property);
      }

      protected internal void SetDisplayValue(IVMPropertyDescriptor property, object value) {
         Contract.Requires<ArgumentNullException>(property != null);
         Kernel.SetDisplayValue(property, value);
      }

      protected void Revalidate(ValidationScope scope, ValidationMode mode) {
         Kernel.Revalidate(scope, mode);
      }

      protected void CopyFromSource() {
         Kernel.UpdateFromSource();
      }

      protected void CopyFromSource(IVMPropertyDescriptor property) {
         Kernel.UpdateFromSource(property);
      }

      protected void CopyToSource() {
         Kernel.UpdateSource();
      }

      protected void CopyToSource(IVMPropertyDescriptor property) {
         Kernel.UpdateSource(property);
      }

      [Obsolete("Use Refresh")]
      protected void UpdateFromSource() {
         Kernel.UpdateFromSource();
      }

      [Obsolete("Use Refresh")]
      protected void UpdateFromSource(IVMPropertyDescriptor property) {
         Kernel.UpdateFromSource(property);
      }

      protected void Refresh() {
         //Console.WriteLine("Refresh is not correctly implemented yet. Strange behavior may be observed!");
         Kernel.Refresh();
      }

      protected void Refresh(IVMPropertyDescriptor property) {
         //Console.WriteLine("Refresh is not correctly implemented yet. Strange behavior may be observed!");
         Kernel.Refresh(property);
      }


      //object IViewModel.GetValue(IVMProperty property) {
      //   return property.GetValue(Kernel);
      //}

      //void IViewModel.SetValue(IVMProperty property, object value) {

      //   property.SetValue(Kernel, value);
      //}

      public IBehaviorContext GetContext() {
         return Kernel;
      }

      void IViewModel.NotifyPropertyChanged(IVMPropertyDescriptor property) {
         OnPropertyChanged(property);
      }

      void IViewModel.NotifyValidationStateChanged(IVMPropertyDescriptor property) {
         OnValidationStateChanged(property);
      }

      protected virtual void OnPropertyChanged(IVMPropertyDescriptor property) {
         OnPropertyChanged(property.PropertyName);
      }

      protected virtual void OnValidationStateChanged(IVMPropertyDescriptor property) {
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

      public ValidationState GetValidationState(ValidationStateScope scope) {
         return Kernel.GetValidationState(scope);
      }

      protected void Load(IVMPropertyDescriptor property) {
         GetDisplayValue(property);
      }

      //T IViewModel.GetValue<T>(IVMProperty<T> property) {
      //   return GetValue(property);
      //}

      //T IViewModel.GetValidatedValue<T>(IVMProperty<T> property) {
      //   return GetValidatedValue(property);
      //}

      //object IViewModel.GetDisplayValue(IVMProperty property) {
      //   return GetDisplayValue(property);
      //}


      //void IViewModel.SetDisplayValue(IVMProperty property, object value) {
      //   SetDisplayValue(property, value);
      //}
   }
}
