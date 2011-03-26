namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   public class BehaviorKeys {
      protected static BehaviorKey Key(Expression<Func<BehaviorKey>> behaviorFieldSelector) {
         string key = ExpressionService.GetPropertyName(behaviorFieldSelector);
         return new BehaviorKey(key);
      }
   }
}
