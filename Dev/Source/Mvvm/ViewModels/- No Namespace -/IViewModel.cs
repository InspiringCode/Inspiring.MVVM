namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;


   public interface IViewModel<out TDescriptor> : IViewModel where TDescriptor : VMDescriptorBase {

   }

   public interface IViewModelExpression<in TDescriptor> : IViewModel where TDescriptor : VMDescriptorBase {

   }

   public interface IViewModel {
      VMKernel Kernel { get; }
      VMDescriptorBase Descriptor { get; set; }
      IBehaviorContext GetContext();
      void NotifyChange(ChangeArgs args);
   }
}
