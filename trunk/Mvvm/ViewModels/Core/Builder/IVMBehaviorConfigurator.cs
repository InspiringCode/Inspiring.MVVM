namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IVMBehaviorConfigurator {
      void Custom<T>(VMPropertyBase<T> property, VMBehaviorKey behaviorToEnable);
   }
}
