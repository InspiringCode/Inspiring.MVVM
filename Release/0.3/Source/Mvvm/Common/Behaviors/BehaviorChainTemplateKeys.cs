namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   public abstract class BehaviorChainTemplateKeys {
      protected static BehaviorChainTemplateKey Key(Expression<Func<BehaviorChainTemplateKey>> behaviorFieldSelector) {
         string key = ExpressionService.GetPropertyName(behaviorFieldSelector);
         return new BehaviorChainTemplateKey(key);
      }
   }
}
