namespace Inspiring.Mvvm.Common {
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IBehaviorFactory {
      IBehavior Create(BehaviorKey key);
   }
}
