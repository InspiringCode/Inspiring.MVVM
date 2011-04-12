namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   public class BehaviorChainTemplateKeys {
      public static readonly BehaviorChainTemplateKey Property = Key(() => Property);


      public static readonly BehaviorChainTemplateKey ViewModel = new BehaviorChainTemplateKey("ViewModel");
      public static readonly BehaviorChainTemplateKey CollectionProperty = new BehaviorChainTemplateKey("CollectionProperty");
      public static readonly BehaviorChainTemplateKey CommandProperty = new BehaviorChainTemplateKey("CommandProperty");
      public static readonly BehaviorChainTemplateKey ViewModelProperty = new BehaviorChainTemplateKey("ViewModelProperty");
      public static readonly BehaviorChainTemplateKey DefaultCollectionBehaviors = new BehaviorChainTemplateKey("DefaultCollectionBehaviors");
      public static readonly BehaviorChainTemplateKey CommandBehaviors = new BehaviorChainTemplateKey("CommandBehaviors");

      protected static BehaviorChainTemplateKey Key(Expression<Func<BehaviorChainTemplateKey>> behaviorFieldSelector) {
         string key = ExpressionService.GetPropertyName(behaviorFieldSelector);
         return new BehaviorChainTemplateKey(key);
      }
   }
}
