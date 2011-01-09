namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;


   public interface IViewModel<in TDescriptor> : IViewModel where TDescriptor : VMDescriptorBase {

   }

   //[ContractClass(typeof(IViewModelContract))]
   public interface IViewModel {
      VMKernel Kernel { get; }
      VMDescriptorBase Descriptor { get; set; }

      //T GetValue<T>(IVMProperty<T> property);

      //T GetValidatedValue<T>(IVMProperty<T> property);

      //object GetDisplayValue(IVMProperty property);

      //void SetDisplayValue(IVMProperty property, object value);

      //object GetValue(IVMProperty property);
      //void SetValue(IVMProperty property, object value);

      IBehaviorContext GetContext();

      void NotifyPropertyChanged(IVMPropertyDescriptor property);
      void NotifyValidationStateChanged(IVMPropertyDescriptor property);
   }

   namespace Contracts {
      // TODO
      //[ContractClassFor(typeof(IViewModel))]
      //internal abstract class IViewModelContract : IViewModel {

      //   public VMKernel Kernel {
      //      get { return null; }
      //   }

      //   public object GetValue(IVMProperty property) {
      //      throw new System.NotImplementedException();
      //   }

      //   public void SetValue(IVMProperty property, object value) {
      //      throw new System.NotImplementedException();
      //   }

      //   public IBehaviorContext GetContext() {
      //      throw new System.NotImplementedException();
      //   }

      //   public void NotifyPropertyChanged(string propertyName) {
      //      throw new System.NotImplementedException();
      //   }

      //   public VMDescriptorBase Descriptor {
      //      get {
      //         return null;
      //      }
      //      set {
      //         Contract.Requires<InvalidOperationException>(
      //            Descriptor == null || Descriptor == value,
      //            ExceptionTexts.DescriptorCannotBeChanged
      //         );
      //      }
      //   }


      //   public void NotifyPropertyChanged(IVMProperty property) {
      //      throw new NotImplementedException();
      //   }

      //   public void NotifyValidationStateChanged(IVMProperty property) {
      //      throw new NotImplementedException();
      //   }
      //}
   }
}
