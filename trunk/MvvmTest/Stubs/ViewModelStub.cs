﻿namespace Inspiring.MvvmTest.Stubs {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelStub : IViewModel {
      public ViewModelStub() {
         VMDescriptorBase descriptor = new VMDescriptorStub();
         Kernel = new VMKernel(this, descriptor, ServiceLocator.Current);
      }

      public VMKernel Kernel {
         get;
         private set;
      }

      public object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation) {
         throw new NotImplementedException();
      }

      public void SetValue(IVMProperty property, object value) {
         throw new NotImplementedException();
      }

      public bool IsValid(bool validateChildren) {
         throw new NotImplementedException();
      }

      public void Revalidate() {
         throw new NotImplementedException();
      }

      public event EventHandler<ValidationEventArgs> Validating;

      public event EventHandler<ValidationEventArgs> Validated;

      public void InvokeValidate(IViewModel changedVM, VMPropertyBase changedProperty) {
         throw new NotImplementedException();
      }

      public IViewModel Parent { get; set; }


      public IBehaviorContext GetContext() {
         throw new NotImplementedException();
      }


      public void RaisePropertyChanged(string propertyName) {
         throw new NotImplementedException();
      }


      public VMDescriptorBase Descriptor {
         get {
            throw new NotImplementedException();
         }
         set {
            throw new NotImplementedException();
         }
      }
   }
}
