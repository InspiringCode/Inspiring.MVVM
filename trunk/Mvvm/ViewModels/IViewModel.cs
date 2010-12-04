namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   [ContractClass(typeof(IViewModelContract))]
   public interface IViewModel {
      VMKernel Kernel { get; }
      object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation);
      void SetValue(IVMProperty property, object value);
      IBehaviorContext GetContext();
      void RaisePropertyChanged(string propertyName);
      VMDescriptorBase Descriptor { get; set; }
   }

   namespace Contracts {
      // TODO
      [ContractClassFor(typeof(IViewModel))]
      internal abstract class IViewModelContract : IViewModel {

         public VMKernel Kernel {
            get { return null; }
         }

         public object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation) {
            throw new System.NotImplementedException();
         }

         public void SetValue(IVMProperty property, object value) {
            throw new System.NotImplementedException();
         }

         public IBehaviorContext GetContext() {
            throw new System.NotImplementedException();
         }

         public void RaisePropertyChanged(string propertyName) {
            throw new System.NotImplementedException();
         }

         public VMDescriptorBase Descriptor {
            get {
               return null;
            }
            set {
               Contract.Requires<InvalidOperationException>(
                  Descriptor == null || Descriptor == value,
                  ExceptionTexts.DescriptorCannotBeChanged
               );
            }
         }
      }
   }
}
