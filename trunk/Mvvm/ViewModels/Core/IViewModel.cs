namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IVMCollectionExpression<out TItem> {

   }

   public interface IViewModel<in TDescriptor> : IViewModel where TDescriptor : VMDescriptorBase {

   }
}
