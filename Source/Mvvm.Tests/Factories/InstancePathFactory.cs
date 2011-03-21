// <copyright file="InstancePathFactory.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>

using System;
using Microsoft.Pex.Framework;

namespace Inspiring.Mvvm.ViewModels.Core {
   public static partial class InstancePathFactory {
      [PexFactoryMethod(typeof(InstancePath))]
      public static object Create(IViewModel[] steps) {
         if (steps == null) {
            throw new ArgumentNullException();
         }
         return new InstancePath(steps);
      }

      [PexFactoryMethod(typeof(ViewModelStub))]
      public static ViewModelStub CreateViewModel() {
         return new ViewModelStub();
      }

      public class ViewModelStub : IViewModel {
         public VMKernel Kernel {
            get { throw new System.NotImplementedException(); }
         }

         public object GetValue(IVMProperty property) {
            throw new NotImplementedException();
         }

         public void SetValue(IVMProperty property, object value) {
            throw new NotImplementedException();
         }

         VMKernel IViewModel.Kernel {
            get { throw new NotImplementedException(); }
         }

         object IViewModel.GetValue(IVMProperty property) {
            throw new NotImplementedException();
         }

         void IViewModel.SetValue(IVMProperty property, object value) {
            throw new NotImplementedException();
         }

         public IBehaviorContext GetContext() {
            throw new NotImplementedException();
         }

         IBehaviorContext IViewModel.GetContext() {
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


         public void NotifyPropertyChanged(IVMProperty property) {
            throw new NotImplementedException();
         }

         public void NotifyValidationStateChanged(IVMProperty property) {
            throw new NotImplementedException();
         }


         public T GetValue<T>(IVMProperty<T> property) {
            throw new NotImplementedException();
         }

         public T GetValidatedValue<T>(IVMProperty<T> property) {
            throw new NotImplementedException();
         }
      }
   }
}