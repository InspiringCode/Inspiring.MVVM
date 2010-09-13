namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;

   internal sealed class DefaultBehaviorFactory : IBehaviorFactory {
      private VMBehaviorKey _behavior;

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
            case VMBehaviorKey.DisconnectedViewModelBehavior:
               return new CacheValueBehavior<TValue>();
            case VMBehaviorKey.PropertyChangedTrigger:
               throw new NotImplementedException();
            case VMBehaviorKey.CollectionValueCache:
               return new CacheValueBehavior<TValue>();
            case VMBehaviorKey.CollectionValidator:
               throw new NotImplementedException("Validation is not implemented yet.");
            case VMBehaviorKey.CollectionInstanceCache:
               return new CacheValueBehavior<TValue>();
            case VMBehaviorKey.CollectionFactory:
               // Some ugly reflection here because TValue is 'VMCollection<TVM>' but 
               // the behavior expects only 'TVM'.
               Type itemType = typeof(TValue).GetGenericArguments().Single();
               Type factoryType = typeof(CollectionFactoryBehavior<>).MakeGenericType(itemType);
               return (IBehavior)Activator.CreateInstance(factoryType);
            default:
               throw new NotSupportedException();
         }
      }
   }
}
