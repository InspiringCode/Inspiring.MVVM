namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IVMBehaviorConfigurator {
      void Custom<T>(IVMProperty<T> property, VMBehaviorKey behaviorToEnable);
   }
}
