namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IBehaviorFactory {
      IBehavior Create<TValue>();
   }
}
