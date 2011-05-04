namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IVMDescriptor {
      BehaviorChain Behaviors { get; set; }
      VMPropertyCollection Properties { get; }
      FieldDefinitionCollection Fields { get; }
   }
}