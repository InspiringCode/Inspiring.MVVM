namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public abstract class PropertyProvider {
      protected virtual BehaviorFactory GetFactoryWithCommonBehaviors<TOwnerVM, TValue, TSourceObject>() {
         return new BehaviorFactory()
            .RegisterBehavior<DisplayValueAccessorBehavior<TValue>>(PropertyBehaviorKeys.DisplayValueAccessor)
            .RegisterBehavior<UntypedPropertyAccessorBehavior<TValue>>(PropertyBehaviorKeys.UntypedValueAccessor)
            .RegisterBehavior<UndoSetValueBehavior<TValue>>(PropertyBehaviorKeys.Undo)
            .RegisterBehavior<ValueValidationSourceBehavior<TValue>>(PropertyBehaviorKeys.ValueValidationSource)
            .RegisterBehavior<PropertyChangedNotifierBehavior<TValue>>(PropertyBehaviorKeys.PropertyChangedNotifier)
            .RegisterBehavior<PropertyDescriptorProviderBehavior>(PropertyBehaviorKeys.PropertyDescriptorProvider);
      }
   }
}
