namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

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
         Contract.Requires(viewModelConfiguration != null);

         PropertyConfigurations = new BehaviorChainConfigurationCollection();
         ViewModelConfiguration = viewModelConfiguration;
      }

      public BehaviorChainConfigurationCollection PropertyConfigurations { get; private set; }

      public BehaviorChainConfiguration ViewModelConfiguration { get; private set; }

      /// <summary>
      ///   Creates concrete <see cref="BehaviorChain"/>s from the <see 
      ///   cref="BehaviorChainConfiguration"/>s and assigns them to the <paramref 
      ///   name="descriptor"/> and its VM properties.
      /// </summary>
      internal void ApplyTo(VMDescriptorBase descriptor) {
         Contract.Ensures(descriptor.Behaviors != null);

         var chain = ViewModelConfiguration.CreateChain();
         chain.Initialize(descriptor);
         descriptor.Behaviors = chain;

         PropertyConfigurations.ApplyToProperties(descriptor);
      }
   }
}
