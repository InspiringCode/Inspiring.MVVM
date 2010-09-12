namespace Inspiring.Mvvm.ViewModels.Core.New {
   using Inspiring.Mvvm.ViewModels.Behaviors;

   public interface IBehaviorFactory {
      IBehavior Create<TValue>();
   }
}
