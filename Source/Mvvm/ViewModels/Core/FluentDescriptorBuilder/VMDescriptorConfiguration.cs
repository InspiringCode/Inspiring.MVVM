namespace Inspiring.Mvvm.ViewModels.Core {
   
   /// <summary>
   ///   Holds transient information about the <see cref="BehaviorChainConfiguration"/>s
   ///   of a VM descriptor and its properties.
   /// </summary>
   /// <remarks>
   ///   Once the actual <see cref="BehaviorChain"/>s are created an object of this
   ///   class is not needed anymore.
   /// </remarks>
   public sealed class VMDescriptorConfiguration {
      public VMDescriptorConfiguration(BehaviorChainConfiguration viewModelConfiguration) {
         Check.NotNull(viewModelConfiguration, nameof(viewModelConfiguration));

         PropertyConfigurations = new BehaviorChainConfigurationCollection();
         ViewModelConfiguration = viewModelConfiguration;
      }

      public BehaviorChainConfigurationCollection PropertyConfigurations { get; private set; }

      public BehaviorChainConfiguration ViewModelConfiguration { get; internal set; }

      /// <summary>
      ///   Creates concrete <see cref="BehaviorChain"/>s from the <see 
      ///   cref="BehaviorChainConfiguration"/>s and assigns them to the <paramref 
      ///   name="descriptor"/> and its VM properties.
      /// </summary>
      internal void ApplyTo(IVMDescriptor descriptor) {

         var chain = ViewModelConfiguration.CreateChain();
         chain.Initialize(descriptor);
         descriptor.Behaviors = chain;

         PropertyConfigurations.ApplyToProperties(descriptor);
      }
   }
}
