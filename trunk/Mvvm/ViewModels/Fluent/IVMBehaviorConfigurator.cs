using Inspiring.Mvvm.ViewModels.Behaviors;
namespace Inspiring.Mvvm.ViewModels.Fluent {

   public interface IVMBehaviorConfigurator {
      void EnableBehavior(VMBehaviorFactory behavior);
      void EnableBehavior(VMBehaviorFactory behavior, params VMProperty[] forProperties);
   }
}
