namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public interface IValidationBuilder<TVM> : IHideObjectMembers where TVM : ViewModel {
      IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property);
      ICollectionValidationBuilder<TItemVM> CheckCollection<TItemVM>(
         IVMProperty<VMCollection<TItemVM>> property
      ) where TItemVM : ViewModel;
   }

   public interface IValidationBuilder<TVM, TValue> : IHideObjectMembers
      where TVM : ViewModel {

      void Custom(Func<TVM, TValue, ValidationResult> validation);

      IValidationBuilder<TParentVM, TVM, TValue> WithParent<TParentVM>() where TParentVM : ViewModel;
   }

   public interface IValidationBuilder<TParentVM, TVM, TValue> : IHideObjectMembers
      where TVM : ViewModel
      where TParentVM : ViewModel {

      void Custom(Func<TParentVM, TVM, TValue, ValidationResult> validation);
   }

   public interface ICollectionValidationBuilder<TItemVM> where TItemVM : ViewModel {
      void Custom(CollectionValidator<TItemVM> validation);
   }

   public delegate void CollectionValidator<TItemVM>(
      TItemVM itemToValidate,
      IEnumerable<TItemVM> allItems,
      ValidationEventArgs args
   );
}
