namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorChainTemplateKeys {
      public static readonly BehaviorChainTemplateKey ViewModel = new BehaviorChainTemplateKey("ViewModel");
      public static readonly BehaviorChainTemplateKey Property = new BehaviorChainTemplateKey("Property");
      public static readonly BehaviorChainTemplateKey CollectionProperty = new BehaviorChainTemplateKey("CollectionProperty");
      public static readonly BehaviorChainTemplateKey CommandProperty = new BehaviorChainTemplateKey("CommandProperty");
      public static readonly BehaviorChainTemplateKey ViewModelProperty = new BehaviorChainTemplateKey("ViewModelProperty");
      public static readonly BehaviorChainTemplateKey DefaultCollectionBehaviors = new BehaviorChainTemplateKey("DefaultCollectionBehaviors");
      public static readonly BehaviorChainTemplateKey CommandBehaviors = new BehaviorChainTemplateKey("CommandBehaviors");
   }
}
