namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IVMBehaviorConfigurator {
      void Custom<T>(VMProperty<T> property, VMBehaviorKey behaviorToEnable);
   }
}
