﻿namespace Inspiring.MvvmTest.Stubs {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelStub : IViewModel {
      private Dictionary<IVMProperty, object> _fakeValues;

      public ViewModelStub()
         : this(new VMDescriptorStub()) {

      }

      public ViewModelStub(VMDescriptorBase descriptor) {
         _fakeValues = new Dictionary<IVMProperty, object>();
         Descriptor = descriptor;
         Kernel = new VMKernel(this, Descriptor, ServiceLocator.Current);
      }

      public VMKernel Kernel {
         get;
         private set;
      }



      public object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation) {
         return _fakeValues[property];
      }

      public void SetValue(IVMProperty property, object value) {
         _fakeValues[property] = value;
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
         get;
         set;
      }
   }
}
