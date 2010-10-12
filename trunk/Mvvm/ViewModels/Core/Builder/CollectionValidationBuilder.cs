namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionValidationBuilder<TItemVM> :
      ICollectionValidationBuilder<TItemVM>
      where TItemVM : ViewModel {

      public void Custom(CollectionValidator<TItemVM> validation) {
         throw new NotImplementedException();
      }
   }
}
