namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public interface IValidationBuilder<TVM> : IHideObjectMembers where TVM : IViewModel {
      IValidationBuilder<TVM, TValue> Check<TValue>(VMProperty<TValue> property);
      ICollectionValidationBuilder<TItemVM> CheckCollection<TItemVM>(
         IVMProperty<VMCollection<TItemVM>> property
      ) where TItemVM : IViewModel;
      void ViewModelValidator(Action<TVM, _ViewModelValidationArgs> validator);
   }

   public interface IValidationBuilder<TVM, TValue> : IHideObjectMembers
      where TVM : IViewModel {

      void Custom(Func<TVM, TValue, ValidationResult> validation);

      IValidationBuilder<TParentVM, TVM, TValue> WithParent<TParentVM>() where TParentVM : IViewModel;
   }

   public interface IValidationBuilder<TParentVM, TVM, TValue> : IHideObjectMembers
      where TVM : IViewModel
      where TParentVM : IViewModel {

      void Custom(Func<TParentVM, TVM, TValue, ValidationResult> validation);
   }

   public interface ICollectionValidationBuilder<TItemVM> where TItemVM : IViewModel {
      void Custom(CollectionValidator<TItemVM> validation);
      ICollectionValidationBuilder<TItemVM, TValue> Check<TValue>(VMProperty<TValue> itemProperty);
   }

   public interface ICollectionValidationBuilder<TItemVM, TValue> where TItemVM : IViewModel {
      void Custom(Action<ValidationEventArgs<TItemVM, TValue>> validator);
   }

   public delegate void CollectionValidator<TItemVM>(
      TItemVM itemToValidate,
      IEnumerable<TItemVM> allItems,
      ValidationEventArgs args
   );

   public sealed class ValidationEventArgs<TItemVM, TItemValue> where TItemVM : IViewModel {
      private ValidationEventArgs _args;

      internal ValidationEventArgs(
         IEnumerable<ValidationValue<TItemVM, TItemValue>> allItems,
         ValidationEventArgs args
      ) {
         _args = args;
         AllItems = allItems;
         Item = new ValidationValue<TItemVM, TItemValue>(
            (TItemVM)args.VM,
            (TItemValue)args.PropertyValue
         );
      }

      public IEnumerable<ValidationValue<TItemVM, TItemValue>> AllItems { get; private set; }

      public ValidationValue<TItemVM, TItemValue> Item { get; private set; }

      public bool AffectsOtherItems {
         get { return _args.AffectsOtherItems; }
         set { _args.AffectsOtherItems = value; }
      }

      public void AddError(string errorMessage) {
         _args.AddError(errorMessage);
      }
   }

   public sealed class ValidationValue<TItemVM, TItemValue> {
      internal ValidationValue(TItemVM vm, TItemValue value) {
         VM = vm;
         Value = value;
      }

      public TItemVM VM { get; private set; }
      public TItemValue Value { get; private set; }
   }
}
