﻿namespace Inspiring.MvvmTest.Stubs {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   // TODO: Remove
   public class ViewModelStub : IViewModel {
      private Dictionary<IVMPropertyDescriptor, object> _fakeValues;
      private IBehaviorContext _context;

      public ViewModelStub()
         : this(new VMDescriptorStub()) {

      }

      public ViewModelStub(IVMDescriptor descriptor) {
         _fakeValues = new Dictionary<IVMPropertyDescriptor, object>();
         Descriptor = descriptor;
         Kernel = new VMKernel(this, Descriptor, ServiceLocator.Current);
         _context = Kernel;
      }

      public ViewModelStub(Behavior behavior)
         : this() {
         Descriptor.Behaviors.Successor = behavior;
         Descriptor.Behaviors.Initialize(Descriptor);
      }

      public void OverrideContext(IBehaviorContext context) {
         _context = context;
      }

      public VMKernel Kernel {
         get;
         private set;
      }



      public object GetValue(IVMPropertyDescriptor property) {
         return _fakeValues[property];
      }

      public void SetValue<T>(IVMPropertyDescriptor<T> property, T value) {
         property.Behaviors.SetValueNext(_context, value);
         _fakeValues[property] = value;
      }

      public bool IsValid(bool validateChildren) {
         throw new NotImplementedException();
      }

      public void Revalidate() {
         throw new NotImplementedException();
      }

      public void InvokeValidate(IViewModel changedVM, IVMPropertyDescriptor changedProperty) {
         throw new NotImplementedException();
      }

      public IViewModel Parent { get; set; }


      public IBehaviorContext GetContext() {
         return _context;
      }


      public void NotifyPropertyChanged(string propertyName) {
         throw new NotImplementedException();
      }


      public IVMDescriptor Descriptor {
         get;
         set;
      }

      public T GetValue<T>(IVMPropertyDescriptor<T> property) {
         return (T)GetValue((IVMPropertyDescriptor)property);
      }

      public T GetValidatedValue<T>(IVMPropertyDescriptor<T> property) {
         return (T)GetValue((IVMPropertyDescriptor)property);
      }

      public object GetDisplayValue(IVMPropertyDescriptor property) {
         return GetValue(property);
      }


      public void SetDisplayValue(IVMPropertyDescriptor property, object value) {
         Kernel.SetDisplayValue(property, value);
      }


      public void NotifyChange(ChangeArgs args) {
         throw new NotImplementedException();
      }
   }
}
