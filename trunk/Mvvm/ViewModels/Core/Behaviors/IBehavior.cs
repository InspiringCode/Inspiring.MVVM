namespace Inspiring.Mvvm.ViewModels.Behaviors {
   public interface IBehavior {
      IBehavior Successor { get; set; }

      BehaviorPosition Position { get; }
   }
}
