namespace Inspiring.Mvvm.ViewModels.Core {
   public sealed class BehaviorInitializationContext {
      public BehaviorInitializationContext(
         IVMDescriptor descriptor,
         IVMPropertyDescriptor property = null
      ) {
         Check.NotNull(descriptor, nameof(descriptor));

         Fields = descriptor.Fields;
         Descriptor = descriptor;
         Property = property;
      }

      public FieldDefinitionCollection Fields { get; private set; }

      public IVMDescriptor Descriptor { get; private set; }

      public IVMPropertyDescriptor Property { get; private set; }
   }
}
