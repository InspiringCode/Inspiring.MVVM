namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class IsLoadedIndicatorBehavior<TValue> :
      Behavior,
      IIsLoadedIndicatorBehavior {

      public bool IsLoaded(IBehaviorContext context) {
         ValueCacheBehaviorOld<TValue> cacheBehavior;
         return TryGetBehavior(out cacheBehavior) ?
            cacheBehavior.HasCachedValue(context) :
            true;
      }
   }
}
