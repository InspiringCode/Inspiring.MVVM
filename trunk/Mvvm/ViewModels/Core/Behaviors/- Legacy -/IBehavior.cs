namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      void Initialize(BehaviorInitializationContext context);
   }
}
