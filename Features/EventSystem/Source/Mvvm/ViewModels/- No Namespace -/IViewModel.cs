namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;


   public interface IViewModel<out TDescriptor> : IViewModel where TDescriptor : IVMDescriptor {

   }

   public interface IViewModelExpression<in TDescriptor> : IViewModel where TDescriptor : IVMDescriptor {

   }

   public interface IViewModel {
      VMKernel Kernel { get; }
      IVMDescriptor Descriptor { get; set; }
      IBehaviorContext GetContext();
      void NotifyChange(ChangeArgs args);
   }
}
