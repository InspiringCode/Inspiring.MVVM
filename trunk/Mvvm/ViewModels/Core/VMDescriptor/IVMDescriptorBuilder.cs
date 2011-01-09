namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   An interface that enables a non-generic <see cref="VMDescriptor"/>
   ///   to hold a generic <see cref="VMDescriptorBuilder{TDescriptor, TVM}"/>.
   /// </summary>
   internal interface IVMDescriptorBuilder {
      void ConfigureDescriptor(VMDescriptor descriptor, VMDescriptorConfiguration configuration);
   }
}
