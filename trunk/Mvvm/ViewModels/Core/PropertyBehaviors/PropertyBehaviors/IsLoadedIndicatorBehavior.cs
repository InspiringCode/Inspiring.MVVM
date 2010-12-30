namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class IsLoadedIndicatorBehavior<TValue> :
      Behavior,
      IIsLoadedIndicatorBehavior {

      public bool IsLoaded(IBehaviorContext context) {
         return GetNextBehavior<ValueCacheBehavior<TValue>>().HasCachedValue(context);
      }
   }
}
