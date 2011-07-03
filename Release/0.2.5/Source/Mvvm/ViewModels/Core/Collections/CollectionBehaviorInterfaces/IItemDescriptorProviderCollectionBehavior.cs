namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IItemDescriptorProviderCollectionBehavior : IBehavior {
      IVMDescriptor ItemDescriptor { get; }
   }
}
