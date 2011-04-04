namespace Inspiring.Mvvm.ViewModels.Core.GeneralBehaviors {

   internal sealed class DefaultValueFactoryBehavior<TValue> :
      Behavior,
      IValueFactoryBehavior<TValue> {

      public TValue CreateValue(IBehaviorContext context) {
         return default(TValue);
      }
   }
}
