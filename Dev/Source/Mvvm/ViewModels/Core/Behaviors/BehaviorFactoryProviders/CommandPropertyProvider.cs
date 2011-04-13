namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class CommandPropertyProvider : SimplePropertyProvider {
      protected override BehaviorFactory GetCommonFactory<TOwnerVM, TValue, TSourceObject>() {
         return base
            .GetCommonFactory<TOwnerVM, TValue, TSourceObject>();
      }
   }
}
