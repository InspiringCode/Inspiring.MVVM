namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class DefaultBehaviorFactory : IBehaviorFactory {
      private VMBehaviorKey _behavior;

      public DefaultBehaviorFactory(BehaviorKey behaviorToCreate) {
         throw new NotImplementedException();
      }

      [Obsolete]
      public DefaultBehaviorFactory(VMBehaviorKey behaviorToCreate) {
         _behavior = behaviorToCreate;
      }
      public IBehavior Create<TValue>() {
         switch (_behavior) {
            case VMBehaviorKey.InvalidDisplayValueCache:
               return new AllowInvalidDisplayValuesBehavior();
            case VMBehaviorKey.DisplayValueValidator:
               return new DisplayValueValidationBehavior();
            case VMBehaviorKey.DisplayValueAccessor:
               return new DisplayValueAccessorBehavior<TValue>();
            case VMBehaviorKey.Validator:
               return new ValidationBehavior<TValue>();
            case VMBehaviorKey.PropertyValueCache:
               return new RefreshableValueCahche<TValue>();
            case VMBehaviorKey.PropertyChangedTrigger:
               return new PropertyChangedBehavior<TValue>();
            case VMBehaviorKey.CollectionValueCache:
               return new RefreshableValueCahche<TValue>();
            case VMBehaviorKey.CollectionValidator:
               throw new NotImplementedException("Validation is not implemented yet.");
            case VMBehaviorKey.CollectionInstanceCache:
               return new CacheValueBehavior<TValue>();
            case VMBehaviorKey.CommandValueCache:
               return new CacheValueBehavior<TValue>();
            case VMBehaviorKey.ViewModelValueCache:
               return new RefreshableValueCahche<TValue>();
            case VMBehaviorKey.ManualUpdateBehavior:
               return new DefaultManualUpdateBehavior<TValue>();
            case VMBehaviorKey.PropertyDescriptorBehavior:
               return new PropertyDescriptorBehavior();
            default:
               throw new NotSupportedException();
         }
      }
   }
}
