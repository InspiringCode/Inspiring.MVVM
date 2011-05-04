namespace Inspiring.Mvvm.ViewModels.Core.FluentDescriptorBuilder.Dependencies {

   public class VMDependencyBuilder<TVM, TDescriptor>
      : IVMDependencyBuilder<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptor {


      public IDependencySelfSourceBuilder<TVM, TVM, TDescriptor, TDescriptor> OnChangeOf {
         get { throw new System.NotImplementedException(); }
      }
   }
}
