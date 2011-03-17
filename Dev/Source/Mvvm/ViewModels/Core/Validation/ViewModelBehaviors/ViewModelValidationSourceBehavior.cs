namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelValidationSourceBehavior :
      Behavior,
      IRevalidationBehavior {


      public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
         throw new NotImplementedException();
      }

      public void Revalidate(IBehaviorContext context, ValidationContext validationContext, ValidationMode mode) {
         throw new NotImplementedException();
      }

      public void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException();
      }
   }
}
