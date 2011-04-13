namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class CommandPropertyProvider : SimplePropertyProvider {
      protected override BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>() {
         return base
            .GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>();
      }
   }
}
