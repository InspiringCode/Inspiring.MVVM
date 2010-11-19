namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Future;

   internal abstract class ViewModelNew<TDescriptor> :
      CustomTypeDescriptor,
      IViewModel
      where TDescriptor : VMDescriptorBase {

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

      protected VMKernel Kernel {
         get {
            Contract.Requires<InvalidOperationException>(
               DescriptorBase != null,
               ExceptionTexts.DescriptorNotSet
            );

            Contract.Ensures(Contract.Result<VMKernel>() != null);

            if (_kernel == null) {
               _kernel = new VMKernel(this, DescriptorBase);
            }

            return _kernel;
         }
      }

      public T GetValue<T>(VMPropertyBase<T> property, ValueStage state = ValueStage.PreValidation) {
         throw new NotImplementedException("Refactor VMProperty");
      }

      public void SetValue<T>(VMPropertyBase<T> property, T value) {
         throw new NotImplementedException("Refactor VMProperty");
      }

      public object GetDisplayValue(IVMProperty property) {
         throw new NotImplementedException("Refactor VMProperty");
      }

      public void SetDisplayValue(IVMProperty property, object value) {
         throw new NotImplementedException("Refactor VMProperty");
      }


      public object GetValue(IVMProperty property, ValueStage stage) {
         throw new NotImplementedException();
      }

      public void SetValue(IVMProperty property, object value) {
         throw new NotImplementedException();
      }


      object IViewModel.GetValue(IVMProperty property, ValueStage stage) {
         throw new NotImplementedException();
      }

      void IViewModel.SetValue(IVMProperty property, object value) {
         throw new NotImplementedException();
      }

      bool IViewModel.IsValid(bool validateChildren) {
         throw new NotImplementedException();
      }

      void IViewModel.Revalidate() {
         throw new NotImplementedException();
      }

      event EventHandler<ValidationEventArgs> IViewModel.Validating {
         add { throw new NotImplementedException(); }
         remove { throw new NotImplementedException(); }
      }

      event EventHandler<ValidationEventArgs> IViewModel.Validated {
         add { throw new NotImplementedException(); }
         remove { throw new NotImplementedException(); }
      }


      void IViewModel.InvokeValidate(IViewModel changedVM, VMPropertyBase changedProperty) {
         throw new NotImplementedException();
      }
   }
}
