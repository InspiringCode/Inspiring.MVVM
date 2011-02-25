namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.Properties {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Fluent;

   /// <inheritdoc />
   internal sealed class ValuePropertyBuilder<TSourceObject> : IValuePropertyBuilder<TSourceObject> {
      /// <inheritdoc />
      public VMProperty<T> MapsTo<T>(Expression<Func<TSourceObject, T>> sourcePropertySelector) {
         throw new NotImplementedException();
      }

      /// <inheritdoc />
      public VMProperty<T> DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter = null
      ) {
         throw new NotImplementedException();
      }

      /// <inheritdoc />
      public VMProperty<T> Of<T>() {
         throw new NotImplementedException();
      }
   }
}
