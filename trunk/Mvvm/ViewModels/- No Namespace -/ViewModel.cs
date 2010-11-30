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
      INotifyPropertyChanged,
      IDataErrorInfo
      where TDescriptor : VMDescriptorBase {

      public ViewModel() {

      }

      public ViewModel(TDescriptor descriptor, IServiceLocator serviceLocator = null) {
         DescriptorBase = descriptor;
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

      protected TDescriptor DescriptorBase {
         get;
         set;
      }

      protected IServiceLocator ServiceLocator {
         get;
         private set;
      }

      protected VMKernel Kernel {
         get {
            Contract.Requires<InvalidOperationException>(
               DescriptorBase != null,
               ExceptionTexts.DescriptorNotSet
            );

            Contract.Ensures(Contract.Result<VMKernel>() != null);

            if (_kernel == null) {
               _kernel = new VMKernel(this, DescriptorBase, ServiceLocator);
            }

            return _kernel;
         }
      }

      string IDataErrorInfo.Error {
         get {
            ValidationState state = Kernel.GetValidationState();
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

      protected internal T GetValue<T>(VMPropertyBase<T> property, ValueStage stage = ValueStage.PreValidation) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.GetValue(Kernel, stage);
      }

      protected internal void SetValue<T>(VMPropertyBase<T> property, T value) {
         Contract.Requires<ArgumentNullException>(property != null);
         property.SetValue(Kernel, value);
      }

      protected internal object GetDisplayValue(VMPropertyBase property) {
         Contract.Requires<ArgumentNullException>(property != null);
         return property.GetDisplayValue(Kernel);
      }

      protected internal void SetDisplayValue(VMPropertyBase property, object value) {
         Contract.Requires<ArgumentNullException>(property != null);
         property.SetDisplayValue(Kernel, value);
      }

      object IViewModel.GetValue(IVMProperty property, ValueStage stage) {
         return property.GetValue(Kernel, stage);
      }

      void IViewModel.SetValue(IVMProperty property, object value) {
         property.SetValue(Kernel, value);
      }

      public IBehaviorContext GetContext() {
         return Kernel;
      }

      void IViewModel.RaisePropertyChanged(string propertyName) {
         OnPropertyChanged(propertyName);
      }

      protected virtual void OnPropertyChanged(string propertyName) {
         var handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      protected override PropertyDescriptorCollection GetPropertyDescriptors() {
         Contract.Requires<InvalidOperationException>(
            DescriptorBase != null,
            ExceptionTexts.DescriptorNotSet
         );

         return DescriptorBase
            .Behaviors
            .GetNextBehavior<TypeDescriptorBehavior>()
            .PropertyDescriptors;
      }
   }
}
