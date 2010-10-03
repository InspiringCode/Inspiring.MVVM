namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   public interface IValidationBuilder<TVM> : IHideObjectMembers where TVM : ViewModel {
      IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property);
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
}
