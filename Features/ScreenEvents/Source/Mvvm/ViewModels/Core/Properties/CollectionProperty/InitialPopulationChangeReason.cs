namespace Inspiring.Mvvm.ViewModels.Core {

   public sealed class InitialPopulationChangeReason : IChangeReason {
      public static readonly InitialPopulationChangeReason Instance = new InitialPopulationChangeReason();

      private InitialPopulationChangeReason() {
      }
   }
}
