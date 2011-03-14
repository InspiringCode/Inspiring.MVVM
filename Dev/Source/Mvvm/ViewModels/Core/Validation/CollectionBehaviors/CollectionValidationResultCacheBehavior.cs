namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   internal sealed class CollectionValidationResultCacheBehavior {
      public IEnumerable<ValidationError> GetItemErrors(
         IViewModel item,
         IVMPropertyDescriptor property
      ) {
         throw new NotImplementedException();
      }

      public void SetErrors(
         IVMPropertyDescriptor property,
         IEnumerable<ValidationError> collectionValidatorErrors
      ) {

      }
   }
}
