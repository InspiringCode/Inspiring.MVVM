namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public static class BehaviorExtensions {
      public static void IsCached<TVM, TDescriptor, TValue>(
         this ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> builder
      ) {
         if (builder.ContainsBehavior(PropertyBehaviorKeys.ValueCache)) {
            builder.Enable(PropertyBehaviorKeys.ValueCache);
         }
      }

      // We have to use Object as type because we describe the 'DisplayValue'
      // of the property, which is of type object. This is necessary to allow
      // the view model to make type conversions. If we would return the real
      // type of the property, the .NET Binding system would try to do the
      // conversion.
      public static void SupportsDisplayValueConversion<TVM, TDescriptor, TValue>(
         this ISinglePropertyBehaviorBuilder<TVM, TDescriptor, TValue> builder,
         bool supportsConversion = true
      ) {
         builder.Configure<PropertyDescriptorProviderBehavior<TValue>>(
            PropertyBehaviorKeys.PropertyDescriptorProvider,
            b => b.ReturnActualType = !supportsConversion
         );
      }
   }
}
