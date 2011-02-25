namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder;

   /// <summary>
   ///   A fluent builder that creates and configures a <see cref="VMDescriptor"/>
   ///   that can be used for a <see cref="ViewModel"/>.
   /// </summary>
   public static class VMDescriptorBuilder {
      /// <summary>
      ///   Selects the descriptor type that should be created.
      /// </summary>
      public static VMDescriptorBuilder<TDescriptor> OfType<TDescriptor>()
         where TDescriptor : VMDescriptor, new() {
         return new VMDescriptorBuilder<TDescriptor>(baseBuilder: null);
      }

      /// <summary>
      ///   Selects the base descriptor on which this descriptor is based.
      /// </summary>
      /// <param name="descriptor">
      ///   A <see cref="VMDescriptor"/> that provides a base configuration
      ///   (property mappings, validators, behaviors, ...) for this descriptor
      ///   which can selectively be extended and modified.
      /// </param>
      public static DerivedVMDescriptorBuilder<TBaseDescriptor> Inherits<TBaseDescriptor>(
         TBaseDescriptor descriptor
      ) where TBaseDescriptor : VMDescriptor {
         Contract.Requires<ArgumentNullException>(descriptor != null);
         Contract.Assert(
            descriptor.Builder != null,
            "The base descriptor was not created by the 'VMDescriptorBuilder'."
         );

         return new DerivedVMDescriptorBuilder<TBaseDescriptor>(descriptor.Builder);
      }
   }
}
