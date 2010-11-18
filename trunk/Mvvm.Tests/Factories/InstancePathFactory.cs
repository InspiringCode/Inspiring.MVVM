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
            throw new System.NotImplementedException();
         }


         public object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation) {
            throw new NotImplementedException();
         }

         public void SetValue(IVMProperty property, object value) {
            throw new NotImplementedException();
         }
      }
   }
}
